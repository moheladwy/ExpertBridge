// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.Jobs;

namespace ExpertBridge.Api.Core.Entities.JobStatuses;

public class JobStatus
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public JobStatusEnum Status { get; set; }

    // Navigation properties
    public ICollection<Job> Jobs { get; set; } = [];
}
