// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.Areas;

/// <summary>
/// Represents a geographical area associated with a user profile or job posting.
/// </summary>
/// <remarks>
/// Areas are used for location-based job matching and filtering, consisting of a governorate and region.
/// </remarks>
public class Area
{
    /// <summary>
    /// Gets or sets the unique identifier for the area.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the unique identifier of the associated profile.
    /// </summary>
    public string ProfileId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the governorate (administrative division).
    /// </summary>
    public Governorates Governorate { get; set; }

    /// <summary>
    /// Gets or sets the region name within the governorate.
    /// </summary>
    public string Region { get; set; } = null!;

    // Navigation properties
    /// <summary>
    /// Gets or sets the profile associated with this area.
    /// </summary>
    public Profile Profile { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of job postings in this area.
    /// </summary>
    public ICollection<JobPosting> JobPostings { get; set; } = [];
}
