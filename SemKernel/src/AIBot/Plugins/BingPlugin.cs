using Microsoft.SemanticKernel;
using System.ComponentModel;
using System;
using Microsoft.SemanticKernel.Embeddings;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using Microsoft.SemanticKernel.Data;
using System.Text;
using AIBot.Config;

#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
namespace AIBot.Plugins
{
    public class BingPlugin
    {
        BingTextSearch m_TextSearch = new BingTextSearch(apiKey: AppCommonSettings.BingApiKey);
        // Build a text search plugin with Bing search and add to the kernel

        [KernelFunction("SearchInternet")]
        [Description("Searches for the most recent information in the internet through Bing API")]
        [return: Description("Search results with Name, Content and URL")]
        public async Task<string> SearchAsync(
           string query)
        {
            KernelSearchResults<TextSearchResult> textResults = await m_TextSearch.GetTextSearchResultsAsync(query, new() { Top = 5, Skip = 0 });

            StringBuilder sb = new StringBuilder();
            await foreach (TextSearchResult result in textResults.Results)
            {
                sb.AppendLine($"Name:  {result.Name}");
                sb.AppendLine($"Content: {result.Value}");
                sb.AppendLine($"Link:  {result.Link}");
                sb.AppendLine("-----------------------------");
            }
            return sb.ToString();
        }

    }
}
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.