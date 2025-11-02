namespace ExpertBridge.Application.Settings;

/// <summary>
///     Configuration settings for content moderation sensitivity thresholds used by AI-powered inappropriate language
///     detection.
/// </summary>
/// <remarks>
///     This settings class defines the acceptable threshold values for various categories of inappropriate content.
///     Used by NsfwContentDetectionService to determine when content should be flagged or rejected.
///     **Configured in appsettings.json under "NsfwThresholds" section: **
///     <code>
///         {
///           "NsfwThresholds": {
///             "Toxicity": 0.7,
///             "SevereToxicity": 0.5,
///             "Obscene": 0.7,
///             "Threat": 0.6,
///             "Insult": 0.7,
///             "IdentityAttack": 0.6,
///             "SexualExplicit": 0.7
///           }
///         }
///     </code>
///     Groq LLM analyzes text and returns confidence scores (0.0-1.0) for each category.
///     Content is flagged if ANY category score exceeds its configured threshold.
/// </remarks>
public sealed class NsfwThresholds
{
    /// <summary>
    ///     The configuration section name in appsettings.json.
    /// </summary>
    public const string Section = "NsfwThresholds";

    /// <summary>
    ///     Gets or sets the threshold for general toxic, rude, or disrespectful language.
    /// </summary>
    /// <remarks>
    ///     Recommended: 0.6-0.7 for balanced moderation.
    ///     Content scoring above this value is considered toxic.
    /// </remarks>
    public double Toxicity { get; set; }

    /// <summary>
    ///     Gets or sets the threshold for severely hateful, aggressive, or extremely disrespectful language.
    /// </summary>
    /// <remarks>
    ///     Recommended: 0.4-0.6 (stricter than general toxicity).
    ///     Content scoring above this value is considered severely toxic.
    /// </remarks>
    public double SevereToxicity { get; set; }

    /// <summary>
    ///     Gets or sets the threshold for vulgar, profane, or sexually suggestive language.
    /// </summary>
    /// <remarks>
    ///     Recommended: 0.6-0.7 for balanced moderation.
    ///     Content scoring above this value is considered obscene.
    /// </remarks>
    public double Obscene { get; set; }

    /// <summary>
    ///     Gets or sets the threshold for threatening language suggesting violence or harm.
    /// </summary>
    /// <remarks>
    ///     Recommended: 0.5-0.6 (stricter due to safety concerns).
    ///     Content scoring above this value is considered a threat.
    /// </remarks>
    public double Threat { get; set; }

    /// <summary>
    ///     Gets or sets the threshold for insulting or derogatory personal attacks.
    /// </summary>
    /// <remarks>
    ///     Recommended: 0.6-0.7 for balanced moderation.
    ///     Content scoring above this value is considered an insult.
    /// </remarks>
    public double Insult { get; set; }

    /// <summary>
    ///     Gets or sets the threshold for attacks based on identity characteristics (race, religion, gender, etc.).
    /// </summary>
    /// <remarks>
    ///     Recommended: 0.5-0.6 (stricter due to discrimination concerns).
    ///     Content scoring above this value is considered an identity attack.
    /// </remarks>
    public double IdentityAttack { get; set; }

    /// <summary>
    ///     Gets or sets the threshold for explicitly sexual or pornographic content.
    /// </summary>
    /// <remarks>
    ///     Recommended: 0.6-0.7 for professional platforms.
    ///     Content scoring above this value is considered sexually explicit.
    /// </remarks>
    public double SexualExplicit { get; set; }
}
