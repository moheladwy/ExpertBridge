// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.Areas;

public class Area
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProfileId { get; set; }
    public Governorates Governorate { get; set; }
    public string Region { get; set; }

    // Navigation properties
    public Profile Profile { get; set; }
    public ICollection<JobPosting> JobPostings { get; set; } = [];
}
