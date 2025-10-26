// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.JobStatuses;

/// <summary>
/// Defines the possible states of a job contract throughout its lifecycle.
/// </summary>
/// <remarks>
/// Job status transitions follow a defined workflow from offer through completion or cancellation.
/// </remarks>
public enum JobStatusEnum
{
    /// <summary>
    /// Job offer has been extended to a worker but not yet accepted.
    /// </summary>
    Offered,

    /// <summary>
    /// Worker has accepted the job offer, contract is established.
    /// </summary>
    Accepted,

    /// <summary>
    /// Work is actively being performed on the job.
    /// </summary>
    InProgress,

    /// <summary>
    /// Job has been completed by the worker.
    /// </summary>
    Completed,

    /// <summary>
    /// Job is awaiting approval from the client (hirer) after completion.
    /// </summary>
    PendingClientApproval,

    /// <summary>
    /// Worker has declined the job offer.
    /// </summary>
    Declined,

    /// <summary>
    /// Job contract has been cancelled by either party.
    /// </summary>
    Cancelled
}
