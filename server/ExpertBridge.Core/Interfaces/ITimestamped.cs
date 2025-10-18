// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Interfaces;

/// <summary>
/// Defines the contract for entities that track creation and modification timestamps.
/// </summary>
/// <remarks>
/// Implementing this interface enables automatic timestamp management through Entity Framework interceptors.
/// </remarks>
public interface ITimestamped
{
    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last modified.
    /// </summary>
    DateTime? LastModified { get; set; }
}
