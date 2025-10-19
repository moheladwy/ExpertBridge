namespace ExpertBridge.Extensions.Resilience;

/// <summary>
/// Defines constant names for resilience pipeline strategies used throughout the ExpertBridge application.
/// Provides centralized pipeline name references for Polly resilience policies.
/// </summary>
public static class ResiliencePipelines
{
    /// <summary>
    /// Resilience pipeline for handling malformed JSON responses from AI/LLM model APIs (Groq, Ollama).
    /// Includes retry logic with exponential backoff and timeout policies for parsing failures.
    /// </summary>
    public const string MalformedJsonModelResponse = "MalformedJsonModelResponse";
}
