// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Chats;
using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ChatParticipants;

/// <summary>
///     Represents a many-to-many relationship between chats and profiles.
/// </summary>
/// <remarks>
///     Chat participants define membership in job-related conversations between hirers and workers.
///     Typically, a chat has two participants: the job poster and the applicant/worker.
/// </remarks>
public class ChatParticipant
{
    /// <summary>
    ///     Gets or sets the unique identifier of the chat.
    /// </summary>
    public string ChatId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the profile participating in the chat.
    /// </summary>
    public string ProfileId { get; set; }

    // Navigation properties
    /// <summary>
    ///     Gets or sets the chat this participant belongs to.
    /// </summary>
    public Chat Chat { get; set; }

    /// <summary>
    ///     Gets or sets the profile participating in the chat.
    /// </summary>
    public Profile Profile { get; set; }
}
