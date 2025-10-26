// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Notifications.Models.IPC;

/// <summary>
///     Represents a batch request containing one or more notifications to be processed by the notification pipeline.
///     This is the top-level message type queued in the Channel&lt;SendNotificationsRequestMessage&gt; for IPC.
/// </summary>
/// <remarks>
///     This container enables batching multiple notifications into a single channel message for improved efficiency.
///     The background worker processes all notifications in a batch together, reducing overhead from separate
///     transactions.
///     **Architecture Flow:**
///     1. NotificationFacade creates one or more SendNotificationMessage instances
///     2. Facade wraps them in SendNotificationsRequestMessage
///     3. Facade writes container to Channel (single Write operation)
///     4. NotificationSendingPipelineHandlerWorker reads container from Channel
///     5. Worker persists all notifications in batch (single database transaction)
///     6. Worker broadcasts all notifications via SignalR (parallel operations)
///     **Batching Benefits:**
///     - Reduces channel overhead (one Write/Read for multiple notifications)
///     - Enables atomic database persistence (all notifications in single transaction)
///     - Improves throughput for high-notification scenarios (e.g., popular post with many commenters)
///     - Simplifies error handling (batch succeeds or fails together)
///     **Typical Batch Sizes:**
///     - Single notification: Most common case (one user action triggers one notification)
///     - Multiple notifications: Bulk operations like moderation actions affecting multiple users
///     This is an internal IPC model used exclusively for Channel-based communication between facade and worker.
/// </remarks>
public class SendNotificationsRequestMessage
{
    /// <summary>
    ///     Gets or sets the list of notifications to be processed in this batch request.
    /// </summary>
    /// <remarks>
    ///     Initialized to an empty list to prevent null reference exceptions.
    ///     Typically contains 1-10 notifications per batch under normal load.
    /// </remarks>
    public List<SendNotificationMessage> Notifications { get; set; } = new();
}
