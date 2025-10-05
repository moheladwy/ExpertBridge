# ExpertBridge.Extensions - GitHub Copilot Instructions

## Project Purpose

Infrastructure and cross-cutting concerns library providing reusable extension methods for Firebase authentication, AWS S3, caching, CORS, health checks, logging, OpenTelemetry, and embeddings.

## Architecture Role

**Infrastructure/Extensions Layer** - Provides extension methods that configure and integrate third-party services and cross-cutting concerns into the application. All extension methods follow the pattern of extending `IHostApplicationBuilder` or `WebApplicationBuilder`.

## Key Responsibilities

-   Firebase authentication and authorization setup
-   AWS S3 integration for file storage
-   Redis caching with FusionCache
-   CORS policy configuration
-   Health checks for dependencies
-   Serilog structured logging
-   OpenTelemetry observability
-   Pgvector embedding utilities

## Project Dependencies

```xml
Dependencies:
- ExpertBridge.Core (for interfaces only)

External:
- FirebaseAdmin, FirebaseAuth
- Amazon.S3, AWSSDK.Core
- StackExchange.Redis, FusionCache
- AspNetCore.HealthChecks.*
- Serilog, OpenTelemetry
- Pgvector
```

## Extension Method Pattern

### Generic Host Builder Extension Template

All extension methods should:

1. Use generic type constraint `where TBuilder : IHostApplicationBuilder`
2. Return the builder for fluent chaining
3. Read configuration from `builder.Configuration`
4. Register services with `builder.Services`

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExpertBridge.Extensions.MyFeature;

/// <summary>
///     Provides extension methods for configuring MyFeature in a .NET application.
/// </summary>
public static class MyFeatureExtensions
{
    /// <summary>
    ///     Adds MyFeature services to the application.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the host application builder.</typeparam>
    /// <param name="builder">The host application builder to which MyFeature will be added.</param>
    /// <returns>Returns the given host application builder with MyFeature configured.</returns>
    public static TBuilder AddMyFeature<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var settings = builder.Configuration.GetSection(MySettings.SectionName).Get<MySettings>()!;

        builder.Services.AddSingleton<IMyService>(sp =>
        {
            // Service initialization
            return new MyService(settings);
        });

        return builder;
    }
}
```

## Firebase Integration

### Firebase App and Authentication

```csharp
namespace ExpertBridge.Extensions.Firebase;

internal static class Firebase
{
    /// <summary>
    ///     Adds the Firebase app, auth, and messaging services to the application builder.
    /// </summary>
    public static TBuilder AddFirebaseApp<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var firebaseApp = FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile("FirebaseOAuthCredentialsExpertBridge.json")
        });
        var firebaseAuth = FirebaseAuth.GetAuth(firebaseApp);
        var firebaseMessaging = FirebaseMessaging.GetMessaging(firebaseApp);

        builder.Services.AddSingleton(firebaseApp);
        builder.Services.AddSingleton(firebaseAuth);
        builder.Services.AddSingleton(firebaseMessaging);

        return builder;
    }

    /// <summary>
    ///     Adds the Firebase authentication service to the application builder.
    /// </summary>
    public static TBuilder AddFirebaseAuthentication<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var config = builder.Configuration.GetSection("Firebase").Get<FirebaseSettings>();
        var authSettings = builder.Configuration.GetSection("Authentication:Firebase").Get<FirebaseAuthSettings>();

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.Authority = authSettings.Issuer;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = config.ProjectId,
                    ValidateLifetime = true
                };
                options.RequireHttpsMetadata = false;
            });

        builder.Services.AddAuthorization();

        return builder;
    }

    /// <summary>
    ///     Adds the HttpClient service for the Firebase service.
    /// </summary>
    public static TBuilder AddHttpClientForFirebaseService<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var settings = builder.Configuration.GetSection("Firebase").Get<FirebaseSettings>()!;
        builder.Services.AddHttpClient<IFirebaseAuthService>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(settings.AuthenticationTokenUri);
        });

        return builder;
    }
}
```

### Settings Classes

```csharp
namespace ExpertBridge.Extensions.Firebase;

public sealed class FirebaseSettings
{
    public const string SectionName = "Firebase";

    public string ApiKey { get; set; }
    public string ProjectId { get; set; }
    public string AuthenticationTokenUri { get; set; }
}

