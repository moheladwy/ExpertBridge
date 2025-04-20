// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Core.Entities.Media;

public abstract class MediaObject : BaseModel
{
    public string Name { get; set; }
    public required string Url { get; set; }
    public required string Key { get; set; }
    public required string Type { get; set; }
}
