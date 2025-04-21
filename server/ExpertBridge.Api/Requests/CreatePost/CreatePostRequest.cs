// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Requests.CreatePost;

public class CreatePostRequest
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public List<MediaObjectRequest>? Media { get; set; }
}
