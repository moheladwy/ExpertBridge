// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests;

public class CreateMessageRequest
{
    // WARNING!
    // Do NOT trust the chat id coming from the client.
    // Always check if the creating user is participent in this chat first.
    public required string ChatId { get; set; }
    public required string Content { get; set; }
}
