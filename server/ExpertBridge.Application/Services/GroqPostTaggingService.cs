// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using ExpertBridge.Core.Responses;
using ExpertBridge.Extensions.Resilience;
using ExpertBridge.GroqLibrary.Providers;
using Polly.Registry;
using ResiliencePipeline = Polly.ResiliencePipeline;

namespace ExpertBridge.Application.Services;

/// <summary>
/// AI-powered service for automatically generating relevant tags for posts by analyzing content using Groq LLM.
/// Supports both English and Arabic posts with bilingual tag generation.
/// </summary>
/// <remarks>
/// This service leverages Groq's large language model to perform intelligent content analysis and tag extraction.
/// It's a critical component of the platform's content discovery and categorization system.
/// 
/// **Key Features:**
/// - Automatic tag generation from post title and content
/// - Bilingual support (English and Egyptian Arabic)
/// - Language detection (Arabic, English, Mixed, Other)
/// - Tag translation and normalization
/// - Existing tag enhancement and validation
/// - Structured JSON output with retry resilience
/// 
/// **Use Cases:**
/// - New post creation: Generate initial tags from content
/// - Post editing: Enhance existing tags with AI suggestions
/// - Content moderation: Validate tag relevance
/// - Search optimization: Improve discoverability through quality tags
/// 
/// **Groq Integration:**
/// Uses Groq API (llama3-70b or mixtral model) for:
/// 1. Content understanding and semantic analysis
/// 2. Language detection with high accuracy
/// 3. Relevant tag extraction (3-6 tags per post)
/// 4. Bilingual tag generation (English + Arabic)
/// 5. Category classification (Technology, Business, etc.)
/// 
/// **Tag Quality Rules:**
/// - Minimum 3 tags, maximum 6 tags per post
/// - Tags must be lowercase
/// - No special characters or numbers
/// - Space-separated multi-word tags allowed
/// - Tags must be relevant to post content only
/// - No duplicate or near-duplicate tags
/// 
/// **Resilience:**
/// Implements Polly resilience pipeline for handling:
/// - Malformed JSON responses from LLM
/// - Transient API failures
/// - Rate limiting
/// - Automatic retry with exponential backoff
/// 
/// Results are returned as PostCategorizerResponse containing detected language and categorized tags.
/// </remarks>
public sealed class GroqPostTaggingService
{
    /// <summary>
    /// The Groq LLM text provider for generating AI-powered text analysis and tag extraction.
    /// </summary>
    /// <remarks>
    /// Configured to use a specific Groq model (e.g., llama3-70b-8192 or mixtral-8x7b-32768)
    /// with parameters optimized for structured output generation.
    /// </remarks>
    private readonly GroqLlmTextProvider _groqLlmTextProvider;

