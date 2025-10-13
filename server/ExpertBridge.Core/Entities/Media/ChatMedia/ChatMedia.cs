// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Chats;

namespace ExpertBridge.Core.Entities.Media.ChatMedia;

public class ChatMedia : MediaObject
{
    // Foreign keys
    public string ChatId { get; set; }

    // Navigation properties
    public Chat Chat { get; set; }
}
