// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ExpertBridge.Api.Data.DatabaseContexts;

public class ExpertBridgeDbContextFactory : IDesignTimeDbContextFactory<ExpertBridgeDbContext>
{
    public ExpertBridgeDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables("ASPNETCORE_ENVIRONMENT")
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json")
            .AddUserSecrets<ExpertBridgeDbContext>()
            .Build();

        var connectionString = configuration.GetConnectionString("Postgresql")!;
        var optionsBuilder = new DbContextOptionsBuilder<ExpertBridgeDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new ExpertBridgeDbContext(optionsBuilder.Options);
    }
}
