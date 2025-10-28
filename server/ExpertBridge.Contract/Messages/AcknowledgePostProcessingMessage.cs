// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Contract.Messages;

/// <summary>
///     Represents an acknowledgment message for completed post processing via RabbitMQ/MassTransit.
/// </summary>
/// <remarks>
///     This message confirms that all AI processing steps (tagging, embedding, moderation) have
///     completed and provides the final moderation result.
/// </remarks>
public class AcknowledgePostProcessingMessage
{
    /// <summary>
    ///     Gets or sets a value indicating whether the post content passed moderation checks.
    /// </summary>
    /// <remarks>
    ///     If false, the post may require manual review or may be flagged/removed.
    /// </remarks>
    public bool IsAppropriate { get; set; }
}
