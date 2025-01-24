// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace AIBot.Config
{

    /// <summary>
    /// Azure AI Search service settings.
    /// </summary>
    public class AzureAISearchConfig
    {
        public const string ConfigSectionName = "AzureAISearch";

        [Required]
        public static string Endpoint { get; set; } = string.Empty;

        [Required]
        public static string ApiKey { get; set; } = string.Empty;
    }
}