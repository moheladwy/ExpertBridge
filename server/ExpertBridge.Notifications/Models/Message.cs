// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Notifications.Models;

/// <summary>
/// Represents a chat message transmitted via SignalR to connected clients for real-time messaging functionality.
/// </summary>
/// <remarks>
/// This DTO is designed specifically for SignalR communication through the <see cref="INotificationClient.ReceiveMessage"/> method.
/// It carries essential message data from server to client without coupling to persistence models.
/// 
/// **Key Properties:**
/// - SenderId: Profile ID of the message sender
/// - ReceiverId: Profile ID of the message recipient
/// - ChatId: Unique identifier for the conversation thread
/// - Content: The text content of the message
/// - CreatedAt: Timestamp when the message was created
/// - IsConfirmationMessage: Flag for special UI rendering (e.g., system messages, read receipts)
/// 
/// **Use Cases:**
/// - Real-time one-to-one messaging
/// - Chat notifications with special formatting (confirmations, receipts)
/// - Message synchronization across multiple client sessions
/// 
/// The separation from domain entities provides flexibility for future changes to the messaging system
/// without affecting database schema or other application layers.
/// </remarks>
public class Message
{
    /// <summary>
    /// Gets or sets the profile ID of the user sending the message.
    /// </summary>
    public required string SenderId { get; set; }

    /// <summary>
    /// Gets or sets the profile ID of the user receiving the message.
    /// </summary>
    public required string ReceiverId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the chat conversation thread.
    /// </summary>
    public required string ChatId { get; set; }

    /// <summary>
    /// Gets or sets the text content of the message.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the message was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a special confirmation message requiring different UI rendering.
    /// </summary>
    /// <remarks>
    /// When true, the client should render this message differently than regular chat messages
    /// (e.g., system notifications, read receipts, typing indicators, or status updates).
    /// </remarks>
    public bool IsConfirmationMessage { get; set; }
}
