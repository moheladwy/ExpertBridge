// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using System.Text.Json.Nodes;
using ExpertBridge.GroqLibrary.Clients;
using ExpertBridge.GroqLibrary.Interfaces;
using ExpertBridge.GroqLibrary.Models;
using ExpertBridge.GroqLibrary.Settings;

namespace ExpertBridge.GroqLibrary.Providers;

/// <summary>
///     Provides integration with Groq LLM (Large Language Model) API for text generation.
///     Implements <see cref="ILlmTextProvider" /> interface for consistent LLM operations.
/// </summary>
public sealed class GroqLlmTextProvider : ILlmTextProvider
{
    private readonly GroqApiChatCompletionClient _client;
    private readonly string _model;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GroqLlmTextProvider" /> class with a custom HTTP client.
    /// </summary>
    /// <param name="groqApiChatCompletionClient">
    ///     The <see cref="GroqApiChatCompletionClient" /> instance to be used for API requests.
    /// </param>
    public GroqLlmTextProvider(GroqApiChatCompletionClient groqApiChatCompletionClient)
    {
        _client = groqApiChatCompletionClient;
        _model = GroqTextModels.LLAMA3_70B_8192;
    }

    /// <summary>
    ///     Generates text based on the provided user prompt using the configured LLM model.
    /// </summary>
    /// <param name="userPrompt">The user's input prompt for text generation.</param>
    /// <returns>The generated text response from the model.</returns>
    public async Task<string> GenerateAsync(string userPrompt)
    {
        var request = new JsonObject
        {
            ["model"] = _model,
            ["messages"] = JsonSerializer.SerializeToNode(new[]
            {
                new { role = LlmRoles.UserRole, content = userPrompt }
            })
        };

        var response = await _client.CreateChatCompletionAsync(request);
        return response?["choices"]?[0]?["message"]?["content"]?.GetValue<string>() ?? string.Empty;
    }

    /// <summary>
    ///     Generates text based on both system and user prompts using the configured LLM model.
    /// </summary>
    /// <param name="systemPrompt">The system instructions or context for the generation.</param>
    /// <param name="userPrompt">The user's input prompt for text generation.</param>
    /// <returns>The generated text response from the model.</returns>
    public async Task<string> GenerateAsync(string systemPrompt, string userPrompt)
    {
        var request = new JsonObject
        {
            ["model"] = _model,
            ["messages"] = JsonSerializer.SerializeToNode(new[]
            {
                new { role = LlmRoles.SystemRole, content = systemPrompt },
                new { role = LlmRoles.UserRole, content = userPrompt }
            })
        };

        var response = await _client.CreateChatCompletionAsync(request);
        var result = response?["choices"]?[0]?["message"]?["content"]?.GetValue<string>() ?? string.Empty;

        var start = result.IndexOf('{', StringComparison.Ordinal);
        var end = result.LastIndexOf('}');

        return result
            .Substring(start, end - start + 1)
            .TrimEnd('`', '\n', '\r', ',');
    }
}
