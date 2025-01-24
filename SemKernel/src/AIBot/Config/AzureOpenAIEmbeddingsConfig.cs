// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace AIBot.Config;

/// <summary>
/// Azure OpenAI Embeddings service settings.
/// </summary>
public class AzureOpenAIEmbeddingsConfig
{
    public const string ConfigSectionName = "AzureOpenAIEmbeddings";

    [Required]
    public static string DeploymentName { get; set; } = string.Empty;

    [Required]
    public static string Endpoint { get; set; } = string.Empty;
}