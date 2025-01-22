using Microsoft.SemanticKernel.ChatCompletion;
using System.Collections.Generic;

namespace AIBot.Bots
{
    public class ConversationData
    {
        public string ConversationId { get; set; }
        public string Token { get; set; }
        public string ChannelId { get; set; }
        public string Watermark { get; set; }

        public List<HistoryContent> ChatHistory { get; set; }
    }

    public class HistoryContent
    {
        public AuthorRole AuthorRole { get; set; }
        public string Content { get; set; }

    }
}
