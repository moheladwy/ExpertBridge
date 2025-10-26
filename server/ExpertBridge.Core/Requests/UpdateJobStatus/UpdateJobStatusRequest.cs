// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.UpdateJobStatus;

/// <summary>
/// Represents a request to update the status of an active job.
/// </summary>
/// <remarks>
/// Status changes track job lifecycle: Pending, InProgress, Completed, Cancelled, etc.
/// </remarks>
public class UpdateJobStatusRequest
{
    /// <summary>
    /// Gets or sets the new status for the job.
    /// </summary>
    /// <remarks>
    /// Should be a valid value from the JobStatusEnum enumeration.
    /// </remarks>
    public string Status { get; set; }
}
