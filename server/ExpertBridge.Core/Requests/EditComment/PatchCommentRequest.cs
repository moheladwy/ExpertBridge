// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.EditComment;

public class PatchCommentRequest
{
    public required string CommentId { get; set; }
    public bool? Upvote { get; set; }
    public bool? Downvote { get; set; }
    public string? Content { get; set; }
}
