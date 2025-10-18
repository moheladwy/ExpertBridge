// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExpertBridge.Extensions.CORS;

/// <summary>
/// Provides extension methods for configuring Cross-Origin Resource Sharing (CORS) policies in the ExpertBridge application.
/// Defines multiple policies for different client access patterns including unrestricted API access and SignalR real-time connections.
/// </summary>
public static class Cors
{
    /// <summary>
    /// Registers CORS policies for the application including unrestricted access and SignalR client-specific policies.
    /// Configures two policies: AllowAll for general API access and SignalRClients for real-time WebSocket connections with credentials.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder to configure CORS policies for.</param>
    /// <returns>The builder instance for method chaining.</returns>
    /// <remarks>
    /// This method configures two CORS policies:
    /// 
    /// **AllowAll Policy:**
    /// - Permits requests from any origin
    /// - Allows all HTTP methods (GET, POST, PUT, DELETE, etc.)
    /// - Allows all headers
    /// - Suitable for development or public API endpoints
    /// - Does not support credentials (cookies, authorization headers)
    /// 
    /// **SignalRClients Policy:**
    /// - Restricts origins to specific allowed URLs:
    ///   - http://localhost:5173 (Vite development server)
    ///   - http://localhost:5174 (Alternative development port)
    ///   - https://expert-bridge.netlify.app (Production deployment)
    /// - Allows all HTTP methods and headers
    /// - Enables credentials support for authenticated SignalR connections
    /// - Required for real-time notification and chat features via SignalR hubs
    /// 
    /// Use the policy names from <see cref="CorsPolicyNames"/> when applying policies to controllers or endpoints.
    /// </remarks>
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
