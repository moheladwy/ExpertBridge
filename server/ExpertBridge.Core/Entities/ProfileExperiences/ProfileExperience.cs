// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Media.ProfileExperienceMedia;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.ProfileExperiences;

/// <summary>
/// Represents a work experience entry on a user's professional profile.
/// </summary>
/// <remarks>
/// Profile experiences build a user's employment history and help demonstrate their qualifications to potential hirers.
/// </remarks>
public class ProfileExperience : BaseModel, ISoftDeletable
{
    /// <summary>
    /// Gets or sets the unique identifier of the profile this experience belongs to.
    /// </summary>
    public string ProfileId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the job title held during this experience.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets the description of responsibilities and achievements.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Gets or sets the company or organization name.
    /// </summary>
    public string Company { get; set; } = null!;

    /// <summary>
    /// Gets or sets the location where the work was performed.
    /// </summary>
    public string Location { get; set; } = null!;

    /// <summary>
    /// Gets or sets the start date of this employment.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of this employment, or null if currently employed.
    /// </summary>
    public DateTime? EndDate { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the profile this experience belongs to.
    /// </summary>
    public Profile Profile { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of media attachments for this experience entry.
    /// </summary>
    public ICollection<ProfileExperienceMedia> Medias { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the experience is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the experience was deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
