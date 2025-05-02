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
}
