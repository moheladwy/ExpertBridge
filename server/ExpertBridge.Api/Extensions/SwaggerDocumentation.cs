// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.OpenApi;

namespace ExpertBridge.Api.Extensions;

/// <summary>
///     Provides extension methods for configuring the application's Swagger documentation.
/// </summary>
internal static class SwaggerDocumentation
{
    /// <summary>
    ///     Adds the Swagger UI service to the application builder.
    /// </summary>
    /// <param name="builder">
    ///     The IHostApplicationBuilder to add the Swagger UI service to.
    /// </param>
    /// <param name="apiName">The api name for the docs.</param>
    /// <param name="apiVersion">The api version for the docs. The default is "v1".</param>
    public static TBuilder AddSwaggerGen<TBuilder>(this TBuilder builder, string apiName, string apiVersion = "v1")
        where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = apiName, Version = apiVersion });

            options.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter your JWT token in this field: {your token}",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

            options.AddSecurityRequirement(doc =>
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference(apiName, doc)
                        {
                            Reference = new OpenApiReferenceWithDescription()
                        },
                        []
                    }
                }
            );
        });

        return builder;
    }
}
