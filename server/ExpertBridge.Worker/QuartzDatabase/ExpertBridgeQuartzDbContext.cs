// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Worker.QuartzDatabase;

/// <summary>
///     Database context for Quartz.NET job scheduler persistence using PostgreSQL.
/// </summary>
/// <remarks>
///     This context is used exclusively for Quartz.NET job scheduling data and includes
///     the PostgreSQL vector extension for advanced database features.
/// </remarks>
internal sealed class ExpertBridgeQuartzDbContext : DbContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ExpertBridgeQuartzDbContext" /> class.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public ExpertBridgeQuartzDbContext(DbContextOptions<ExpertBridgeQuartzDbContext> options) : base(options)
    {
    }

    /// <summary>
    ///     Configures the schema for Quartz.NET tables and PostgreSQL extensions when the model is being created.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("vector");

        // Adds Quartz.NET PostgresSQL schema to EntityFrameworkCore
        modelBuilder.AddQuartz(builder => builder.UsePostgreSql());
    }
}
