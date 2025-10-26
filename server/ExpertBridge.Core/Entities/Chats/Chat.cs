// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.Messages;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.Chats;

/// <summary>
///     Represents a private conversation between a hirer and a worker regarding a job.
/// </summary>
/// <remarks>
///     Chats are created automatically when a job contract is established and provide a dedicated communication channel
///     for discussing job details, progress updates, and deliverables.
/// </remarks>
public class Chat : BaseModel, ISoftDeletable
{
    /// <summary>
    ///     Gets or sets the unique identifier of the hirer (job poster).
    /// </summary>
    public required string HirerId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the worker (job contractor).
    /// </summary>
    public required string WorkerId { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the chat conversation was ended.
    /// </summary>
    /// <remarks>
    ///     This property is typically set when the associated job is completed or cancelled.
    /// </remarks>
    public DateTime? EndedAt { get; set; }

    /// <summary>
    ///     Gets or sets the profile of the hirer participant.
    /// </summary>
    public Profile Hirer { get; set; }

    /// <summary>
    ///     Gets or sets the profile of the worker participant.
    /// </summary>
    public Profile Worker { get; set; }

    /// <summary>
    ///     Gets or sets the job associated with this chat conversation.
    /// </summary>
    public Job Job { get; set; }

    /// <summary>
    ///     Gets or sets the collection of messages exchanged in this chat.
    /// </summary>
    public ICollection<Message> Messages { get; set; } = [];

    /// <summary>
    ///     Gets or sets a value indicating whether the chat is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the chat was marked as deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    //public ICollection<ChatParticipant> Participants { get; set; } = [];
    //public ICollection<ChatMedia> Medias { get; set; } = [];
}
