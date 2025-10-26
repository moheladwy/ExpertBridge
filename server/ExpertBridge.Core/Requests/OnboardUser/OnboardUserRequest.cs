// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.OnboardUser;

/// <summary>
///     Represents a request to onboard a new user with their interests.
/// </summary>
/// <remarks>
///     During onboarding, users select tags representing their professional interests.
///     These tags are used to generate personalized content recommendations.
/// </remarks>
public class OnboardUserRequest
{
    /// <summary>
    ///     Gets or sets the collection of tag IDs representing the user's professional interests.
    /// </summary>
    public List<string> Tags { get; set; }
}
