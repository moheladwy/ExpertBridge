// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.ManyToManyRelationships.JobPostingTags;
using ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags;
using ExpertBridge.Core.Entities.ManyToManyRelationships.UserInterests;

namespace ExpertBridge.Core.Entities.Tags;

/// <summary>
///     Represents a tag used for categorizing and organizing content in the ExpertBridge platform.
/// </summary>
/// <remarks>
///     Tags are automatically assigned to posts and job postings through AI-powered categorization.
///     They support multilingual names (English and Arabic) and are used for content discovery and user interest matching.
/// </remarks>
public class Tag : BaseModel
{
    /// <summary>
    ///     Gets or sets the English name of the tag.
    /// </summary>
    public string EnglishName { get; set; }

    /// <summary>
    ///     Gets or sets the Arabic name of the tag.
    /// </summary>
    /// <remarks>
    ///     This property is optional and enables Arabic-speaking users to browse and filter content effectively.
    /// </remarks>
    public string? ArabicName { get; set; }

    /// <summary>
    ///     Gets or sets a description explaining what the tag represents.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets the collection of user interest relationships with this tag.
    /// </summary>
    /// <remarks>
    ///     This relationship tracks which users are interested in this tag's topic area.
    /// </remarks>
    public ICollection<UserInterest> ProfileTags { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of posts associated with this tag.
    /// </summary>
    public ICollection<PostTag> PostTags { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of job postings associated with this tag.
    /// </summary>
    public ICollection<JobPostingTag> JobPostingTags { get; set; } = [];
}
