// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.Jobs;

/// <summary>
/// Defines validation constraints for Job entity properties.
/// </summary>
/// <remarks>
/// These constraints are used in Entity Framework Core configurations and FluentValidation validators.
/// </remarks>
public class JobEntityConstraints
{
    /// <summary>
    /// Minimum actual cost for completed jobs (0).
    /// </summary>
    public const int MinActualCost = 0;
}
