// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Data;

public static class Extensions
{
    /// <summary>
    ///     Adds the database service to the application builder.
    /// </summary>
    /// <param name="services">
    ///     The service collection to add the database service to.
    /// </param>
    /// <param name="configuration">
    ///     The configuration to get the connection string from.
    /// </param>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgresql")!;
        services.AddDbContext<ExpertBridgeDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        return services;
    }
}