public sealed class FirebaseAuthSettings
{
    public string Issuer { get; set; }
}
```

## AWS S3 Integration

```csharp
namespace ExpertBridge.Extensions.AWS;

internal static class S3
{
    /// <summary>
    ///     Adds the S3 object service to the application builder.
    /// </summary>
    public static void AddS3ObjectService(this WebApplicationBuilder builder) =>
        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var awsConfig = sp.GetRequiredService<IOptions<AwsSettings>>().Value;
            var credentials = new BasicAWSCredentials(awsConfig.AwsKey, awsConfig.AwsSecret);
            var configurations = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsConfig.Region)
            };
            return new AmazonS3Client(credentials, configurations);
        });
}
```

## Caching with FusionCache

```csharp
namespace ExpertBridge.Extensions.Caching;

public static class FusionCaching
{
    /// <summary>
    ///     Configures FusionCache in a .NET application by setting up default caching options, serialization,
    ///     and distributed caching using Redis, based on the application's configuration.
    /// </summary>
    public static TBuilder AddFusionCache<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var cacheSettings = builder.Configuration.GetSection(CacheSettings.SectionName).Get<CacheSettings>()!;
        var redisConnectionString = builder.Configuration.GetConnectionString("Redis")!;

        builder.Services.AddFusionCache()
            .WithDefaultEntryOptions(options =>
            {
                options.Duration = TimeSpan.FromMinutes(cacheSettings.DefaultDurationInMinutes);
                // Setting this flag to true will execute most of the operations in the background.
                // Resulting in a performance boost.
                options.AllowBackgroundDistributedCacheOperations = true;
            })
            .WithSerializer(new FusionCacheSystemTextJsonSerializer())
            .WithDistributedCache(
#pragma warning disable CA2000
                new RedisCache(
                    new RedisCacheOptions
                    {
                        Configuration = redisConnectionString,
                        InstanceName = cacheSettings.InstanceName
                    }
                )
#pragma warning restore CA2000
            ).AsHybridCache();

        return builder;
    }
}

public class CacheSettings
{
    public const string SectionName = "Redis";

    public string InstanceName { get; set; }
    public int DefaultDurationInMinutes { get; set; } = 10;
}
```

## CORS Configuration

```csharp
namespace ExpertBridge.Extensions.CORS;

public static class Cors
{
    /// <summary>
    ///     Adds a default CORS policy service to allow any origin, method, and header.
    /// </summary>
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

public static class CorsPolicyNames
{
    public const string AllowAll = "AllowAll";
    public const string SignalRClients = "SignalRClients";
}
```

## Health Checks

```csharp
namespace ExpertBridge.Extensions.HealthChecks;

public static class HealthChecks
{
    /// <summary>
    ///     Adds default health checks to the application, including a self-check and a PostgreSQL database check.
    /// </summary>
    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        var npgsqlConnectionString = builder.Configuration.GetConnectionString("Postgresql")!;
        var redisConnectionString = builder.Configuration.GetConnectionString("Redis")!;

        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
            .AddNpgSql(npgsqlConnectionString, tags: ["live"])
            .AddRedis(redisConnectionString, "Redis", tags: ["live"], timeout: TimeSpan.FromSeconds(30));

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });

        // Only health checks tagged with the "live" tag must pass for the app to be considered alive
        app.MapHealthChecks("/alive", new HealthCheckOptions
        { Predicate = r => r.Tags.Contains("live") });

        return app;
    }
}
```

## Logging with Serilog

```csharp
namespace ExpertBridge.Extensions.Logging;

public static class Serilog
{
    /// <summary>
    ///     Configures Serilog as the logging provider for the application using settings from configuration.
    /// </summary>
    public static void AddSerilogLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(context.Configuration));

        builder.AddSeqEndpoint("Seq");
    }
}
```

### appsettings.json Configuration

```json
{
    "Serilog": {
        "Using": [
            "Serilog.Sinks.Console",
            "Serilog.Sinks.File",
            "Serilog.Sinks.Seq"
        ],
        "MinimumLevel": {
            "Default": "Information",
            "Override": {
                "Microsoft": "Warning",
                "System": "Warning"
            }
        },
        "WriteTo": [
            {
                "Name": "Console",
                "Args": {
                    "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                }
            },
            {
                "Name": "File",
                "Args": {
                    "path": "logs/log-.txt",
                    "rollingInterval": "Day",
                    "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                }
            },
            {
                "Name": "Seq",
                "Args": {
                    "serverUrl": "http://localhost:5341"
                }
            }
        ]
    }
}
```

## OpenTelemetry Configuration

```csharp
namespace ExpertBridge.Extensions.OpenTelemetry;

public static class OpenTelemetry
{
    /// <summary>
    ///     Configures OpenTelemetry for the application.
    /// </summary>
    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddServiceDiscovery();

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter();
            })
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    /// <summary>
    ///     Configures default HTTP client settings including timeout, retry policy, and service discovery.
    /// </summary>
    public static TBuilder ConfigureHttpClientDefaults<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });

        return builder;
    }
}
```

## Embeddings with Pgvector

```csharp
namespace ExpertBridge.Extensions.Embeddings;

