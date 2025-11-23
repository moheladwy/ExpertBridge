---
applyTo: "**"
---

# ExpertBridge Server Architecture & Development Guidelines

## Overview

ExpertBridge is a professional networking and knowledge-sharing platform built with .NET 10.0, following Clean Architecture principles with Domain-Driven Design (DDD) patterns. The solution is organized into multiple projects with clear separation of concerns.

---

## Solution Structure

### Core Projects

#### **ExpertBridge.Core**

-   **Purpose**: Domain entities, value objects, and core interfaces
-   **Dependencies**: Minimal (EF Core abstractions, Pgvector)
-   **Key Patterns**:
    -   All entities inherit from `BaseModel` (provides `Id`, `CreatedAt`, `LastModified`)
    -   Domain interfaces: `ISoftDeletable`, `ISafeContent`, `IRecommendableContent`, `ITimestamped`
    -   Organized by aggregate roots (Users/, Posts/, Comments/, Jobs/, etc.)
    -   Entity-specific constraints in `GlobalEntitiesConstraints.cs`
-   **Guidelines**:
    -   Keep business logic pure and framework-independent
    -   Use XML documentation comments for all public members
    -   Entities should be rich domain models with behavior, not anemic data holders
    -   Apply marker interfaces (`ISoftDeletable`, `ISafeContent`) for cross-cutting concerns

#### **ExpertBridge.Data**

-   **Purpose**: Database context, migrations, and EF Core configurations
-   **Dependencies**: EF Core, Npgsql, Redis caching
-   **Key Patterns**:
    -   DbContext configuration
    -   Entity configurations using Fluent API
    -   Migration history
-   **Guidelines**:
    -   Use Fluent API for complex configurations (avoid Data Annotations)
    -   Keep DbContext lean; configuration goes in separate files
    -   Always include migrations with descriptive names
    -   Use `UserSecretsId` for development credentials

#### **ExpertBridge.Contract**

-   **Purpose**: DTOs for API requests and responses
-   **Dependencies**: None (pure POCOs)
-   **Structure**:
    -   `Requests/` - organized by feature (CreatePost/, EditComment/, etc.)
    -   `Responses/` - flat structure with Response suffix
    -   `Messages/` - message broker contracts
    -   `Queries/` - query parameters
-   **Guidelines**:
    -   Keep contracts framework-agnostic
    -   Use record types for immutable DTOs when appropriate
    -   Add FluentValidation validators in Application layer
    -   Never expose domain entities directly in API contracts

#### **ExpertBridge.Application**

-   **Purpose**: Business logic, services, and application orchestration
-   **Dependencies**: Core, Contract, Data, Extensions, Notifications
-   **Key Components**:
    -   Domain services (FirebaseAuthService, S3Service, TaggingService)
    -   Settings classes (NsfwThresholds, etc.)
-   **Guidelines**:
    -   Services should be stateless and registered as Scoped or Transient
    -   Use dependency injection for all dependencies
    -   Implement validation using FluentValidation
    -   Use Z.EntityFramework.Extensions for bulk operations

#### **ExpertBridge.Extensions**

-   **Purpose**: Cross-cutting infrastructure concerns as extension methods
-   **Structure**:
    -   `AWS/` - S3 integration
    -   `Caching/` - FusionCache configuration
    -   `CORS/` - CORS policies
    -   `Embeddings/` - Vector embedding services (Ollama)
    -   `HealthChecks/` - Health check configurations
    -   `Logging/` - Serilog setup
    -   `MessageBroker/` - MassTransit/RabbitMQ
    -   `OpenTelemetry/` - Observability
    -   `Resilience/` - Polly resilience pipelines
-   **Guidelines**:
    -   All configuration as extension methods on `IHostApplicationBuilder` or `IServiceCollection`
    -   Follow naming: `Add{Feature}` or `Configure{Feature}`
    -   Include telemetry/logging in all extensions
    -   Make configurations environment-aware

#### **ExpertBridge.Notifications**

-   **Purpose**: Real-time notifications via SignalR
-   **Dependencies**: SignalR, MassTransit
-   **Key Components**:
    -   `NotificationsHub` - SignalR hub
    -   `NotificationFacade` - abstraction layer
    -   Extension methods for registration
-   **Guidelines**:
    -   Use strongly-typed hub methods
    -   Integrate with message broker for distributed scenarios
    -   Handle connection lifecycle properly

