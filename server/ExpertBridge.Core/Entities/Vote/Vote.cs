// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.Vote;

public class Vote
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public bool IsUpvote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
