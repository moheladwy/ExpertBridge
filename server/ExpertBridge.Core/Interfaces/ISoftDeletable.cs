// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Interfaces;

/// <summary>
/// Defines the contract for entities that support soft deletion.
/// </summary>
/// <remarks>
/// Implementing this interface allows entities to be marked as deleted without physically removing them from the database.
/// This enables audit trails and the possibility of data recovery.
/// </remarks>
public interface ISoftDeletable
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is marked as deleted.
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was marked as deleted.
    /// </summary>
    /// <remarks>
    /// This property is null if the entity has not been deleted.
    /// </remarks>
    DateTime? DeletedAt { get; set; }
}
