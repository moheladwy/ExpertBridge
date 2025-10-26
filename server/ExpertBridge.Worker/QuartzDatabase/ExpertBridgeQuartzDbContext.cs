// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Worker.QuartzDatabase;

internal sealed class ExpertBridgeQuartzDbContext : DbContext
{
    public ExpertBridgeQuartzDbContext(DbContextOptions<ExpertBridgeQuartzDbContext> options) : base(options)
    {
    }

    /// <summary>
    ///     The OnModelCreating method that is called when the model is being created.
    /// </summary>
    /// <param name="modelBuilder">
    ///     The model builder that is used to build the model.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("vector");

        // Adds Quartz.NET PostgresSQL schema to EntityFrameworkCore
        modelBuilder.AddQuartz(builder => builder.UsePostgreSql());
    }
}
