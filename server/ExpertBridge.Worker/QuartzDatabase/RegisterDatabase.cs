// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Worker.QuartzDatabase;

internal static class RegisterDatabase
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
    public static IServiceCollection AddQuartzDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("QuartzDatabase")!;
        services.AddDbContext<ExpertBridgeQuartzDbContext>(options =>
        {
            options.UseNpgsql(connectionString, o =>
            {
                o.UseVector();
                o.EnableRetryOnFailure(
                    5,
                    TimeSpan.FromSeconds(60),
                    null);
            });
        });
        return services;
    }
}
