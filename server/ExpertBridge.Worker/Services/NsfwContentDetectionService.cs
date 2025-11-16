using System.Text.Json;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Extensions.Resilience;
using Groq.Core.Providers;
using Polly.Registry;
using ResiliencePipeline = Polly.ResiliencePipeline;

namespace ExpertBridge.Worker.Services;

/// <summary>
///     A service for detecting NSFW (Not Safe for Work) content using the Groq Large Language Model (LLM) API.
///     This service provides functionality to analyze text and return NSFW detection results categorized
///     into various predefined metrics.
/// </summary>
public sealed class NsfwContentDetectionService
{
    /// <summary>
    ///     An instance of <see cref="LlmTextProvider" /> used to interact with the Groq Large Language Model (LLM)
    ///     API for text-based generation.
    /// </summary>
    private readonly LlmTextProvider _llmTextProvider;

    /// <summary>
    ///     An instance of <see cref="JsonSerializerOptions" /> configured for deserializing JSON responses in a
    ///     case-insensitive manner,
    ///     ensuring robust parsing.
    /// </summary>
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    ///     An instance of <see cref="Polly.ResiliencePipeline" /> used to provide resilience mechanisms such as retries
    ///     and transient fault handling when interacting with the Groq Large Language Model (LLM) API.
    /// </summary>
    private readonly ResiliencePipeline _resiliencePipeline;

    /// <summary>
    ///     Service for detecting NSFW (Not Safe for Work) content using Groq Large Language Model (LLM) API.
    ///     Handles interactions with the <see cref="LlmTextProvider" /> for analyzing and generating text-related tasks.
    /// </summary>
    public NsfwContentDetectionService(
        LlmTextProvider llmTextProvider,
        ResiliencePipelineProvider<string> resilience)
    {
        _llmTextProvider = llmTextProvider;
        _resiliencePipeline = resilience.GetPipeline(ResiliencePipelines.MalformedJsonModelResponse);
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, AllowOutOfOrderMetadataProperties = true, AllowTrailingCommas = true
        };
    }

    /// <summary>
    ///     Analyzes the given text to detect NSFW (Not Safe for Work) content and categorizes it into various predefined
    ///     metrics.
    ///     Processes text using the Groq Large Language Model (LLM) API and returns the analysis as a structured response.
    /// </summary>
    /// <param name="text">The input text to be analyzed for NSFW content. Must not be null or empty.</param>
    /// <returns>
    ///     A <see cref="InappropriateLanguageDetectionResponse" /> object containing the detection results for multiple
    ///     NSFW categories.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="text" /> is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the response fails deserialization or parsing.</exception>
    public async Task<InappropriateLanguageDetectionResponse?> DetectAsync(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        try
        {
            InappropriateLanguageDetectionResponse result = null;

            // Use the resilience pipeline to handle transient errors and retries
            await _resiliencePipeline.ExecuteAsync(async ct =>
            {
                var systemPrompt = GetSystemPrompt();
                var userPrompt = GetUserPrompt(text);
                var response = await _llmTextProvider.GenerateAsync(systemPrompt, userPrompt);
                result = JsonSerializer.Deserialize<InappropriateLanguageDetectionResponse>(response,
                             _jsonSerializerOptions)
                         ?? throw new InvalidOperationException(
                             "Failed to deserialize the nsfw detection response: null result");
            });

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
    private static string GetOutputFormatSchema()
    {
        return File.ReadAllText("LlmOutputFormat/NSFWDetectionOutputFormat.json");
    }

    /// <summary>
    ///     Generates the predefined system prompt used for interactions with the Groq LLM API.
    ///     Intended for internal use to ensure a standard prompt structure.
    /// </summary>
    /// <returns>A string representing the standardized system prompt utilized by the service.</returns>
    private static string GetSystemPrompt()
    {
        List<string> systemPrompt =
        [
            "You are an AI moderation system specializing in the detection of NSFW and toxic language across multiple languages, including English and Egyptian Arabic.",
            "Your task is to analyze the given text and output the likelihood (as a probability between 0 and 1) that it falls into each of the following categories:",
            "Toxicity, SevereToxicity, Obscene, Threat, Insult, IdentityAttack, SexualExplicit.",
            "Base your evaluation on linguistic meaning, intent, and contextual cues — not only on isolated words. Be sensitive to cultural nuances and slang used in Arabic and English code-switching.",
            "Your response must strictly be a valid JSON object conforming to the exact 'NsfwDetectionResponse' schema with the following fields:",
            GetOutputFormatSchema(),
            "Each probability value must be a numeric value between 0 and 1, inclusive, rounded to five decimal places.",
            "Do not include any explanations, comments, markdown formatting, or additional fields. Output only the JSON object.",
            "If uncertain, make a probabilistic estimation based on linguistic cues rather than abstaining."
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
            text
        ];
        return string.Join("\n", userPrompt);
    }
}
