# ExpertBridge.Host - GitHub Copilot Instructions

## Project Purpose

.NET Aspire orchestration project for managing the distributed application composed of the API, Admin dashboard, and infrastructure services (Redis, Seq, PostgreSQL). Provides local development environment orchestration.

## Architecture Role

**Service Orchestration** - Aspire App Host that defines, configures, and launches the distributed application with its dependencies, enabling local development with container-based infrastructure.

## Key Responsibilities

-   Define distributed application structure
-   Configure service dependencies and relationships
-   Manage container lifecycles
-   Set up service discovery
-   Configure health checks
-   Enable OpenTelemetry observability
-   Manage connection strings
-   Provide developer dashboard

## Project Dependencies

```xml
Dependencies:
- ExpertBridge.Api (service orchestration)
- ExpertBridge.Admin (service orchestration)

External:
- Aspire.Hosting.AppHost (v9.3.1)
- Aspire.Hosting.Redis
- Aspire.Hosting.PostgreSQL
```

## Program.cs Pattern

### Distributed Application Configuration

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Redis cache configuration
var redisPassword = builder.Configuration["Redis:REDIS_PASSWORD"]!;

var redis = builder
    .AddRedis("Redis", port: 6379)
    .WithImage("redis", "alpine")
    .WithContainerName("expertbridge-redis")
    .WithDataVolume("expertbridge-redis-data")
    .WithPersistence(TimeSpan.FromMinutes(5))
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOtlpExporter()
    .PublishAsConnectionString();

