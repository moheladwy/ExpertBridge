// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Responses
{
    // TODO - You will propably need more fields here.
    public class ProfileResponse
    {
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public double Rating { get; set; }
        public int RatingCount { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public string? Username { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsBanned { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? JobTitle { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
