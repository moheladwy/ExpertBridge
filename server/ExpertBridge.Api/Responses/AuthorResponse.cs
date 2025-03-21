// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Responses
{
    public record AuthorResponse
    {
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public string? JobTitle { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
