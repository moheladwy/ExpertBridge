// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.GroqLibrary.Settings;

/// <summary>
///     Contains the endpoints for the Groq API.
/// </summary>
public static class GroqApiEndpoints
{
    /// <summary>The base URL for the Groq API.</summary>
    public const string BaseUrl = "https://api.groq.com/openai/v1/";

    /// <summary>The endpoint for chat completions.</summary>
    public const string ChatCompletionsEndpoint = "chat/completions";

    /// <summary>The endpoint for audio transcriptions.</summary>
    public const string TranscriptionsEndpoint = "audio/transcriptions";

    /// <summary>The endpoint for audio translations.</summary>
    public const string TranslationsEndpoint = "audio/translations";

    /// <summary>The endpoint for retrieving all available models from the Groq API.</summary>
    public const string GetAllModelsEndpoint = "models";
}
