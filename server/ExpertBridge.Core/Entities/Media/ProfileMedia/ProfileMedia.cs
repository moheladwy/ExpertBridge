// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Entities.Media.ProfileMedia;

public class ProfileMedia : MediaObject
{
    // Foreign keys
    public string ProfileId { get; set; }

    // Navigation properties
    public Profile Profile { get; set; }
}
