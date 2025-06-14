// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExpertBridge.Extensions.CORS;

public static class Cors
{
    /// <summary>
    ///     Adds a default CORS policy service to allow any origin, method, and header.
    /// </summary>
    /// <param name="builder">
    ///     The application builder to configure CORS service for.
    /// </param>
    /// <typeparam name="TBuilder">
    ///     The type of the application builder.
    /// </typeparam>
    /// <returns>
    ///     The application builder with CORS service configured.
    /// </returns>
    public static TBuilder AddCors<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyNames.AllowAll, policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            options.AddPolicy(CorsPolicyNames.SignalRClients, policy =>
            {
                policy.WithOrigins(
                        "http://localhost:5173",
                        "http://localhost:5174",
                        "https://expert-bridge.netlify.app"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return builder;
    }
}
