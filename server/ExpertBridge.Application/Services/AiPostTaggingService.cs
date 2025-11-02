// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Extensions.Resilience;
using ExpertBridge.GroqLibrary.Providers;
using Polly.Registry;
using ResiliencePipeline = Polly.ResiliencePipeline;

namespace ExpertBridge.Application.Services;

/// <summary>
///     AI-powered service for automatically generating relevant tags for posts by analyzing content using Groq LLM.
///     Supports both English and Arabic posts with bilingual tag generation.
/// </summary>
/// <remarks>
///     This service leverages Groq's large language model to perform intelligent content analysis and tag extraction.
///     It's a critical part of the platform's content discovery and categorization system.
///     Results are returned as PostCategorizerResponse containing detected language and categorized tags.
/// </remarks>
public sealed class AiPostTaggingService
{
    /// <summary>
    ///     The Groq LLM text provider for generating AI-powered text analysis and tag extraction.
    /// </summary>
    /// <remarks>
    ///     Configured to use a specific Groq model (e.g., llama3-70b-8192 or mixtral-8x7b-32768)
    ///     with parameters optimized for structured output generation.
    /// </remarks>
    private readonly GroqLlmTextProvider _groqLlmTextProvider;

    /// <summary>
    ///     JSON serialization options configured for robust parsing of LLM responses.
    /// </summary>
    /// <remarks>
    ///     Configured with:
    ///     - PropertyNameCaseInsensitive: Handle inconsistent casing from LLM
    ///     - AllowOutOfOrderMetadataProperties: Parse flexible JSON structures
    ///     - AllowTrailingCommas: Tolerate JSON formatting issues
    ///     These settings improve resilience against minor formatting variations in LLM output.
    /// </remarks>
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    ///     Polly resilience pipeline for handling transient failures and malformed responses.
    /// </summary>
    /// <remarks>
    ///     Uses the MalformedJsonModelResponse pipeline which includes:
    ///     - Retry policy for intermittent failures
    ///     - Circuit breaker for persistent issues
    ///     - Timeout policy for long-running requests
    ///     This ensures reliable tag generation even when LLM responses are occasionally malformed.
    /// </remarks>
    private readonly ResiliencePipeline _resiliencePipeline;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AiPostTaggingService" /> class with Groq LLM provider and resilience
    ///     configuration.
    /// </summary>
    /// <param name="groqLlmTextProvider">
    ///     The Groq LLM provider for text generation and analysis.
    /// </param>
    /// <param name="resilience">
    ///     The resilience pipeline provider for fault tolerance configuration.
    /// </param>
    /// <remarks>
    ///     Configures JSON deserialization options for lenient parsing of LLM responses.
    ///     Retrieves the MalformedJsonModelResponse pipeline for handling structured output issues.
    /// </remarks>
    public AiPostTaggingService(
        GroqLlmTextProvider groqLlmTextProvider,
        ResiliencePipelineProvider<string> resilience)
    {
        _groqLlmTextProvider = groqLlmTextProvider;
        _resiliencePipeline = resilience.GetPipeline(ResiliencePipelines.MalformedJsonModelResponse);
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, AllowOutOfOrderMetadataProperties = true, AllowTrailingCommas = true
        };
    }

    /// <summary>
    ///     Generates relevant tags for a post by analyzing its title, content, and existing tags using Groq LLM.
    /// </summary>
    /// <param name="title">The post-title to analyze for tag generation. Must not be null or empty.</param>
    /// <param name="content">The post-content/body to analyze for tag extraction. Must not be null or empty.</param>
    /// <param name="existingTags">
    ///     Collection of existing tags associated with the post. If provided, the LLM will translate them and generate
    ///     additional unique tags. If empty, the LLM will generate tags from scratch. Must not be null.
    /// </param>
    /// <returns>
    ///     A <see cref="PostCategorizerResponse" /> containing:
    ///     - Language: Detected language (Arabic, English, Mixed, Other)
    ///     - Tags: List of generated/translated tags with bilingual names and descriptions
    ///     Returns null if the LLM fails to generate valid output after all retry attempts.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when title or content is null or empty.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when existingTags is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the LLM response cannot be deserialized into the expected format.
    ///     This typically indicates a severe issue with the Groq API or model configuration.
    /// </exception>
    /// <exception cref="JsonException">
    ///     Thrown when JSON parsing fails after all retry attempts.
    ///     The resilience pipeline should handle most transient JSON issues.
    /// </exception>
    /// <remarks>
    ///     **Example Usage:**
    ///     <code>
    ///         var tags = await groqPostTaggingService.GeneratePostTagsAsync(
    ///             "How to optimize React performance",
    ///             "I'm looking for best practices to improve React app performance...",
    ///             new[] { "react", "javascript" }
    ///         );
    /// 
    ///         // Result might include:
    ///         // Language: English
    ///         // Tags: ["react", "javascript", "performance optimization", "web development", "frontend"]
    ///     </code>
    ///     **Performance Considerations:**
    ///     - LLM calls are relatively slow (2-5 seconds typical).
    ///     - Should be called asynchronously in background workers.
    ///     - Results should be cached to avoid redundant API calls.
    ///     - Rate limiting may apply based on llm provider tier.
    ///     The resilience pipeline ensures reliable operation even when the LLM occasionally
    ///     returns malformed JSON or experiences transient failures.
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
    private static string GetOutputFormatSchema()
    {
        return File.ReadAllText("LlmOutputFormat/PostCategorizationOutputFormat.json");
    }

    /// <summary>
    ///     Generates the master system prompt that defines the AI’s core behavior for text categorization tasks.
    ///     This prompt instructs the AI on language detection, tag translation, tag generation, and output formatting.
    ///     It enforces schema compliance, linguistic consistency, and structured output rules for bilingual (English–Egyptian
    ///     Arabic) posts.
    /// </summary>
    /// <returns>
    ///     A string containing the complete system prompt with all instructions and output schema definition.
    /// </returns>
    private static string GetSystemPrompt()
    {
        List<string> systemPrompt =
        [
            "You are an advanced text categorization AI specialized in analyzing posts written in English, Egyptian Arabic, or both.",
            "Your role is to detect the language of a post, extract or generate appropriate tags, and output the results as structured JSON following the defined schema.",

            "### Core Responsibilities:",
            "1. Detect the post’s language and classify it as one of: English, Arabic, Mixed, or Other.",
            "2. Extract existing tags if provided and translate them without altering their meaning.",
            "3. If no tags exist, generate new relevant tags based on the content.",
            "4. Each tag must include:",
            "   - 'english': the English tag name (lowercase, words separated by spaces)",
            "   - 'egyptian_arabic': the Egyptian Arabic translation (Arabic script, lowercase)",
            "   - 'description': a concise English explanation of the tag meaning or context.",

            "### Tag Generation Rules:",
            "5. Always generate between three (3) and six (6) tags in total.",
            "6. Tags must be relevant to the post’s core topic or problem only.",
            "7. Tags must be unique, non-repetitive, and semantically distinct.",
            "8. Tags must not contain numbers, punctuation, or special characters.",
            "9. Tags must not include language names such as 'english', 'arabic', etc.",
            "10. Tag names must be entirely lowercase, separated by a single space.",

            "### Output Format and Validation:",
            "11. The response must strictly conform to the following JSON schema:",
            "```json",
            GetOutputFormatSchema(),
            "```",
            "12. Do not include any markdown formatting, code fences, explanations, or commentary.",
            "13. Return only the raw JSON structure as the final output.",
            "14. Ensure the JSON is syntactically valid and matches all required field names.",
            "15. The output must represent the detected language and a list of tags as defined above.",

            "### Summary:",
            "You must always return a structured, well-formatted JSON object with the detected language and 3–6 bilingual tags, each containing an English name, Egyptian Arabic translation, and English description."
        ];

        return string.Join("\n", systemPrompt);
    }

    /// <summary>
    ///     Generates the user prompt that supplies the AI with the specific post details for categorization.
    ///     This includes the title, content, and any existing tags, which the model will use according to
    ///     the system prompt instructions.
    /// </summary>
    /// <param name="title">The title of the post.</param>
    /// <param name="content">The content or body text of the post.</param>
    /// <param name="existingTags">The list of existing tags, if any.</param>
    /// <returns>
    ///     A formatted string representing the input post data for the categorization task.
    /// </returns>
    private static string GetUserPrompt(string title, string content, IReadOnlyCollection<string> existingTags)
    {
        List<string> userPrompt =
        [
            "### Post Title:",
            title,
            "",
            "### Post Content:",
            content,
            "",
            "### Existing Tags:",
            existingTags.Count == 0 ? "[]" : $"[{string.Join(", ", existingTags)}]"
        ];

        return string.Join("\n", userPrompt);
    }
}
