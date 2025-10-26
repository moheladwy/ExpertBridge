// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.CreateMessage;

/// <summary>
/// Represents a request to create a new chat message.
/// </summary>
/// <remarks>
/// <para>
/// Security Warning: The chat ID provided by the client must not be trusted.
/// Always verify that the creating user is a participant in the specified chat before processing.
/// </para>
/// </remarks>
public class CreateMessageRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the chat conversation.
    /// </summary>
    /// <remarks>
    /// <strong>Security:</strong> Always validate that the requesting user is a participant in this chat.
    /// </remarks>
    public required string ChatId { get; set; }

    /// <summary>
    /// Gets or sets the text content of the message.
    /// </summary>
    public required string Content { get; set; }
}