// Seq logging configuration
var seq = builder.AddSeq("Seq", port: 4002)
    .WithContainerName("expertbridge-seq")
    .WithDataVolume("expertbridge-seq-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

// PostgreSQL database configuration
var postgres = builder
    .AddPostgres("Postgresql", port: 5432)
    .WithImage("postgres", "16-alpine")
    .WithContainerName("expertbridge-postgres")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
    .WithDataVolume("expertbridge-postgres-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin()
    .WithOtlpExporter()
    .AddDatabase("ExpertBridgeDb");

// API service configuration
builder.AddProject<ExpertBridge_Api>("ExpertBridgeApi")
    .WithReference(redis)
    .WithReference(postgres)
    .WithReference(seq)
    .WaitFor(redis)
    .WaitFor(postgres)
    .WaitFor(seq)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

// Admin service configuration
builder.AddProject<ExpertBridge_Admin>("ExpertBridgeAdmin")
    .WithReference(redis)
    .WithReference(postgres)
    .WithReference(seq)
    .WaitFor(seq)
    .WaitFor(redis)
    .WaitFor(postgres)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();

builder.Build().Run();
```

## Resource Configuration Patterns

### Redis Configuration

```csharp
var redis = builder
    .AddRedis("Redis", port: 6379)
    .WithImage("redis", "alpine")              // Use Alpine for smaller image
    .WithContainerName("expertbridge-redis")   // Named container for easy identification
    .WithDataVolume("expertbridge-redis-data") // Persist data across restarts
    .WithPersistence(TimeSpan.FromMinutes(5))  // RDB snapshot every 5 minutes
    .WithLifetime(ContainerLifetime.Persistent) // Keep container running
    .WithOtlpExporter()                        // Enable OpenTelemetry
    .PublishAsConnectionString();              // Publish connection string
```

### PostgreSQL Configuration

```csharp
var postgres = builder
    .AddPostgres("Postgresql", port: 5432)
    .WithImage("postgres", "16-alpine")
    .WithContainerName("expertbridge-postgres")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
    .WithDataVolume("expertbridge-postgres-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin()                             // Enable pgAdmin UI
    .WithOtlpExporter()
    .AddDatabase("ExpertBridgeDb");           // Create specific database
```

### Seq Logging Configuration

```csharp
var seq = builder
    .AddSeq("Seq", port: 4002)
    .WithContainerName("expertbridge-seq")
    .WithDataVolume("expertbridge-seq-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints();             // Expose external endpoints
```

## Project References

### Adding Projects with Dependencies

```csharp
builder.AddProject<ExpertBridge_Api>("ExpertBridgeApi")
    .WithReference(redis)           // Inject Redis connection string
    .WithReference(postgres)        // Inject PostgreSQL connection string
    .WithReference(seq)             // Inject Seq endpoint
    .WaitFor(redis)                 // Wait for Redis to be ready
    .WaitFor(postgres)              // Wait for PostgreSQL to be ready
    .WaitFor(seq)                   // Wait for Seq to be ready
    .WithOtlpExporter()             // Enable OpenTelemetry export
    .WithExternalHttpEndpoints();   // Expose external HTTP endpoints
```

## Configuration Patterns

### Reading Configuration

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Read from appsettings.json
var redisPassword = builder.Configuration["Redis:REDIS_PASSWORD"]!;
var databaseName = builder.Configuration["Database:Name"]!;
```

### appsettings.json Structure

```json
{
    "Redis": {
        "REDIS_PASSWORD": "your-redis-password"
    },
    "Database": {
        "Name": "ExpertBridgeDb"
    }
}
```

## Container Lifetime Options

```csharp
// Persistent: Container kept across app restarts
.WithLifetime(ContainerLifetime.Persistent)

// Session: Container removed when app stops
.WithLifetime(ContainerLifetime.Session)
```

## Volume Management

### Named Volumes for Data Persistence

```csharp
.WithDataVolume("expertbridge-redis-data")    // Named volume
.WithBindMount("./data", "/data")              // Bind mount local folder
```

## Service Discovery

Services are automatically registered for service discovery:

```csharp
// In consuming project (Api/Admin), connection strings are injected:
var redisConnection = builder.Configuration.GetConnectionString("Redis");
var postgresConnection = builder.Configuration.GetConnectionString("Postgresql");
```

## Health Checks

Aspire automatically adds health checks:

-   `/health` - Overall health
-   `/alive` - Liveness probe

Access via the Aspire dashboard at `http://localhost:15xxx`

## OpenTelemetry Integration

### Enable for All Services

```csharp
builder.AddProject<ExpertBridge_Api>("ExpertBridgeApi")
    .WithOtlpExporter();  // Exports metrics, traces, logs

var redis = builder.AddRedis("Redis")
    .WithOtlpExporter();  // Monitor Redis metrics
```

## Aspire Dashboard

The dashboard provides:

-   Resource status monitoring
-   Logs aggregation
-   Distributed tracing
-   Metrics visualization
-   Environment variables
-   Connection strings

Access at: `http://localhost:15xxx` (port shown in console)

## Common Configurations

### Environment Variables

```csharp
builder.AddProject<ExpertBridge_Api>("ExpertBridgeApi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("FeatureFlags:EnableNewFeature", "true");
```

### Launch Profiles

```csharp
builder.AddProject<ExpertBridge_Api>("ExpertBridgeApi")
    .WithLaunchProfile("https");  // Use specific launch profile
```

### HTTP/HTTPS Endpoints

```csharp
builder.AddProject<ExpertBridge_Api>("ExpertBridgeApi")
    .WithHttpEndpoint(port: 5000, name: "http")
    .WithHttpsEndpoint(port: 5001, name: "https")
    .WithExternalHttpEndpoints();
```

## Best Practices

1. **Use ContainerLifetime.Persistent for databases** - Preserve data between runs
2. **Add .WaitFor() dependencies** - Ensure services start in correct order
3. **Use named volumes** - Persist data across container recreations
4. **Enable WithOtlpExporter** - Essential for observability
5. **Use WithExternalHttpEndpoints** - Expose services for external access
6. **Configure health checks** - Automatic with Aspire, validates in dashboard
7. **Use Alpine images** - Smaller, faster container startup
8. **Name containers consistently** - Use project-resource naming pattern
9. **Configure PgAdmin for PostgreSQL** - Helpful for database inspection
10. **Read sensitive config from secrets** - Never hardcode passwords

## Development Workflow

### Start the Application

```bash
dotnet run --project ExpertBridge.Host
```

### Access Services

-   **Aspire Dashboard**: `http://localhost:15xxx`
-   **API**: `http://localhost:5000` / `https://localhost:5001`
-   **Admin**: `http://localhost:5002`
-   **Seq**: `http://localhost:4002`
-   **pgAdmin**: `http://localhost:5050`

### Stop Containers

```bash
# Stop session containers (automatic)
# Persistent containers keep running

# To stop persistent containers:
docker stop expertbridge-redis expertbridge-postgres expertbridge-seq
```

## manifest.json

Aspire generates a deployment manifest:

```json
{
    "resources": {
        "Redis": {
            "type": "container.v0",
            "connectionString": "...",
            "image": "redis:alpine"
        },
        "ExpertBridgeApi": {
            "type": "project.v0",
            "path": "../ExpertBridge.Api/ExpertBridge.Api.csproj"
        }
    }
}
```

## Anti-Patterns to Avoid

-   ❌ Don't use Session lifetime for databases (data loss)
-   ❌ Don't hardcode connection strings (use Aspire's service discovery)
-   ❌ Don't forget .WaitFor() on dependencies (race conditions)
-   ❌ Don't skip WithOtlpExporter (lose observability)
-   ❌ Don't expose internal services externally unnecessarily
-   ❌ Don't forget to configure data volumes (ephemeral data)
-   ❌ Don't use 'latest' tags (unpredictable versions)
-   ❌ Don't start too many services at once (resource consumption)
-   ❌ Don't forget to clean up volumes periodically
-   ❌ Don't skip reading the Aspire dashboard logs

## Folder Structure

```
ExpertBridge.Host/
├── Program.cs                     # Aspire orchestration
├── appsettings.json              # Configuration
├── appsettings.Development.json  # Dev overrides
├── manifest.json                 # Deployment manifest
├── aspirate-state.json           # Aspirate CLI state
└── Properties/
    └── launchSettings.json       # Launch profiles
```

## Aspirate CLI

For Kubernetes deployment:

```bash
# Install aspirate
dotnet tool install -g aspirate

# Generate Kubernetes manifests
aspirate generate

# Apply to cluster
aspirate apply
```

## Additional Resources Configuration

### Kafka (Example)

```csharp
var kafka = builder.AddKafka("messaging")
    .WithLifetime(ContainerLifetime.Persistent);
```

### RabbitMQ (Example)

```csharp
var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);
```

### MongoDB (Example)

```csharp
var mongo = builder.AddMongoDB("mongodb")
    .WithMongoExpress()
    .WithDataVolume();
```
