// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

public class MessageResponse
{
    public required string SenderId { get; set; }
    public required string ChatId { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsConfirmationMessage { get; set; } // this will be used for different ui rendering in chat
}
