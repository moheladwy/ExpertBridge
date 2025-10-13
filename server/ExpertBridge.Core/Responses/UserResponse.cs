// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Users;

namespace ExpertBridge.Core.Responses;

public class UserResponse
{
    public UserResponse(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        Id = user.Id;
        FirebaseId = user.ProviderId;
        Email = user.Email;
        Username = user.Username;
        PhoneNumber = user.PhoneNumber;
        FirstName = user.FirstName;
        LastName = user.LastName;
        IsBanned = user.IsBanned;
        IsEmailVerified = user.IsEmailVerified;
        IsDeleted = user.IsDeleted;
    }

    public string Id { get; set; }
    public string FirebaseId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsBanned { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsDeleted { get; set; }
}
