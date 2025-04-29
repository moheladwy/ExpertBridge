

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace ExpertBridge.Api.Extensions;

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

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
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
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    /// <summary>
    ///     Adds the default health checks to the application builder.
    /// </summary>
    /// <param name="builder">
    ///     The WebApplicationBuilder to add the default health checks to.
    /// </param>
    /// <typeparam name="TBuilder">
    ///     The type of the builder.
    /// </typeparam>
    /// <returns>
    ///     The WebApplicationBuilder with the default health checks added.
    /// </returns>
    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var connectionString = builder.Configuration.GetConnectionString("Postgresql")!;
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
            .AddNpgSql(connectionString: connectionString, tags: ["live"]);

        return builder;
    }

    public static TBuilder AddCors<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return builder;
    }
}
