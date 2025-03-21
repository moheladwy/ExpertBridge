// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Core.Entities.Media.PostMedia;

public class PostMedia
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Foreign keys
    public string PostId { get; set; }
    public string MediaId { get; set; }

    // Navigation properties
    public Posts.Post Post { get; set; }
    public MediaObject Media { get; set; }
}
