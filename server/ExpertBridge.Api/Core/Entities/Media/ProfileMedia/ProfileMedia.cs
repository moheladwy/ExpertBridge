// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Core.Entities.Media.ProfileMedia;

public class ProfileMedia
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Foreign keys
    public string ProfileId { get; set; }
    public string MediaId { get; set; }

    // Navigation properties
    public Profiles.Profile Profile { get; set; }
    public MediaObject Media { get; set; }
}
