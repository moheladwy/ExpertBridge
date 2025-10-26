// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.EntityConfiguration;

/// <summary>
/// Defines PostgreSQL index filter expressions for Entity Framework Core configuration.
/// </summary>
/// <remarks>
/// Used in entity configurations to create partial indexes that only index rows matching filter conditions.
/// This improves query performance and reduces index size for soft-delete patterns.
/// </remarks>
public static class IndexFilters
{
    /// <summary>
    /// Partial index filter that excludes soft-deleted rows (where IsDeleted is true).
    /// </summary>
    /// <remarks>
    /// Use this filter on indexes for entities implementing ISoftDeletable to avoid indexing deleted records.
    /// </remarks>
    public const string NotDeleted = "(\"IsDeleted\") = false";
}
