// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.ModerationReports;

/// <summary>
///     Represents a moderation report for potentially inappropriate content on the platform.
/// </summary>
/// <remarks>
///     Moderation reports are generated through AI-powered content analysis that detects various types of inappropriate
///     content, including
///     toxicity, obscenity, threats, insults, identity attacks, and sexual content. Reports can be manually reviewed
///     and resolved by moderators.
/// </remarks>
public sealed class ModerationReport : BaseModel, ISoftDeletable
{
    /// <summary>
    ///     Gets or sets the type of content being reported.
    /// </summary>
    public ContentTypes ContentType { get; set; }

    /// <summary>
    ///     Gets or sets who reported the content.
    /// </summary>
    public ReportedBy ReportedBy { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the content being reported.
    /// </summary>
    public string ContentId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the content author.
    /// </summary>
    public string AuthorId { get; set; }

    /// <summary>
    ///     Gets or sets the reason or description for the moderation report.
    /// </summary>
    public string Reason { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the report has been reviewed and resolved by a moderator.
    /// </summary>
    public bool IsResolved { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the AI analysis determined the content to be negative or inappropriate.
    /// </summary>
    public bool IsNegative { get; set; }

    // AI Analysis Results
    /// <summary>
    ///     Gets or sets the toxicity score from AI content analysis (0.0 to 1.0).
    /// </summary>
    /// <remarks>
    ///     Higher values indicate more toxic content including rude, disrespectful, or unreasonable comments.
    /// </remarks>
    public double Toxicity { get; set; }

    /// <summary>
    ///     Gets or sets the severe toxicity score from AI content analysis (0.0 to 1.0).
    /// </summary>
    /// <remarks>
    ///     Higher values indicate very hateful, aggressive, or disrespectful content that is very likely to make users leave a
    ///     discussion.
    /// </remarks>
    public double SevereToxicity { get; set; }

    /// <summary>
    ///     Gets or sets the obscenity score from AI content analysis (0.0 to 1.0).
    /// </summary>
    /// <remarks>
    ///     Higher values indicate content containing swear words, curse words, or other obscene language.
    /// </remarks>
    public double Obscene { get; set; }

    /// <summary>
    ///     Gets or sets the threat score from AI content analysis (0.0 to 1.0).
    /// </summary>
    /// <remarks>
    ///     Higher values indicate content containing threats of violence or harm to individuals or groups.
    /// </remarks>
    public double Threat { get; set; }

    /// <summary>
    ///     Gets or sets the insult score from AI content analysis (0.0 to 1.0).
    /// </summary>
    /// <remarks>
    ///     Higher values indicate content containing insulting, inflammatory, or negative comments towards a person or group.
    /// </remarks>
    public double Insult { get; set; }

    /// <summary>
    ///     Gets or sets the identity attack score from AI content analysis (0.0 to 1.0).
    /// </summary>
    /// <remarks>
    ///     Higher values indicate negative or hateful content targeting someone's identity including race, religion, gender,
    ///     disability, etc.
    /// </remarks>
    public double IdentityAttack { get; set; }

    /// <summary>
    ///     Gets or sets the sexually explicit content score from AI content analysis (0.0 to 1.0).
    /// </summary>
    /// <remarks>
    ///     Higher values indicate content containing references to sexual acts, body parts, or other sexual content.
    /// </remarks>
    public double SexualExplicit { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this moderation report has been soft-deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when this moderation report was soft-deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