public static class PgvectorExtensions
{
    /// <summary>
    ///     Converts a ReadOnlyMemory of float values to a Pgvector Vector.
    /// </summary>
    public static Vector ToPgvector(this ReadOnlyMemory<float> embedding) =>
        new(embedding.ToArray());

    /// <summary>
    ///     Converts a float array to a Pgvector Vector.
    /// </summary>
    public static Vector ToPgvector(this float[] embedding) =>
        new(embedding);

    /// <summary>
    ///     Converts a Pgvector Vector to a float array.
    /// </summary>
    public static float[] ToArray(this Vector vector) =>
        vector.ToArray();
}
```

## Best Practices

1. **Use generic type constraints** - All extension methods should use `where TBuilder : IHostApplicationBuilder`
2. **Return the builder** - Enable fluent chaining of configuration methods
3. **Read from configuration** - Use `builder.Configuration.GetSection()` for settings
4. **Use settings classes** - Define settings with `const string SectionName`
5. **Document thoroughly** - Use XML documentation comments for all public methods
6. **Register as singletons** - Most infrastructure services should be singleton lifetime
7. **Use IOptions pattern** - Inject configuration as `IOptions<TSettings>`
8. **Handle connection strings** - Use `builder.Configuration.GetConnectionString()`
9. **Add health checks** - Every external dependency should have a health check
10. **Configure observability** - Add logging, metrics, and tracing for all services

## Configuration Pattern

### Extension Method Usage in Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDatabase(builder.Configuration);

// Firebase
builder.AddFirebaseApp();
builder.AddFirebaseAuthentication();
builder.AddHttpClientForFirebaseService();

// Caching
builder.Services.Configure<CacheSettings>(
    builder.Configuration.GetSection(CacheSettings.SectionName));
builder.AddFusionCache();

// Infrastructure
builder.AddDefaultHealthChecks();
builder.AddCors();
builder.AddSerilogLogging();
builder.ConfigureOpenTelemetry();
builder.ConfigureHttpClientDefaults();

// AWS
builder.Services.Configure<AwsSettings>(
    builder.Configuration.GetSection(AwsSettings.SectionName));
builder.AddS3ObjectService();
```

## Anti-Patterns to Avoid

-   ❌ Don't return `void` from extension methods (return the builder for chaining)
-   ❌ Don't hardcode configuration values in extension methods
-   ❌ Don't use `WebApplicationBuilder` when `IHostApplicationBuilder` works
-   ❌ Don't register services with incorrect lifetimes
-   ❌ Don't skip XML documentation comments
-   ❌ Don't expose implementation details in public APIs
-   ❌ Don't forget to add health checks for external dependencies
-   ❌ Don't mix configuration concerns (keep each extension focused)
-   ❌ Don't use magic strings for configuration keys
-   ❌ Don't create extension methods without proper error handling

## Folder Organization

```
ExpertBridge.Extensions/
├── AWS/
│   ├── S3.cs
│   └── AwsSettings.cs
├── Firebase/
│   ├── Firebase.cs
│   ├── FirebaseSettings.cs
│   └── FirebaseAuthSettings.cs
├── Caching/
│   ├── FusionCaching.cs
│   └── CacheSettings.cs
├── CORS/
│   ├── Cors.cs
│   └── CorsPolicyNames.cs
├── HealthChecks/
│   └── HealthChecks.cs
├── Logging/
│   └── Serilog.cs
├── OpenTelemetry/
│   └── OpenTelemetry.cs
└── Embeddings/
    └── PgvectorExtensions.cs
```

Each folder contains:

1. Main extension class with static extension methods
2. Settings classes for configuration binding
3. Related constants or enums
