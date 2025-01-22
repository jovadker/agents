using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;

namespace AIBot.Plugins
{
    /// <summary>
    /// Azure AI Search SK Plugin.
    /// It uses <see cref="ITextEmbeddingGenerationService"/> to convert string query to vector.
    /// It uses <see cref="IAzureAISearchService"/> to perform a request to Azure AI Search.
    /// </summary>
    sealed class AzureAISearchPlugin(
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        ITextEmbeddingGenerationService textEmbeddingGenerationService,
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        IAzureAISearchService searchService)
    {
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        private readonly ITextEmbeddingGenerationService _textEmbeddingGenerationService = textEmbeddingGenerationService;
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        private readonly IAzureAISearchService _searchService = searchService;

        [KernelFunction("Search")]
        [Description("Searches for the most relevant text data in the collection. {{collection='aoai'}}. This is a global knowledge base")]
        [return: Description("Search results in the global knowledge base")]
        public async Task<string> SearchAsync(
            string query,
            string collection,
            List<string> searchFields = null,
            CancellationToken cancellationToken = default)
        {
            // Convert string query to vector
            ReadOnlyMemory<float> embedding = await _textEmbeddingGenerationService.GenerateEmbeddingAsync(query, cancellationToken: cancellationToken);

            // Perform search
            return await _searchService.SearchAsync(collection, embedding, searchFields, cancellationToken) ?? string.Empty;
        }
    }

}
