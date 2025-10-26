// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobPostings;

namespace ExpertBridge.Core.Entities.Media.JobPostingMedia;

/// <summary>
///     Represents a media attachment associated with a job posting.
/// </summary>
/// <remarks>
///     Job posting media files are stored in AWS S3 and can showcase project examples, requirements diagrams, or reference
///     materials.
/// </remarks>
public class JobPostingMedia : MediaObject
{
    // Foreign keys
    /// <summary>
    ///     Gets or sets the unique identifier of the job posting this media belongs to.
    /// </summary>
    public string JobPostingId { get; set; }

    // Navigation properties
    /// <summary>
    ///     Gets or sets the job posting this media is attached to.
    /// </summary>
    public JobPosting JobPosting { get; set; }
}
