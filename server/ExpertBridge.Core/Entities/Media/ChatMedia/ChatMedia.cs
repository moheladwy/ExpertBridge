// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Chats;

namespace ExpertBridge.Core.Entities.Media.ChatMedia;

/// <summary>
/// Represents a media attachment shared within a chat conversation.
/// </summary>
/// <remarks>
/// Chat media files are stored in AWS S3 and allow users to share images, documents, or other files
/// during job-related discussions between hirers and workers.
/// </remarks>
public class ChatMedia : MediaObject
{
    // Foreign keys
    /// <summary>
    /// Gets or sets the unique identifier of the chat this media belongs to.
    /// </summary>
    public string ChatId { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the chat this media is attached to.
    /// </summary>
    public Chat Chat { get; set; }
}
