// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Responses;

public class GetCommentVotesResponse
{
    public string CommentId { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public bool IsUpvoted { get; set; }
}
