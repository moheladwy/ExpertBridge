// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using ExpertBridge.Api.Core.Entities.Profiles;

namespace ExpertBridge.Api.Core.Entities.Users;

public class User : BaseModel
{
    public required string ProviderId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Email { get; set; }
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsBanned { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsOnboarded { get; set; }

    // Navigation properties
    [JsonIgnore]
    public Profile Profile { get; set; }
}
