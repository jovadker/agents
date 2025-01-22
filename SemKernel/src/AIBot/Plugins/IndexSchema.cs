using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AIBot.Plugins
{
    /// <summary>
    /// Custom index schema. It may contain any fields that exist in search index.
    /// </summary>
    sealed class IndexSchema
    {
        [JsonPropertyName("chunk_id")]
        public string ChunkId { get; set; }

        [JsonPropertyName("parent_id")]
        public string ParentId { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("chunk")]
        public string Chunk { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("contentVector")]
        public ReadOnlyMemory<float> Vector { get; set; }
    }
}
