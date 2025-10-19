// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities;

/// <summary>
/// Represents the base entity model with common properties for all domain entities.
/// </summary>
/// <remarks>
/// All domain entities should inherit from this class to ensure consistent identifier and timestamp management.
/// The <see cref="Id"/> property is automatically initialized with a new GUID upon instantiation.
/// </remarks>
public abstract class BaseModel : ITimestamped
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    /// <remarks>
    /// This property is automatically initialized with a new GUID when the entity is instantiated.
    /// </remarks>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last modified.
    /// </summary>
    public DateTime? LastModified { get; set; }
}
