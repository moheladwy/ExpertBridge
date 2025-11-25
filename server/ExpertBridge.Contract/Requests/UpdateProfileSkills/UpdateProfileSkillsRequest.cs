// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Contract.Requests.UpdateProfileSkills;

/// <summary>
///     Represents a request to update the skills associated with a user profile.
/// </summary>
/// <remarks>
///     This replaces the existing skill set with the provided skills.
///     Skills are used for profile matching and job recommendations.
/// </remarks>
public sealed class UpdateProfileSkillsRequest
{
    /// <summary>
    ///     Gets or sets the collection of skill IDs to associate with the profile.
    /// </summary>
    /// <remarks>
    ///     Replaces all existing skills. Pass an empty list to remove all skills.
    /// </remarks>
    public List<string> Skills { get; set; } = [];
}
