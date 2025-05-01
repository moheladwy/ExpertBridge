using System.Text.Json;
using ExpertBridge.Api.Responses;
using ExpertBridge.GroqLibrary.Providers;

namespace ExpertBridge.Api.Services;

/// <summary>
/// Service responsible for categorizing posts by analyzing their content, title, and existing tags.
/// Utilizes a GroqLlmProvider to process and generate categorization results.
/// </summary>
public class PostCategorizationService
{
    /// <summary>
    /// An instance of <see cref="GroqLlmTextProvider"/> used to interact with the Groq Large Language Model (LLM)
    /// API for generating text-based categorizations in the context of post-analysis.
    /// </summary>
    private readonly GroqLlmTextProvider _groqLlmTextProvider;

    /// <summary>
    /// An instance of <see cref="JsonSerializerOptions"/> configured for deserializing JSON responses in a case-insensitive manner,
    /// ensuring robust parsing of post-categorization results from the GroqLlmTextProvider.
    /// </summary>
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    /// Service responsible for categorizing posts by analyzing their title, content, and existing tags.
    /// Relies on a GroqLlmTextProvider instance for interacting with a language model to generate
    /// categorization results.
    /// </summary>
    public PostCategorizationService(GroqLlmTextProvider groqLlmTextProvider)
    {
        _groqLlmTextProvider = groqLlmTextProvider;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Retrieves the schema definition for the output format used in the post-categorization process.
    /// Reads the schema information from the "PostCategorizationOutputFormat.json" file.
    /// </summary>
    /// <returns>
    /// A string containing the JSON schema definition for the expected output format.
    /// </returns>
    private static string GetOutputFormatSchema() => File.ReadAllText("PostCategorizationOutputFormat.json");

    /// <summary>
    /// Generates a predefined system prompt used to instruct the text categorization AI on how to process
    /// and categorize posts. The prompt includes detailed guidelines for handling posts in English and
    /// Arabic, ensuring proper language detection, tag translation, and generation of structured output.
    /// </summary>
    /// <returns>
    /// A string containing the formatted system prompt with specific instructions for the categorization process.
    /// </returns>
    private static string GetSystemPrompt()
    {
        List<string> systemPrompt =
        [
            "You are an advanced text categorization AI specializing in both English and Egyptian Arabic posts.",
            "Your task is to analyze a given post, detect its language (Arabic, English, Mixed, or Other), and categorize it with relevant tags.",
            "For each tag, you must provide both English and Egyptian Arabic names, along with a description.",
            "If the post already has tags, you must translate them and generate additional unique tags.",
            "If the post has tags, translate them without changing their meaning or the existing provided tags.",
            "If the post has no tags, generate new tags from scratch.",
            "Provide a structured output with at least three and at most six tags.",
            "You have to extract JSON details from text according to the Pydantic scheme.",
            "Do not generate any introductory or concluding text.",
            "Tags Names should be in English and Egyptian Arabic regardless of the post's language.",
            "Tags should be in lowercase, and separated by space ' '.",
            "Tags should be relevant to the post problem only.",
            "Tags should be unique and not repetitive.",
            "Tags should not contain numbers, or special characters.",
            "Tags should not contain the language name."
        ];
        return string.Join("\n", systemPrompt);
    }

    /// <summary>
    /// Generates a formatted prompt for the categorization task by including the post's title, content,
    /// existing tags, and specific instructions for processing. Combines these elements into a structured
    /// string to be used as input for the language model.
    /// </summary>
    /// <param name="title">The title of the post to be categorized.</param>
    /// <param name="content">The content of the post to be categorized.</param>
    /// <param name="existingTags">A collection of existing tags associated with the post.</param>
    /// <returns>A string representing the user prompt, formatted with detailed instructions and relevant data.</returns>
    private static string GetUserPrompt(string title, string content, IReadOnlyCollection<string> existingTags)
    {
        List<string> userPrompt =
        [
            "Categorize the following post based on its content and language.",
            "1. First, detect whether the post is in English, Arabic, Mixed, or Other.",
            "2. If the post has existing tags, translate them and generate additional unique tags.",
            "3. If the post has no tags, generate new tags from scratch.",
            "4. For each tag, provide both English and Egyptian Arabic names, along with a description.",
            "5. Tags should be in lowercase, and separated by space ' '.",
            "6. Tags should not contain numbers, or special characters.",
            "7. Tags should be unique and not repetitive.",
            "### Post Title:",
            "```",
            title,
            "```",
            "### Post Content:",
            "```",
            content,
            "```",
            "### Existing Tags:",
            "```",
            existingTags.Count == 0 ? "[]" : $"[{string.Join(", ", existingTags.ToList())}]",
            "```",
            "## Pydantic Details:",
            "```json",
            GetOutputFormatSchema(),
            "```",
            "Return only the raw JSON without any markdown code block formatting."
        ];

        return string.Join("\n", userPrompt);
    }


    /// <summary>
    /// Asynchronously categorizes a post by analyzing its title, content, and existing tags.
    /// Uses the GroqLlmTextProvider to generate categorization results and processes the
    /// response to return a PostCategorizerResponse object, which contains language information
    /// and categorized tags.
    /// </summary>
    /// <param name="title">The title of the post to be categorized. Must not be null or empty.</param>
    /// <param name="content">The content of the post to be categorized. Must not be null or empty.</param>
    /// <param name="existingTags">A collection of tags associated with the post. Must not be null.</param>
    /// <returns>A <see cref="PostCategorizerResponse"/> object containing the language and categorized tags of the post.</returns>
    /// <exception cref="ArgumentException">Thrown if the title or content is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the existingTags collection is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the deserialization of the categorizer response fails or returns a null result.
    /// </exception>
    /// <exception cref="JsonException">Thrown if an error occurs while parsing the categorizer response.</exception>
    public async Task<PostCategorizerResponse> CategorizePostAsync(string title, string content,
        IReadOnlyCollection<string> existingTags)
    {
        ArgumentException.ThrowIfNullOrEmpty(title, nameof(title));
        ArgumentException.ThrowIfNullOrEmpty(content, nameof(content));
        ArgumentNullException.ThrowIfNull(existingTags, nameof(existingTags));
        try
        {
            var systemPrompt = GetSystemPrompt();
            var userPrompt = GetUserPrompt(title, content, existingTags);
            var response = await _groqLlmTextProvider.GenerateAsync(systemPrompt, userPrompt);
            var result = JsonSerializer.Deserialize<PostCategorizerResponse>(response, _jsonSerializerOptions)
                         ?? throw new InvalidOperationException("Failed to deserialize the categorizer response: null result");
            return result;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse the categorizer response: {ex.Message}", ex);
        }
    }
}