---

### Application Projects

#### **ExpertBridge.Api**

-   **Purpose**: Public REST API
-   **Technology**: ASP.NET Core Web API
-   **Key Features**:
    -   JWT authentication (Firebase Admin SDK)
    -   Scalar/OpenAPI documentation (replaced Swagger)
    -   Response caching with profiles
    -   Global exception middleware
    -   Validation filters
-   **Controllers**: AuthController, PostsController, CommentsController, ProfilesController, JobsController, etc.
-   **Guidelines**:
    -   All controllers inherit `ControllerBase`
    -   Apply `[Authorize]` by default, mark exceptions with `[AllowAnonymous]`
    -   Use `[ResponseCache]` with predefined cache profiles
    -   Return proper HTTP status codes (200, 201, 204, 400, 401, 403, 404, 500)
    -   Use XML documentation for OpenAPI generation
    -   Controllers orchestrate, don't contain business logic
    -   Apply validation filters globally

#### **ExpertBridge.Admin**

-   **Purpose**: Admin dashboard (Blazor Server)
-   **Technology**: Blazor Server with Radzen components
-   **Key Features**:
    -   Identity-based authentication
    -   Moderation tools
    -   Admin-specific database context
-   **Guidelines**:
    -   Keep admin logic separate from public API
    -   Use Razor components with server-side rendering
    -   Apply authorization at component level
    -   Leverage Radzen UI components for consistency

#### **ExpertBridge.Worker**

-   **Purpose**: Background processing and scheduled jobs
-   **Technology**: .NET Worker Service with Quartz.NET
-   **Key Components**:
    -   MassTransit consumers (PostTaggingConsumer, InappropriateContentDetectionConsumer, etc.)
    -   Periodic jobs (Quartz)
    -   AI processing services
-   **Consumers**:
    -   `PostProcessingPipelineConsumer` - orchestrates post processing
    -   `PostTaggingConsumer` - AI-powered tagging
    -   `InappropriatePostDetectionConsumer` - NSFW detection
    -   `PostEmbeddingConsumer` - vector embeddings
    -   `NotificationSendingPipelineConsumer` - notification delivery
    -   `UserInterestsUpdatedConsumer` - interest tracking
-   **Guidelines**:
    -   Consumers should be idempotent
    -   Use scoped services within consumers
    -   Implement retry policies for transient failures
    -   Log all processing steps
    -   Use Quartz for scheduled jobs
    -   Keep consumer logic focused (single responsibility)

#### **ExpertBridge.Host**

-   **Purpose**: Service orchestration (likely Aspire host)
-   **Technology**: .NET Aspire (orchestration)
-   **Guidelines**:
    -   Configure service discovery
    -   Manage distributed resources (Redis, PostgreSQL, RabbitMQ)

---

## Architectural Patterns

### Clean Architecture Layers

1. **Core (Domain)**: Entities, interfaces, domain logic
2. **Application**: Use cases, services, DTOs
3. **Infrastructure**: Extensions, data access, external integrations
4. **Presentation**: API, Admin, Worker

### Dependency Flow

```
Api/Admin/Worker → Application → Core
      ↓
  Extensions → Data → Core
```

### Key Principles

-   Dependencies point inward (toward Core)
-   Core has no external dependencies
-   Infrastructure details in Extensions
-   Domain-driven design for entity modeling

---

## Development Standards

### Naming Conventions

-   **Entities**: Singular nouns (e.g., `Post`, `Comment`, `User`)
-   **Collections**: Plural (e.g., `Posts`, `Comments`)
-   **Services**: `{Domain}Service` (e.g., `PostService`, `TaggingService`)
-   **Controllers**: `{Resource}Controller` (e.g., `PostsController`)
-   **Requests**: `{Action}{Resource}Request` (e.g., `CreatePostRequest`)
-   **Responses**: `{Resource}Response` (e.g., `PostResponse`)
-   **Consumers**: `{Action}Consumer` (e.g., `PostTaggingConsumer`)
-   **Extensions**: `{Feature}.cs` with static class (e.g., `Cors.cs`, `MessageBrokerConfigurations.cs`)

### Code Organization

-   Group by feature/aggregate, not by technical layer
-   One entity per file
-   Separate folder for each aggregate root
-   Keep files under 500 lines; split if larger

### Entity Guidelines

