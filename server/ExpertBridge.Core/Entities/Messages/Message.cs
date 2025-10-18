// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Chats;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.Messages;

/// <summary>
/// Represents a message sent within a chat conversation between a hirer and worker.
/// </summary>
/// <remarks>
/// Messages enable real-time communication through SignalR and are persisted for chat history.
/// Special confirmation messages are marked differently for distinct UI rendering.
/// </remarks>
public class Message : BaseModel, ISoftDeletable
{
    /// <summary>
    /// Gets or sets the unique identifier of the message sender.
    /// </summary>
    public required string SenderId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the chat this message belongs to.
    /// </summary>
    public required string ChatId { get; set; }

    /// <summary>
    /// Gets or sets the text content of the message.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a system-generated confirmation message.
    /// </summary>
    /// <remarks>
    /// Confirmation messages are rendered differently in the UI to distinguish them from regular user messages.
    /// Examples include job acceptance notifications, milestone completions, etc.
    /// </remarks>
    public bool IsConfirmationMessage { get; set; }

    /// <summary>
    /// Gets or sets the profile of the message sender.
    /// </summary>
    public Profile Sender { get; set; } = null!;

    /// <summary>
    /// Gets or sets the chat conversation this message belongs to.
    /// </summary>
    public Chat Chat { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether the message is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the message was marked as deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
