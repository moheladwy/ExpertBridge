using Microsoft.OpenApi.Models;

namespace ExpertBridge.Api.Extensions;

/// <summary>
///     Provides extension methods for configuring the application's startup services.
/// </summary>
internal static class Startup
{
    /// <summary>
    ///     Adds the Swagger UI service to the application builder.
    /// </summary>
    /// <param name="builder">
    ///     The WebApplicationBuilder to add the Swagger UI service to.
    /// </param>
    public static void AddSwaggerGen(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "ExpertBridge API", Version = "v1" });

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

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

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
    //public static TBuilder AddCors<TBuilder>(this TBuilder builder)
    //    where TBuilder : IHostApplicationBuilder
    //{
    //    builder.Services.AddCors(options =>
    //    {
    //        options.AddPolicy("AllowAll", policy =>
    //        {
    //            policy.AllowAnyOrigin()
    //                .AllowAnyMethod()
    //                .AllowAnyHeader();
    //        });

    //        options.AddPolicy("SignalRClients", policy =>
    //        {
    //            policy.WithOrigins(
    //                "http://localhost:5173",
    //                "http://localhost:5174",
    //                "https://expert-bridge.netlify.app"
    //                )
    //                .AllowAnyMethod()
    //                .AllowAnyHeader()
    //                .AllowCredentials();
    //        });
    //    });

    //    return builder;
    //}
}
