using System.Text.Json;
using ExpertBridge.Api.Models;
using ExpertBridge.GroqLibrary.Providers;

namespace ExpertBridge.Api.Services;

/// <summary>
///     A service for detecting NSFW (Not Safe for Work) content using the Groq Large Language Model (LLM) API.
///     This service provides functionality to analyze text and return NSFW detection results categorized
///     into various predefined metrics.
/// </summary>
public sealed class NSFWDetectionService
{
    /// <summary>
    ///     An instance of <see cref="GroqLlmTextProvider" /> used to interact with the Groq Large Language Model (LLM)
    ///     API for text-based generation.
    /// </summary>
    private readonly GroqLlmTextProvider _groqLlmTextProvider;

    /// <summary>
    ///     An instance of <see cref="JsonSerializerOptions" /> configured for deserializing JSON responses in a
    ///     case-insensitive manner,
    ///     ensuring robust parsing.
    /// </summary>
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    ///     Service for detecting NSFW (Not Safe for Work) content using Groq Large Language Model (LLM) API.
    ///     Handles interactions with the <see cref="GroqLlmTextProvider" /> for analyzing and generating text-related tasks.
    /// </summary>
    public NSFWDetectionService(GroqLlmTextProvider groqLlmTextProvider)
    {
        _groqLlmTextProvider = groqLlmTextProvider;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    ///     Analyzes the given text to detect NSFW (Not Safe for Work) content and categorizes it into various predefined
    ///     metrics.
    ///     Processes text using the Groq Large Language Model (LLM) API and returns the analysis as a structured response.
    /// </summary>
    /// <param name="text">The input text to be analyzed for NSFW content. Must not be null or empty.</param>
    /// <returns>A <see cref="NsfwDetectionResponse" /> object containing the detection results for multiple NSFW categories.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="text" /> is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the response fails deserialization or parsing.</exception>
    public async Task<NsfwDetectionResponse> DetectAsync(string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(text, nameof(text));
        try
        {
            var systemPrompt = GetSystemPrompt();
            var userPrompt = GetUserPrompt(text);
            var response = await _groqLlmTextProvider.GenerateAsync(systemPrompt, userPrompt);
            var result = JsonSerializer.Deserialize<NsfwDetectionResponse>(response, _jsonSerializerOptions)
                         ?? throw new InvalidOperationException(
                             "Failed to deserialize the nsfw detection response: null result");
            return result;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse the nsfw detection response: {ex.Message}", ex);
        }
    }

    /// <summary>
    ///     Loads and retrieves the output format schema for NSFW detection as a string
    ///     from a predefined schema file.
    /// </summary>
    /// <returns>
    ///     A string containing the contents of the NSFW detection output format schema file.
    /// </returns>
    private static string GetOutputFormatSchema() => File.ReadAllText("LlmOutputFormat/NSFWDetectionOutputFormat.json");

    /// <summary>
    ///     Generates the predefined system prompt used for interactions with the Groq LLM API.
    ///     Intended for internal use to ensure a standard prompt structure.
    /// </summary>
    /// <returns>A string representing the standardized system prompt utilized by the service.</returns>
    private static string GetSystemPrompt()
    {
        List<string> systemPrompt =
        [
            "You are an AI moderation assistant specialized in detecting various forms of toxicity in user‐generated text whether in English, Egyptian Arabic, or another language.",
            "When given a piece of text, you must assign a probability between 0 and 1 to each of the following categories:",
            "Toxicity, SevereToxicity, Obscene, Threat, Insult, IdentityAttack, SexualExplicit.",
            "Your response must be a single JSON object that exactly matches the provided “NsfwDetectionResponse” pydantic schema",
            "no additional fields or commentary.",
            "Validate that each value is a number rounded to five decimal places, and that every field from the schema appears in the output.\n"
        ];
        return string.Join("\n", systemPrompt);
    }

    /// <summary>
    ///     Constructs a user prompt string for use in interactions with an LLM (Large Language Model) API,
    ///     aimed at generating or analyzing text as part of NSFW content detection tasks.
    /// </summary>
    /// <returns>A formatted string representing the user prompt.</returns>
    private static string GetUserPrompt(string text)
    {
        List<string> userPrompt =
        [
            "Please analyze the following text and  return your classification according to the NSFW detection results.",
            "The text is: ",
            text,
            "The output format should be follow the following pydantic schema:",
            GetOutputFormatSchema()
        ];
        return string.Join("\n", userPrompt);
    }
}
