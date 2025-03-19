// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using ExpertBridge.Api.Core.Entities.Profiles;

namespace ExpertBridge.Api.Core.Entities.Users;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProviderId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsBanned { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsOnBoarded { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [JsonIgnore]
    public Profile Profile { get; set; }
}
