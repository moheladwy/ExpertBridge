using System.Text.Json;
using ExpertBridge.Application.Models.GroqResponses;
using ExpertBridge.Extensions.Resilience;
using Groq.Core.Models;
using Groq.Core.Providers;
using Polly.Registry;
using ResiliencePipeline = Polly.ResiliencePipeline;

namespace ExpertBridge.Worker.Services;

/// <summary>
///     A service for processing and analyzing tags or textual categorizations using
///     the Groq Large Language Model (LLM) API. It provides functionality for generating,
///     processing, and handling text-based categorizations leveraging the integration
///     with <see cref="LlmTextProvider" /> for efficient communication with the Groq LLM.
///     The service also supports flexible and robust JSON parsing with case-insensitive
///     property deserialization settings.
/// </summary>
public class AiTagProcessorService
{
    /// <summary>
    ///     An instance of <see cref="LlmTextProvider" /> used to interact with the Groq Large Language Model (LLM)
    ///     API for generating text-based categorizations in the context of post-analysis.
    /// </summary>
    private readonly LlmTextProvider _llmTextProvider;

    /// <summary>
    ///     An instance of <see cref="JsonSerializerOptions" /> configured for deserializing JSON responses in a
    ///     case-insensitive manner,
    ///     ensuring robust parsing of post-categorization results from the LlmTextProvider.
    /// </summary>
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    ///     A pipeline instance implemented via <see cref="Polly" /> to handle resilient
    ///     execution and retry policies for operations encountering scenarios such as
    ///     malformed JSON responses. This supports maintaining robust and fault-tolerant
    ///     behavior when communicating with external APIs or processing data.
    /// </summary>
    private readonly ResiliencePipeline _resiliencePipeline;

    /// <summary>
    ///     Provides functionality for processing and analyzing tags or text categorizations
    ///     using an integrated Groq Large Language Model (LLM) provider. This service interacts
    ///     with the Groq LLM API for generating text-based categorizations and processes
    ///     the results with case-insensitive JSON deserialization for robust and flexible parsing.
    /// </summary>
    public AiTagProcessorService(
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
    ///     Translates and generates a collection of categorizer tags using the Groq Large Language Model (LLM) API.
    ///     This method processes the existing tags, communicates with the Groq LLM, and deserializes
    ///     the categorization response into a list of categorizer tag objects.
    /// </summary>
    /// <param name="existingTags">A collection of existing tags to be translated and processed.</param>
    /// <returns>A collection of categorizer tags containing translated tag details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="existingTags" /> parameter is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the response from the Groq LLM API cannot be deserialized
    ///     or when the deserialization result is null.
    /// </exception>
    /// <exception cref="JsonException">Thrown when a JSON parsing error occurs during deserialization of the response.</exception>
    public async Task<TranslateTagsResponse?> TranslateTagsAsync(IReadOnlyCollection<string> existingTags)
    {
        ArgumentNullException.ThrowIfNull(existingTags);
        try
        {
            TranslateTagsResponse? result = null;
            await _resiliencePipeline.ExecuteAsync(async ct =>
            {
                var systemPrompt = GetSystemPrompt();
                var userPrompt = GetUserPrompt(existingTags);
                var structureOutput = await GetOutputFormatSchema();
                var response = await _llmTextProvider.GenerateAsync(
                    systemPrompt: systemPrompt,
                    userPrompt: userPrompt,
                    structureOutputJsonFormat: structureOutput,
                    model: ChatModels.OPENAI_GPT_OSS_120B.Id);
                result = JsonSerializer.Deserialize<TranslateTagsResponse>(response, _jsonSerializerOptions)
                         ?? throw new InvalidOperationException(
                             "Failed to deserialize the tag processing response: null result");
                return ValueTask.CompletedTask;
            });
            return result;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse the categorizer response: {ex.Message}", ex);
        }
    }

    /// <summary>
    ///     Retrieves the content of the output format schema file used for tag translation.
    /// </summary>
    /// <returns>
    ///     A string containing the JSON schema definition for translating tag responses.
    /// </returns>
    private static async Task<string> GetOutputFormatSchema()
    {
        return await File.ReadAllTextAsync("LlmOutputFormat/TranslateTagResponseOutputFormat.json");
    }

    /// <summary>
    ///     Generates and returns the system prompt used for tag translation and description generation.
    ///     This prompt instructs the model to accurately translate tags between English and Egyptian Arabic,
    ///     ensuring semantic consistency, linguistic correctness, and standardized output formatting.
    /// </summary>
    /// <returns>
    ///     A string containing the complete system prompt, formatted with detailed multilingual translation
    ///     and output specifications compliant with the defined schema.
    /// </returns>
    private static string GetSystemPrompt()
    {
        List<string> systemPrompt =
        [
            "You are an advanced tag translation and description generation assistant.",
            "Your primary objective is to translate tags between English and Egyptian Arabic, ensuring cultural and linguistic accuracy.",
            "For each provided tag, perform the following steps precisely:",

            "1. Generate two versions of the tag:",
            "   - 'english': the English translation of the tag, written in lowercase letters separated by spaces.",
            "   - 'egyptian_arabic': the Egyptian Arabic translation of the tag, also written in lowercase using Arabic script.",
            "2. Provide a concise, clear English description explaining the meaning or context of the tag.",
            "3. If the original tag is already in English, only translate it into Egyptian Arabic.",
            "   If it is already in Egyptian Arabic, only translate it into English.",
            "4. If the language is ambiguous, infer the most probable translation from context.",
            "5. Ensure that all tags are unique, natural, and non-repetitive.",

            "Formatting and Validation Rules:",
            "6. Tags must not contain numbers, punctuation, or special characters.",
            "7. Tags must not include the name of any language (e.g., 'english', 'arabic', etc.).",
            "8. All tag names must be lowercase, separated by single spaces.",
            "9. Do not include explanations, markdown, or commentary outside the JSON output.",
            "10. Output only the final JSON structure — no preface, code blocks, or additional text.",

            "Output Format Requirements:",
            "Your response must strictly conform to the given JSON schema representation.",
            "Each field must appear exactly as defined, without omissions or renaming.",
            "The output must be raw JSON (no markdown formatting, no code fences).",
            "If multiple tags are given, return a JSON array following the same schema for each item.",
            "Maintain absolute formatting integrity — the output must be valid JSON."
        ];

        return string.Join("\n", systemPrompt);
    }


    /// <summary>
    ///     Generates a user prompt string for translating and describing tags by providing detailed
    ///     instructions for translations, descriptions, and formatting.
    /// </summary>
    /// <param name="existingTags">The collection of existing tags that need to be translated and described.</param>
    /// <returns>
    ///     A string containing the user prompt with detailed instructions and formatting requirements for tag processing.
    /// </returns>
    private static string GetUserPrompt(IReadOnlyCollection<string> existingTags)
    {
        List<string> userPrompt =
        [
            "Translate and provide descriptions for the following tags:",
            $"[{string.Join(", ", existingTags.ToList())}]"
        ];
        return string.Join("\n", userPrompt);
    }
}
