// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

/// <summary>
///     Represents the response DTO for chat message information.
/// </summary>
/// <remarks>
///     Messages are delivered in real-time via SignalR and stored for chat history.
///     System messages (confirmation messages) have special UI rendering.
/// </remarks>
public class MessageResponse
{
    /// <summary>
    ///     Gets or sets the unique identifier of the message sender.
    /// </summary>
    public required string SenderId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the chat conversation.
    /// </summary>
    public required string ChatId { get; set; }

    /// <summary>
    ///     Gets or sets the text content of the message.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the message was sent.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this is a system-generated confirmation message.
    /// </summary>
    /// <remarks>
    ///     Confirmation messages are rendered differently in the UI, typically with special styling or icons.
    /// </remarks>
    public bool IsConfirmationMessage { get; set; }
}