```csharp
/// <summary>
/// Domain entity description
/// </summary>
/// <remarks>
/// Additional context, business rules, AI features
/// </remarks>
public class EntityName : BaseModel, ISoftDeletable, ISafeContent
{
    // Properties with XML docs
    // Navigation properties as ICollection<T>
    // No parameterless constructors if domain logic requires initialization
}
```

### Service Guidelines

```csharp
public class FeatureService
{
    private readonly IRepository _repository;
    private readonly ILogger<FeatureService> _logger;

    public FeatureService(IRepository repository, ILogger<FeatureService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    // Methods with clear single responsibility
    // Return DTOs, not entities
    // Use async/await for I/O operations
    // Log important operations
}
```

### Controller Guidelines

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
[ResponseCache(CacheProfileName = CacheProfiles.PersonalizedContent)]
public class ResourceController : ControllerBase
{
    /// <summary>
    /// Action description
    /// </summary>
    /// <param name="request">Request DTO</param>
    /// <returns>Response DTO</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ResourceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateResourceRequest request)
    {
        // Orchestration logic
        var response = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }
}
```

### Consumer Guidelines

```csharp
public class FeatureConsumer : IConsumer<FeatureMessage>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FeatureConsumer> _logger;

    public async Task Consume(ConsumeContext<FeatureMessage> context)
    {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IService>();

        try
        {
            _logger.LogInformation("Processing message for {Id}", context.Message.Id);
            await service.ProcessAsync(context.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process message");
            throw; // Rethrow for retry policy
        }
    }
}
```

---

## Technology Stack

### Core Frameworks

-   **.NET 10.0** (Latest preview)
-   **Entity Framework Core 10.0**
-   **ASP.NET Core 10.0**
-   **Npgsql** (PostgreSQL provider)

### Data & Caching

-   **PostgreSQL** with **Pgvector** extension (vector similarity search)
-   **Redis** (distributed caching via FusionCache)
-   **Entity Framework Core** with Z.EntityFramework.Extensions for bulk ops

### Messaging & Background Jobs

-   **MassTransit** with **RabbitMQ**
-   **Quartz.NET** (scheduled jobs with PostgreSQL persistence)

### AI & ML

-   **Ollama** (local embeddings via Microsoft.Extensions.AI)
-   **Groq SDK** (LLM API for content moderation & categorization)
-   **Pgvector** (vector embeddings for semantic search)

### Authentication & Authorization

-   **Firebase Admin SDK** (JWT token validation)
-   **ASP.NET Core Identity** (Admin dashboard)
-   **JWT Bearer** authentication

### Observability

-   **Serilog** (structured logging to console, file, OpenTelemetry)
-   **OpenTelemetry** (tracing, metrics)
-   **Prometheus** (metrics export)
-   **AspNetCore.HealthChecks** (health monitoring)

### API Documentation

-   **Scalar.AspNetCore** (replaced Swagger)
-   OpenAPI 3.0 with bearer token support

### Cloud Services

-   **AWS S3** (media storage)
-   **Firebase** (authentication)

### Resilience

-   **Polly** (retry, circuit breaker, timeout policies)
-   **Microsoft.Extensions.Resilience**

### Testing

-   **xUnit**
-   **Shouldly** (assertions)
-   **Microsoft.AspNetCore.Mvc.Testing** (integration tests)

---

## Configuration Management

### Central Package Management

-   Versions defined in `Directory.Packages.props`
-   All projects reference packages without version numbers
-   Update versions centrally

### Build Properties

-   `Directory.Build.props` sets global properties
-   Target framework: `net10.0`
-   Nullable reference types enabled
-   Code analysis: latest with all analyzers
-   Implicit usings enabled

### Settings Pattern

```csharp
public sealed class FeatureSettings
{
    public const string Section = "FeatureName";

    public string Property { get; set; } = default!;
}

// Registration
builder.Services.Configure<FeatureSettings>(
    builder.Configuration.GetSection(FeatureSettings.Section));
```

### Secrets Management

-   Development: User Secrets
-   Production: Environment variables
-   AWS credentials: IAM roles or environment variables
-   Firebase: Service account JSON file or environment variables

---

## Database Guidelines

### Entity Configuration

-   Use Fluent API in separate configuration classes
-   Apply conventions via `BaseModel` configuration
-   Soft delete with query filters
-   Timestamp management via interceptors
-   Indexing for common queries

### Migrations

```bash
# Add migration
dotnet ef migrations add MigrationName --project ExpertBridge.Data --startup-project ExpertBridge.Api

