// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.UpdateProfileRequest;

public class UpdateProfileRequest
{
    public string? JobTitle { get; set; }
    public string? Bio { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public List<string> Skills { get; set; } = [];
}
