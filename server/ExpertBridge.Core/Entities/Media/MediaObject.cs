// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Core.Entities.Media;

public abstract class MediaObject : BaseModel, ISoftDeletable
{
    public string Name { get; set; }
    public required string Key { get; set; }
    public required string Type { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; }
}
