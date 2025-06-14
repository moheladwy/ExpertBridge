// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.JobStatuses;

public enum JobStatusEnum
{
    Offered,
    Accepted,
    InProgress,
    Completed,
    PendingClientApproval,
    Declined,
    Cancelled
}
