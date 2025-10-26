// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.JobPostingsVotes;

/// <summary>
///     Represents a user's vote (upvote or downvote) on a job posting.
/// </summary>
/// <remarks>
///     Job posting votes help surface quality job opportunities and provide feedback to hirers.
///     Users can only cast one vote per job posting.
/// </remarks>
public class JobPostingVote : BaseModel
{
    /// <summary>
    ///     Gets or sets a value indicating whether this is an upvote (true) or downvote (false).
    /// </summary>
    public bool IsUpvote { get; set; }

    // Foreign keys
    /// <summary>
    ///     Gets or sets the unique identifier of the profile that cast the vote.
    /// </summary>
    public string ProfileId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the job posting being voted on.
    /// </summary>
    public string JobPostingId { get; set; }

    // Navigation properties
    /// <summary>
    ///     Gets or sets the profile that cast this vote.
    /// </summary>
    public Profile Profile { get; set; }

    /// <summary>
    ///     Gets or sets the job posting that received this vote.
    /// </summary>
    public JobPosting JobPosting { get; set; }
}
