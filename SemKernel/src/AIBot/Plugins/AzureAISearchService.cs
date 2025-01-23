using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Azure.Search.Documents;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AIBot.Plugins
{
    /// <summary>
    /// Implementation of Azure AI Search service.
    /// </summary>
    sealed class AzureAISearchService(SearchIndexClient indexClient) : IAzureAISearchService
    {
        private readonly List<string> _defaultVectorFields = ["contentVector"];

        private readonly SearchIndexClient _indexClient = indexClient;

        private const int maxSearchResults = 5;

        public async Task<string> SearchAsync(
            string collectionName,
            ReadOnlyMemory<float> vector,
            List<string> searchFields = null,
            CancellationToken cancellationToken = default)
        {
            // Get client for search operations
            SearchClient searchClient = _indexClient.GetSearchClient(collectionName);

            // Use search fields passed from Plugin or default fields configured in this class.
            List<string> fields = searchFields is { Count: > 0 } ? searchFields : _defaultVectorFields;

            // Configure request parameters
            VectorizedQuery vectorQuery = new(vector);
            fields.ForEach(vectorQuery.Fields.Add);

            SearchOptions searchOptions = new() { VectorSearch = new() { Queries = { vectorQuery } } };

            // Perform search request
            Response<SearchResults<IndexSchema>> response = await searchClient.SearchAsync<IndexSchema>(searchOptions, cancellationToken);

            List<IndexSchema> results = [];

            // Collect search results
            await foreach (SearchResult<IndexSchema> result in response.Value.GetResultsAsync())
            {
                results.Add(result.Document);
            }

            // Return text from first result.
            // In real applications, the logic can check document score, sort and return top N results
            // or aggregate all results in one text.
            // The logic and decision which text data to return should be based on business scenario. 
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (IndexSchema index in results)
            {
                sb.AppendLine(index.Content);
                i++;
                if (i >= maxSearchResults)
                {
                    break;
                }
            }
            return sb.ToString();
            //return results.FirstOrDefault()?.Content;
        }
    }

}