    /// <summary>
    /// JSON serialization options configured for robust parsing of LLM responses.
    /// </summary>
    /// <remarks>
    /// Configured with:
    /// - PropertyNameCaseInsensitive: Handle inconsistent casing from LLM
    /// - AllowOutOfOrderMetadataProperties: Parse flexible JSON structures
    /// - AllowTrailingCommas: Tolerate JSON formatting issues
    /// 
    /// These settings improve resilience against minor formatting variations in LLM output.
    /// </remarks>
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    /// Polly resilience pipeline for handling transient failures and malformed responses.
    /// </summary>
    /// <remarks>
    /// Uses the MalformedJsonModelResponse pipeline which includes:
    /// - Retry policy for intermittent failures
    /// - Circuit breaker for persistent issues
    /// - Timeout policy for long-running requests
    /// 
    /// This ensures reliable tag generation even when LLM responses are occasionally malformed.
    /// </remarks>
    private readonly ResiliencePipeline _resiliencePipeline;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroqPostTaggingService"/> class with Groq LLM provider and resilience configuration.
    /// </summary>
    /// <param name="groqLlmTextProvider">
    /// The Groq LLM provider for text generation and analysis.
    /// </param>
    /// <param name="resilience">
    /// The resilience pipeline provider for fault tolerance configuration.
    /// </param>
    /// <remarks>
    /// Configures JSON deserialization options for lenient parsing of LLM responses.
    /// Retrieves the MalformedJsonModelResponse pipeline for handling structured output issues.
    /// </remarks>
    public GroqPostTaggingService(
        GroqLlmTextProvider groqLlmTextProvider,
        ResiliencePipelineProvider<string> resilience)
    {
        _groqLlmTextProvider = groqLlmTextProvider;
        _resiliencePipeline = resilience.GetPipeline(ResiliencePipelines.MalformedJsonModelResponse);
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowOutOfOrderMetadataProperties = true,
            AllowTrailingCommas = true
        };
    }

    /// <summary>
    /// Generates relevant tags for a post by analyzing its title, content, and existing tags using Groq LLM.
    /// </summary>
    /// <param name="title">The post title to analyze for tag generation. Must not be null or empty.</param>
    /// <param name="content">The post content/body to analyze for tag extraction. Must not be null or empty.</param>
    /// <param name="existingTags">
    /// Collection of existing tags associated with the post. If provided, the LLM will translate them and generate additional unique tags.
    /// If empty, the LLM will generate tags from scratch. Must not be null.
    /// </param>
    /// <returns>
    /// A <see cref="PostCategorizerResponse"/> containing:
    /// - Language: Detected language (Arabic, English, Mixed, Other)
    /// - Tags: List of generated/translated tags with bilingual names and descriptions
    /// Returns null if the LLM fails to generate valid output after all retry attempts.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when title or content is null or empty.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when existingTags is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the LLM response cannot be deserialized into the expected format.
    /// This typically indicates a severe issue with the Groq API or model configuration.
    /// </exception>
    /// <exception cref="JsonException">
    /// Thrown when JSON parsing fails after all retry attempts.
    /// The resilience pipeline should handle most transient JSON issues.
    /// </exception>
    /// <remarks>
    /// **Processing Flow:**
    /// 1. Validates input parameters (title, content, existingTags)
    /// 2. Constructs system prompt with tagging rules and guidelines
    /// 3. Constructs user prompt with post content and instructions
    /// 4. Sends prompts to Groq LLM via resilience pipeline
    /// 5. Deserializes JSON response into PostCategorizerResponse
    /// 6. Returns structured tag data
    /// 
    /// **System Prompt Instructions:**
    /// The LLM is instructed to:
    /// - Detect post language (Arabic/English/Mixed/Other)
    /// - Generate 3-6 relevant tags
    /// - Provide bilingual tag names (English + Egyptian Arabic)
    /// - Include tag descriptions
    /// - Translate existing tags without changing meaning
    /// - Generate additional unique tags beyond existing ones
    /// - Output in strict JSON format matching PostCategorizationOutputFormat.json schema
    /// 
    /// **User Prompt Format:**
    /// Includes:
    /// - Post title
    /// - Post content
    /// - Existing tags (if any)
    /// - Output format schema (Pydantic/JSON schema)
    /// 
    /// **Example Usage:**
    /// <code>
    /// var tags = await groqPostTaggingService.GeneratePostTagsAsync(
    ///     "How to optimize React performance",
    ///     "I'm looking for best practices to improve React app performance...",
    ///     new[] { "react", "javascript" }
    /// );
    /// 
    /// // Result might include:
    /// // Language: English
    /// // Tags: ["react", "javascript", "performance optimization", "web development", "frontend"]
    /// </code>
    /// 
    /// **Performance Considerations:**
    /// - LLM calls are relatively slow (2-5 seconds typical)
    /// - Should be called asynchronously in background workers
    /// - Results should be cached to avoid redundant API calls
    /// - Rate limiting may apply based on Groq API tier
    /// 
    /// The resilience pipeline ensures reliable operation even when the LLM occasionally
    /// returns malformed JSON or experiences transient failures.
    /// </remarks>
    public async Task<PostCategorizerResponse?> GeneratePostTagsAsync(
        string title,
        string content,
        IReadOnlyCollection<string> existingTags)
    {
        ArgumentException.ThrowIfNullOrEmpty(title, nameof(title));
        ArgumentException.ThrowIfNullOrEmpty(content, nameof(content));
        ArgumentNullException.ThrowIfNull(existingTags, nameof(existingTags));
        try
        {
            PostCategorizerResponse? result = null;

            // Use the resilience pipeline to handle transient errors and retries
            await _resiliencePipeline.ExecuteAsync(async ct =>
            {
                var systemPrompt = GetSystemPrompt();
                var userPrompt = GetUserPrompt(title, content, existingTags);
                var response = await _groqLlmTextProvider.GenerateAsync(systemPrompt, userPrompt);
                result = JsonSerializer.Deserialize<PostCategorizerResponse>(response, _jsonSerializerOptions)
                         ?? throw new InvalidOperationException(
                             "Failed to deserialize the categorizer response: null result");

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
    ///     Retrieves the schema definition for the output format used in the post-categorization process.
    ///     Reads the schema information from the "PostCategorizationOutputFormat.json" file.
    /// </summary>
    /// <returns>
    ///     A string containing the JSON schema definition for the expected output format.
    /// </returns>
    private static string GetOutputFormatSchema() =>
        File.ReadAllText("LlmOutputFormat/PostCategorizationOutputFormat.json");

    /// <summary>
    ///     Generates a predefined system prompt used to instruct the text categorization AI on how to process
    ///     and categorize posts. The prompt includes detailed guidelines for handling posts in English and
    ///     Arabic, ensuring proper language detection, tag translation, and generation of structured output.
    /// </summary>
    /// <returns>
    ///     A string containing the formatted system prompt with specific instructions for the categorization process.
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
    ///     Generates a formatted prompt for the categorization task by including the post's title, content,
    ///     existing tags, and specific instructions for processing. Combines these elements into a structured
    ///     string to be used as input for the language model.
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
}
