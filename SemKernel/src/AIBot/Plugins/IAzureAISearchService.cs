using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AIBot.Plugins
{
    /// <summary>
    /// Abstraction for Azure AI Search service.
    /// </summary>
    interface IAzureAISearchService
    {
        Task<string> SearchAsync(
            string collectionName,
            ReadOnlyMemory<float> vector,
            List<string> searchFields = null,
            CancellationToken cancellationToken = default);
    }

}
