// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.Tags;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.JobPostingTags;

public class JobPostingTag
{
    public string JobPostingId { get; set; }
    public string TagId { get; set; }

    // Navigation properties
    public JobPosting JobPosting { get; set; }
    public Tag Tag { get; set; }
}
