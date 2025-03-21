// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Core.Entities.Media.MediaType;

public class MediaType
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public MediaTypeEnum Type { get; set; }

    // Navigation properties
    public ICollection<MediaObject> Medias { get; set; } = [];
}
