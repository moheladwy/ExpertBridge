// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ExpertBridge.Data.DatabaseContexts;

namespace ExpertBridge.Tests.Unit.Contract.Queries._Fixtures;

/// <summary>
/// Factory for creating in-memory EF Core database contexts for testing.
/// </summary>
/// <remarks>
/// Uses in-memory database provider which doesn't support pgvector types.
/// A custom model customizer is used to ignore vector properties during model building.
/// </remarks>
public static class InMemoryDbContextFixture
{
    /// <summary>
    /// Creates a new in-memory database context with unique database name.
    /// </summary>
    /// <returns>A new instance of <see cref="ExpertBridgeDbContext"/> configured for in-memory testing.</returns>
    public static ExpertBridgeDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ExpertBridgeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .ReplaceService<IModelCustomizer, InMemoryModelCustomizer>()
            .Options;

        return new ExpertBridgeDbContext(options);
    }
}

/// <summary>
/// Custom model customizer that ignores pgvector properties for in-memory database testing.
/// </summary>
public class InMemoryModelCustomizer : ModelCustomizer
{
    public InMemoryModelCustomizer(ModelCustomizerDependencies dependencies)
        : base(dependencies)
    {
    }

    public override void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.Customize(modelBuilder, context);

        // Ignore Vector properties that are not supported by in-memory database
        modelBuilder.Entity<ExpertBridge.Core.Entities.Posts.Post>()
            .Ignore(p => p.Embedding);

        modelBuilder.Entity<ExpertBridge.Core.Entities.JobPostings.JobPosting>()
            .Ignore(jp => jp.Embedding);

        modelBuilder.Entity<ExpertBridge.Core.Entities.Profiles.Profile>()
            .Ignore(p => p.UserInterestEmbedding);
    }
}
