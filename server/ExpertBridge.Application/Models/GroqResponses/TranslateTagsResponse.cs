using ExpertBridge.Contract.Responses;

namespace ExpertBridge.Application.Models.GroqResponses;

/// <summary>
///     Response model from Groq LLM for tag translation and normalization operations.
/// </summary>
/// <remarks>
///     This DTO captures the structured response from Groq API when translating or normalizing tags
///     extracted from user-generated content (posts, profiles, job descriptions).
///     <code>
///           Input Tags: ["JavaScript", "JS", "React.js", "frontend"]
///           Groq Response:
///           {
///             "Tags": [
///               { "Name": "JavaScript", "Category": "Programming Language" },
///               { "Name": "React", "Category": "Framework" },
///               { "Name": "Frontend Development", "Category": "Domain" }
///             ]
///           }
///     </code>
///     The response provides structured tag data (CategorizerTagResponse) which includes
///     normalized names, categories, and potentially confidence scores.
///     This helps maintain tag consistency across the platform and improves content discoverability.
/// </remarks>
public class TranslateTagsResponse
{
    /// <summary>
    ///     Gets or sets the list of translated and normalized tags returned by Groq LLM.
    /// </summary>
    /// <remarks>
    ///     Each CategorizerTagResponse contains:
    ///     - Name: Normalized tag name (canonical form)
    ///     - Category: Tag classification (e.g., "Technology", "Skill", "Industry")
    ///     - Additional metadata as defined in CategorizerTagResponse
    ///     Tags may be filtered, merged, or reordered compared to the input based on LLM analysis.
    /// </remarks>
    public List<CategorizerTagResponse> Tags { get; set; }
}
