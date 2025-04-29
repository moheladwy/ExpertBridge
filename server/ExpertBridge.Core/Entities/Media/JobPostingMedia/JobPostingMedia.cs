// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.JobPostings;

namespace ExpertBridge.Api.Core.Entities.Media.JobPostingMedia;

public class JobPostingMedia : MediaObject
{
    // Foreign keys
    public string JobPostingId { get; set; }

    // Navigation properties
    public JobPosting JobPosting { get; set; }
}
