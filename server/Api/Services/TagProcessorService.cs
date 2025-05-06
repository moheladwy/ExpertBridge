using System.Text.Json;
using Core.Responses;
using GroqLibrary.Providers;

namespace Api.Services;

/// <summary>
///     A service for processing and analyzing tags or textual categorizations using
///     the Groq Large Language Model (LLM) API. It provides functionality for generating,
///     processing, and handling text-based categorizations leveraging the integration
///     with <see cref="GroqLlmTextProvider" /> for efficient communication with the Groq LLM.
///     The service also supports flexible and robust JSON parsing with case-insensitive
///     property deserialization settings.
/// </summary>
public class TagProcessorService
{
    /// <summary>
    ///     An instance of <see cref="GroqLlmTextProvider" /> used to interact with the Groq Large Language Model (LLM)
    ///     API for generating text-based categorizations in the context of post-analysis.
    /// </summary>
    private readonly GroqLlmTextProvider _groqLlmTextProvider;

    /// <summary>
    ///     An instance of <see cref="JsonSerializerOptions" /> configured for deserializing JSON responses in a
    ///     case-insensitive manner,
    ///     ensuring robust parsing of post-categorization results from the GroqLlmTextProvider.
    /// </summary>
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    ///     Provides functionality for processing and analyzing tags or text categorizations
    ///     using an integrated Groq Large Language Model (LLM) provider. This service interacts
    ///     with the Groq LLM API for generating text-based categorizations and processes
    ///     the results with case-insensitive JSON deserialization for robust and flexible parsing.
    /// </summary>
    public TagProcessorService(GroqLlmTextProvider groqLlmTextProvider)
    {
        _groqLlmTextProvider = groqLlmTextProvider;
        _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
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
    public async Task<IEnumerable<CategorizerTag>> TranslateTagsAsync(IReadOnlyCollection<string> existingTags)
    {
        ArgumentNullException.ThrowIfNull(existingTags, nameof(existingTags));
        try
        {
            var systemPrompt = GetSystemPrompt();
            var userPrompt = GetUserPrompt(existingTags);
            var response = await _groqLlmTextProvider.GenerateAsync(systemPrompt, userPrompt);
            var result = JsonSerializer.Deserialize<List<CategorizerTag>>(response, _jsonSerializerOptions)
                         ?? throw new InvalidOperationException(
                             "Failed to deserialize the categorizer response: null result");
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
    private static string GetOutputFormatSchema() => File.ReadAllText("LlmOutputFormat/TranslateTagResponseOutputFormat.json");

    /// <summary>
    ///     Generates and returns the system prompt used for tag translation tasks.
    ///     The prompt provides instructions for translating tags and generating descriptions
    ///     in accordance with specific rules, ensuring consistent and accurate output.
    /// </summary>
    /// <returns>
    ///     A string representing the system prompt, formatted with detailed instructions
    ///     for tag translation and description generation.
    /// </returns>
    private static string GetSystemPrompt()
    {
        List<string> systemPrompt =
        [
            "You are a tag translation specialist. Translate tags between English and Egyptian Arabic and provide detailed descriptions in English only.",
            "Your task is to translate the given tags and provide their descriptions.",
            "1. Provide both English and Egyptian Arabic translations for each tag.",
            "2. Provide a concise description in English.",
            "3. All names should be lowercase and separated by spaces.",
            "4. If a tag is already in English, provide its Egyptian Arabic translation and vice versa.",
            "5. If the language is unclear, try to determine the most likely translation.",
            "6. Tags should be unique and not repetitive.",
            "7. Tags should not contain numbers, or special characters.",
            "8. Tags should not contain the language name.",
            "9. Do not generate any introductory or concluding text."
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
            $"[{string.Join(", ", existingTags.ToList())}]",
            "For each tag:",
            "1. Provide both English and Egyptian Arabic translations",
            "2. Provide a concise description in English",
            "3. All names should be lowercase and separated by spaces",
            "4. If a tag is already in English, provide its Egyptian Arabic translation and vice versa",
            "5. If the language is unclear, try to determine the most likely translation",
            "Return only the raw JSON in the format described below without any markdown code block formatting.",
            "### Pydantic Details:",
            "```json",
            GetOutputFormatSchema(),
            "```",
            "Return only the raw JSON without any markdown code block formatting."
        ];

        return string.Join("\n", userPrompt);
    }
}
