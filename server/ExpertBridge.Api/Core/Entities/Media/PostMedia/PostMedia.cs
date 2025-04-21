// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.Posts;

namespace ExpertBridge.Api.Core.Entities.Media.PostMedia;

public class PostMedia : MediaObject
{
    // Foreign keys
    public string PostId { get; set; }

    // Navigation properties
    public Post Post { get; set; }
}
