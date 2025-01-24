// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace AIBot.Config;

/// <summary>
/// Azure OpenAI service settings.
/// </summary>
public class AzureOpenAIConfig
{
    public const string ConfigSectionName = "AzureOpenAI";

    [Required]
    public static string DeploymentName { get; set; } = string.Empty;

    [Required]
    public static string Endpoint { get; set; } = string.Empty;

     [Required]
    public static string ApiKey { get; set; } = string.Empty;
}