# Update database
dotnet ef database update --project ExpertBridge.Data --startup-project ExpertBridge.Api

# Remove last migration
dotnet ef migrations remove --project ExpertBridge.Data --startup-project ExpertBridge.Api
```

### Query Patterns

-   Use AsNoTracking() for read-only queries
-   Eager load with Include() judiciously
-   Prefer projection (Select) over full entity loading
-   Use bulk operations (Z.EntityFramework.Extensions) for large datasets
-   Implement pagination for list endpoints

---

## Message Broker Patterns

### Publishing

```csharp
await _publishEndpoint.Publish(new EventMessage
{
    EntityId = entity.Id,
    Timestamp = DateTime.UtcNow
});
```

### Consuming

-   Consumers automatically registered via `RegisterMessageBroker(assembly)`
-   Use `IServiceScopeFactory` to resolve scoped services
-   Implement idempotency for all consumers
-   Configure retry policies per message type

### Message Types

-   **Commands**: Imperative, single consumer (e.g., `ProcessPost`)
-   **Events**: Past tense, multiple consumers (e.g., `PostCreated`)
-   **Queries**: Request-response pattern (use Request/Response)

---

## AI & Content Moderation

### NSFW Detection

-   Service: `NsfwContentDetectionService`
-   Thresholds configurable in `appsettings.json` under `NsfwThresholds`
-   Categories: Toxicity, SevereToxicity, Obscene, Threat, Insult, IdentityAttack, SexualExplicit
-   LLM: Groq API with structured JSON output
-   Applies to posts and comments via consumers

### Tagging & Categorization

-   Service: `AiTagProcessorService`, `AiPostTaggingService`
-   Automatic tag extraction from content
-   Tag translation and normalization
-   Embedding generation for semantic search

### Vector Embeddings

-   Generated via Ollama (local) using Microsoft.Extensions.AI
-   Stored in Pgvector columns
-   Used for similarity search and recommendations
-   Entities implementing `IRecommendableContent` support embeddings

---

## Caching Strategy

### FusionCache

-   Distributed (Redis) + local (memory) hybrid
-   Configured via `CacheSettings`
-   Adaptive expiration
-   Fail-safe (cache stale data on backend failure)

### Response Caching

-   Cache profiles: `Default` (5 min), `PersonalizedContent` (3 min with Authorization header)
-   Use `[ResponseCache]` attribute on controllers/actions
-   Vary by header for user-specific content

---

## Security Best Practices

### Authentication

-   Firebase JWT tokens for API
-   Identity cookies for Admin
-   No anonymous access by default (opt-in with `[AllowAnonymous]`)

### Authorization

-   Role-based for Admin
-   Claims-based for API
-   Resource-based checks in services

### Input Validation

-   FluentValidation in Application layer
-   Model validation via `[ApiController]` automatic binding
-   Custom `ValidationFilter` for consistent error responses
-   Sanitize user input before storage

### Content Security

-   NSFW detection for all user-generated content
-   Soft delete for sensitive operations
-   Audit trails for moderation actions

---

## Performance Guidelines

### General

-   Use async/await for I/O operations
-   Avoid N+1 queries (use Include or projections)
-   Cache frequently accessed data
-   Use CDN for static assets (S3)
-   Implement pagination for collections

### Database

-   Index foreign keys and frequently queried columns
-   Use compiled queries for hot paths
-   Batch operations with Z.EntityFramework.Extensions
-   Monitor slow queries with EF Core logging

### API

-   Use response caching
-   Implement conditional requests (ETags)
-   Compress responses (Gzip/Brotli)
-   Use HTTP/2

### Background Processing

-   Process heavy operations asynchronously via message broker
-   Use Quartz for scheduled tasks
-   Implement backpressure for queue management

---

## Testing Guidelines

### Unit Tests

-   Test business logic in isolation
-   Mock external dependencies
-   Use Shouldly for assertions
-   Follow AAA pattern (Arrange, Act, Assert)

### Integration Tests

-   Use `WebApplicationFactory<TProgram>`
-   Test API endpoints end-to-end
-   Use in-memory or test database
-   Reset state between tests

### Test Organization

-   Mirror source structure in test projects
-   Name tests: `MethodName_Scenario_ExpectedResult`
-   Use theory/inline data for parameterized tests

---

## Logging & Monitoring

### Structured Logging

```csharp
_logger.LogInformation("Processing {EntityType} with ID {EntityId}",
    nameof(Post), postId);
