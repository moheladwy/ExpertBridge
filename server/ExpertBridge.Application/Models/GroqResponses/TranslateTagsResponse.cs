using ExpertBridge.Core.Responses;

namespace ExpertBridge.Application.Models.GroqResponses;

/// <summary>
/// Response model from Groq LLM for tag translation and normalization operations.
/// </summary>
/// <remarks>
/// This DTO captures the structured response from Groq API when translating or normalizing tags
/// extracted from user-generated content (posts, profiles, job descriptions).
///
/// **Use Cases:**
/// - Normalizing user-entered tags to standard vocabulary
/// - Translating foreign language tags to English
/// - Converting synonyms to canonical tag names
/// - Validating tag relevance and accuracy
///
/// **Groq Integration:**
/// The AiTagProcessorService sends raw tags to Groq LLM with a prompt requesting:
/// 1. Translation to English (if needed)
/// 2. Normalization to standard terms
/// 3. Validation of relevance
/// 4. Removal of duplicates and near-duplicates
///
/// **Example Flow:**
/// <code>
/// Input Tags: ["JavaScript", "JS", "React.js", "frontend"]
/// Groq Response:
/// {
///   "Tags": [
///     { "Name": "JavaScript", "Category": "Programming Language" },
///     { "Name": "React", "Category": "Framework" },
///     { "Name": "Frontend Development", "Category": "Domain" }
///   ]
/// }
/// </code>
///
/// The response provides structured tag data (CategorizerTagResponse) which includes
/// normalized names, categories, and potentially confidence scores.
///
/// This helps maintain tag consistency across the platform and improves content discoverability.
/// </remarks>
public class TranslateTagsResponse
{
    /// <summary>
    /// Gets or sets the list of translated and normalized tags returned by Groq LLM.
    /// </summary>
    /// <remarks>
    /// Each CategorizerTagResponse contains:
    /// - Name: Normalized tag name (canonical form)
    /// - Category: Tag classification (e.g., "Technology", "Skill", "Industry")
    /// - Additional metadata as defined in CategorizerTagResponse
    ///
    /// Tags may be filtered, merged, or reordered compared to the input based on LLM analysis.
    /// </remarks>
    public List<CategorizerTagResponse> Tags { get; set; }
}
