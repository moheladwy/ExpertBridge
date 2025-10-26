// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.JobApplications;
using ExpertBridge.Core.Entities.JobPostingsVotes;
using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.ManyToManyRelationships.JobPostingTags;
using ExpertBridge.Core.Entities.Media.JobPostingMedia;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;
using Pgvector;

namespace ExpertBridge.Core.Entities.JobPostings;

/// <summary>
///     Represents a job posting created by a user looking to hire experts.
/// </summary>
/// <remarks>
///     Job postings support AI-powered features including automatic tagging, content moderation, semantic search through
///     vector embeddings,
///     and intelligent matching with qualified professionals. They enable the hiring workflow including applications,
///     offers, and contracts.
/// </remarks>
public class JobPosting : BaseModel, ISoftDeletable, IRecommendableContent, ISafeContent
{
    /// <summary>
    ///     Gets or sets the budget allocated for the job in the platform's currency.
    /// </summary>
    public decimal Budget { get; set; }

    /// <summary>
    ///     Gets or sets the geographic area or location for the job.
    /// </summary>
    public string Area { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the author last updated the job posting.
    /// </summary>
    /// <remarks>
    ///     This is distinct from <see cref="BaseModel.LastModified" /> which is automatically set by the system.
    /// </remarks>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the profile of the user who created the job posting.
    /// </summary>
    public Profile Author { get; set; }

    /// <summary>
    ///     Gets or sets the collection of media attachments associated with the job posting.
    /// </summary>
    public ICollection<JobPostingMedia> Medias { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of tags automatically assigned to the job posting.
    /// </summary>
    public ICollection<JobPostingTag> JobPostingTags { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of comments made on the job posting.
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of votes (upvotes/downvotes) on the job posting.
    /// </summary>
    public ICollection<JobPostingVote> Votes { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of applications submitted for this job posting.
    /// </summary>
    public ICollection<JobApplication> JobApplications { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of active jobs/contracts resulting from this posting.
    /// </summary>
    public ICollection<Job> Jobs { get; set; } = [];

    /// <summary>
    ///     Gets or sets the title of the job posting.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     Gets or sets the detailed description and requirements of the job.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the job posting author.
    /// </summary>
    public string AuthorId { get; set; }

    /// <summary>
    ///     Gets or sets the detected language of the job posting content.
    /// </summary>
    /// <remarks>
    ///     This property is populated by AI language detection services during processing.
    /// </remarks>
    public string? Language { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the job posting has been automatically tagged.
    /// </summary>
    public bool IsTagged { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the job posting has been processed through the AI pipeline.
    /// </summary>
    /// <remarks>
    ///     Processing includes language detection, tagging, embedding generation, and content moderation.
    /// </remarks>
    public bool IsProcessed { get; set; }

    /// <summary>
    ///     Gets or sets the vector embedding representation of the job posting for semantic similarity search.
    /// </summary>
    /// <remarks>
    ///     The embedding enables intelligent matching with qualified candidates and job recommendations.
    /// </remarks>
    public Vector? Embedding { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the job posting content has been verified as safe and appropriate.
    /// </summary>
    public bool IsSafeContent { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the job posting is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the job posting was marked as deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