```

### Log Levels

-   **Trace**: Very detailed, development only
-   **Debug**: Diagnostic information
-   **Information**: General flow
-   **Warning**: Unexpected but handled
-   **Error**: Failures requiring attention
-   **Critical**: Application-level failures

### Enrichment

-   Environment (Development/Production)
-   Thread ID
-   Process ID
-   Include correlation IDs for distributed tracing

### Telemetry

-   OpenTelemetry for distributed tracing
-   Custom metrics for business KPIs
-   Health checks for dependencies

---

## Common Patterns

### Repository Pattern

-   Not explicitly used; services interact with DbContext directly
-   Apply abstractions where needed for testability
-   Use specification pattern for complex queries

### CQRS (Informal)

-   Read models (responses) differ from write models (requests)
-   Queries optimized differently from commands
-   No formal CQRS framework, but principle applied

### Event Sourcing (Partial)

-   Domain events published via message broker
-   No full event store, but event-driven architecture

### Saga Pattern

-   Distributed transactions via MassTransit sagas
-   Use for multi-step workflows (e.g., job application process)

---

## Error Handling

### Global Exception Middleware

-   Catches all unhandled exceptions
-   Returns consistent error response format
-   Logs exceptions with context
-   Maps exception types to HTTP status codes

### Custom Exceptions

-   Define in `ExpertBridge.Core.Exceptions`
-   Extend `Exception` with domain-specific context
-   Map to appropriate HTTP status in middleware

### Validation Errors

-   Return 400 Bad Request with structured errors
-   Include field-level error messages
-   Use problem details format (RFC 7807)

---

## Deployment

### Docker

-   Dockerfiles provided for Api, Admin, Worker
-   Multi-stage builds for optimization
-   Use official .NET images

### Health Checks

-   `/health` endpoint for liveness
-   `/health/ready` for readiness
-   Check database, Redis, RabbitMQ connectivity

### Configuration

-   Environment variables override appsettings
-   Use secrets management (Kubernetes secrets, AWS Secrets Manager)
-   Connection strings from environment

---

## Code Review Checklist

-   [ ] Follows naming conventions
-   [ ] XML documentation for public APIs
-   [ ] Async/await for I/O operations
-   [ ] Proper exception handling
-   [ ] Input validation
-   [ ] Logging at appropriate levels
-   [ ] Unit/integration tests included
-   [ ] No hardcoded secrets
-   [ ] Database migrations included (if schema changed)
-   [ ] CQRS pattern respected (separate read/write models)
-   [ ] Null reference checks (nullable enabled)
-   [ ] Disposal of unmanaged resources (IDisposable/IAsyncDisposable)

---

## Resources

### Documentation

-   [ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core)
-   [Entity Framework Core Docs](https://docs.microsoft.com/ef/core)
-   [MassTransit Docs](https://masstransit-project.com/)
-   [Serilog Docs](https://serilog.net/)
-   [OpenTelemetry .NET](https://opentelemetry.io/docs/instrumentation/net/)

### Internal References

-   Solution file: `ExpertBridge.sln`
-   Package versions: `Directory.Packages.props`
-   Build config: `Directory.Build.props`
-   SDK version: `global.json` (net10.0)

---

## Quick Start Commands

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run Host
dotnet run --project ExpertBridge.Host/ExpertBridge.Host.csproj --launch-profile http

# Run tests
dotnet test

# Watch mode (hot reload)
dotnet watch --project ExpertBridge.Host/ExpertBridge.Host.csproj --launch-profile http
```

---

## Important Notes

1. **Always inherit from BaseModel** for domain entities
2. **Use extension methods** for infrastructure configuration
3. **Apply marker interfaces** for cross-cutting concerns
4. **Keep Core pure** - no infrastructure dependencies
5. **Document AI features** in entity XML comments
6. **Implement idempotent consumers** for message processing
7. **Use soft delete** via `ISoftDeletable` interface
8. **Apply query filters** in DbContext for soft-deleted items
9. **Configure caching** for read-heavy operations
10. **Monitor AI API costs** (Groq, Ollama usage)
