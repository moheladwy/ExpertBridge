namespace ExpertBridge.Application.Settings;

/// <summary>
///     Configuration settings for AI-powered features in the ExpertBridge platform.
/// </summary>
/// <remarks>
///     This settings class configures endpoints and parameters for various AI services used throughout the application.
///     Currently includes post categorization service for automatic content classification.
///     **Configured in appsettings.json under "AI" section:**
///     <code>
/// {
///   "AI": {
///     "PostCategorizationUrl": "https://ai-service.example.com/categorize"
///   }
/// }
/// </code>
///     Future AI features may extend this class with additional service endpoints and configuration options.
/// </remarks>
public sealed class AiSettings
{
    /// <summary>
    ///     The configuration section name in appsettings.json.
    /// </summary>
    public const string Section = "AI";

    /// <summary>
    ///     Gets or sets the URL endpoint for the post categorization AI service.
    /// </summary>
    /// <remarks>
    ///     This endpoint is used to automatically classify and categorize user posts based on their content.
    ///     The service typically analyzes post text and returns category suggestions or tags.
    /// </remarks>
    public string PostCategorizationUrl { get; set; } = string.Empty;
}
