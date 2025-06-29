// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.ManyToManyRelationships.JobPostingTags;
using ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags;
using ExpertBridge.Core.Entities.ManyToManyRelationships.UserInterests;

namespace ExpertBridge.Core.Entities.Tags;

public class Tag : BaseModel
{
    public string EnglishName { get; set; }
    public string? ArabicName { get; set; }
    public string Description { get; set; }

    public ICollection<UserInterest> ProfileTags { get; set; } = [];
    public ICollection<PostTag> PostTags { get; set; } = [];
    public ICollection<JobPostingTag> JobPostingTags { get; set; } = [];
}
