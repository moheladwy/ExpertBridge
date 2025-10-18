// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Messages;

/// <summary>
/// Represents a notification message that user interests have been updated via RabbitMQ/MassTransit.
/// </summary>
/// <remarks>
/// This message notifies consumers that a user's interest embeddings have been regenerated
/// and may trigger cache invalidation or recommendation updates.
/// </remarks>
public class UserInterestsUpdatedMessage
{
    /// <summary>
    /// Gets or sets the unique identifier of the user profile that was updated.
    /// </summary>
    public string UserProfileId { get; set; } = null!;
}
