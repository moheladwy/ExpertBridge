# ExpertBridge Solution - GitHub Copilot Instructions

## Solution Overview

ExpertBridge is a .NET 9.0 enterprise-grade social platform for expert professionals built with clean architecture principles. The solution consists of multiple projects organized in a layered architecture pattern.

### Projects Structure

-   **ExpertBridge.Host** - Aspire hosting orchestration
-   **ExpertBridge.Api** - Web API with REST endpoints
-   **ExpertBridge.Admin** - Blazor Server admin dashboard
-   **ExpertBridge.Application** - Business logic and domain services
-   **ExpertBridge.Core** - Domain entities, interfaces, and DTOs
-   **ExpertBridge.Data** - Database context and EF Core migrations
-   **ExpertBridge.Extensions** - Cross-cutting concerns and infrastructure
-   **ExpertBridge.Notifications** - SignalR real-time notifications
-   **ExpertBridge.GroqLibrary** - AI/LLM integration services
-   **ExpertBridge.UnitTests** - xUnit test project

## Core Coding Standards

### File Headers

Always include the standard license header in new files:

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.
```

### Namespace Conventions

-   Use **file-scoped namespaces** (modern C# style with semicolon):

```csharp
namespace ExpertBridge.Application.DomainServices;

public class MyService
{
    // Implementation
}
```

-   Namespace should match the folder structure
-   Use `ExpertBridge.<ProjectName>.<FolderPath>` pattern

### Naming Conventions

-   **Classes**: PascalCase (e.g., `UserService`, `ProfileRepository`)
-   **Interfaces**: IPascalCase (e.g., `IFirebaseAuthService`, `ISoftDeletable`)
-   **Methods**: PascalCase (e.g., `GetUserByIdAsync`, `CreateProfileAsync`)
-   **Private fields**: `_camelCase` with underscore prefix (e.g., `_dbContext`, `_logger`)
-   **Parameters**: camelCase (e.g., `userId`, `cancellationToken`)
-   **Constants**: PascalCase (e.g., `DefaultTimeout`)
-   **Async methods**: Always suffix with `Async`

### Class Modifiers

-   Use `sealed` for classes that shouldn't be inherited (controllers, DbContext, settings classes)
-   Example:

```csharp
public sealed class UsersController : ControllerBase
public sealed class ExpertBridgeDbContext : DbContext
public sealed class FirebaseSettings
```

### Entity and Model Patterns

#### Base Entity

All entities inherit from `BaseModel`:

```csharp
public abstract class BaseModel : ITimestamped
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
}
```

#### Soft Delete Pattern

Entities supporting soft delete implement `ISoftDeletable`:

```csharp
public class User : BaseModel, ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
```

### Dependency Injection

#### Constructor Injection

Use primary constructors for controllers and simple services:

```csharp
public sealed class UsersController(
    ExpertBridgeDbContext _dbContext
) : ControllerBase
{
    // Implementation
}
```

For complex services with multiple dependencies, use traditional constructors:

```csharp
public class CommentService
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly MediaAttachmentService _mediaService;
    private readonly ILogger<CommentService> _logger;

    public CommentService(
        ExpertBridgeDbContext dbContext,
        MediaAttachmentService mediaService,
        ILogger<CommentService> logger)
    {
        _dbContext = dbContext;
        _mediaService = mediaService;
        _logger = logger;
    }
}
```

### API Controllers

#### Controller Structure

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfilesController : ControllerBase
{
    // Dependencies injected via constructor
    // Action methods
}
```

#### Action Methods

-   Always use async/await pattern
-   Return strongly-typed responses
-   Use proper HTTP verbs and route templates:

```csharp
[HttpGet("get/{identityProviderId}")]
public async Task<UserResponse> GetUserByIdentityProviderId([FromRoute] string identityProviderId)
{
    // Implementation
}

[HttpPost("create")]
public async Task<IActionResult> CreateUser([FromBody] RegisterUserRequest request)
{
    // Implementation
}
```

### Database Context

#### DbContext Pattern

```csharp
public sealed class ExpertBridgeDbContext : DbContext
{
    public ExpertBridgeDbContext(DbContextOptions<ExpertBridgeDbContext> options)
        : base(options)
    {
        ChangeTracker.Tracked += UpdateTimestamps;
        ChangeTracker.StateChanged += UpdateTimestamps;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    // Additional DbSets...
}
```

### Query Extensions

Create extension methods for reusable queries in `ExpertBridge.Core.Queries`:

```csharp
namespace ExpertBridge.Core.Queries
{
    public static class ProfileQueries
    {
        public static IQueryable<Profile> FullyPopulatedProfileQuery(this IQueryable<Profile> query)
        {
            return query
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.ProfileSkills)
                .ThenInclude(ps => ps.Skill);
        }

        public static IQueryable<ProfileResponse> SelectProfileResponseFromProfile(this IQueryable<Profile> query)
        {
            return query.Select(p => new ProfileResponse
            {
                Id = p.Id,
                // Map properties...
            });
        }
    }
}
```

### Exception Handling

#### Custom Exceptions

Create domain-specific exceptions in `ExpertBridge.Core.Exceptions`:

```csharp
public abstract class HttpNotFoundException : Exception
{
    protected HttpNotFoundException(string message) : base(message) { }
}

public class UserNotFoundException : HttpNotFoundException
{
    public UserNotFoundException(string message) : base(message) { }
}
```

#### Global Exception Middleware

Exceptions are handled centrally in `GlobalExceptionMiddleware`:

```csharp
catch (HttpNotFoundException ex)
{
    await Results.NotFound(ex.Message).ExecuteAsync(httpContext);
}
catch (UnauthorizedException ex)
{
    await Results.Unauthorized().ExecuteAsync(httpContext);
}
```

### Request/Response Pattern

#### Request DTOs

Group related request properties in folders with validators:

```csharp
namespace ExpertBridge.Core.Requests.RegisterUser;

public class RegisterUserRequest
{
    public string ProviderId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

#### Response DTOs

Response classes often have constructors that accept entities:

```csharp
namespace ExpertBridge.Core.Responses;

public class UserResponse
{
    public string Id { get; set; }
    public string Email { get; set; }
    // Other properties...

    public UserResponse(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        Id = user.Id;
        Email = user.Email;
        // Map properties...
    }
}
```

### Validation

Use FluentValidation for request validation:

```csharp
public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);
    }
}
```

### Background Services

#### Worker Pattern

```csharp
namespace ExpertBridge.Api.BackgroundServices.PeriodicJobs;

public class PostTaggingPeriodicWorker : PeriodicWorker<PostTaggingPeriodicWorker>
{
    protected override TimeSpan Interval => TimeSpan.FromMinutes(5);

    protected override async Task ExecuteTaskAsync(CancellationToken stoppingToken)
    {
        // Implementation
    }
}
```

### Settings Classes

Configuration settings as sealed classes:

```csharp
public sealed class FirebaseSettings
{
    public const string SectionName = "Firebase";

    public string ApiKey { get; set; }
    public string ProjectId { get; set; }
}
```

Bind in Program.cs:

```csharp
builder.Services.Configure<FirebaseSettings>(
    builder.Configuration.GetSection(FirebaseSettings.SectionName));
```

### Async/Await Best Practices

-   Always use `async`/`await` for I/O operations
-   Use `ConfigureAwait(false)` in library code
-   Accept `CancellationToken cancellationToken = default` parameters
-   Example:

```csharp
public async Task<Profile> GetProfileAsync(string id, CancellationToken cancellationToken = default)
{
    return await _dbContext.Profiles
        .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
}
```

### Logging

Use structured logging with Serilog:

```csharp
private readonly ILogger<MyService> _logger;

_logger.LogInformation("Processing user {UserId} profile", userId);
_logger.LogError(ex, "Failed to process profile for user {UserId}", userId);
```

### Entity Framework Best Practices

-   Use `AsNoTracking()` for read-only queries
-   Use `.Include()` and `.ThenInclude()` for eager loading
-   Create reusable query extensions
-   Use projection with `.Select()` for DTOs
-   Example:

```csharp
var profiles = await _dbContext.Profiles
    .AsNoTracking()
    .Include(p => p.User)
    .Include(p => p.ProfileSkills)
    .Where(p => !p.IsDeleted)
    .SelectProfileResponseFromProfile()
    .ToListAsync(cancellationToken);
```

### Vector Embeddings (Pgvector)

For AI-powered similarity search:

```csharp
using Pgvector;
using Pgvector.EntityFrameworkCore;

var similarProfiles = await _dbContext.Profiles
    .Where(p => p.UserInterestEmbedding != null)
    .OrderBy(p => p.UserInterestEmbedding.CosineDistance(userEmbedding))
    .Take(10)
    .ToListAsync();
```

### SignalR Integration

Hub implementation:

```csharp
public class NotificationsHub : Hub<INotificationClient>
{
    public override async Task OnConnectedAsync()
    {
        // Implementation
    }
}
```

Registration in Program.cs:

```csharp
app.MapHub<NotificationsHub>("/api/notificationsHub");
```

### Testing Conventions

Use xUnit with FluentValidation.TestHelper:

```csharp
namespace ExpertBridge.UnitTests;

public class UserValidatorTests
{
    private readonly IValidator<RegisterUserRequest> _validator;

    [Fact]
    public async Task Should_Have_Error_When_Email_Is_Empty()
    {
        var result = await _validator.TestValidateAsync(new RegisterUserRequest
        {
            Email = string.Empty
        });

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}
```

### JSON Serialization

Configure in Program.cs:

```csharp
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter());
});
```

Use `[JsonIgnore]` for navigation properties:

```csharp
[JsonIgnore]
public Profile Profile { get; set; }
```

### Caching

Use Microsoft.Extensions.Caching.Hybrid for distributed caching:

```csharp
private readonly HybridCache _cache;

var cachedProfile = await _cache.GetOrCreateAsync(
    $"profile:{id}",
    async cancel => await GetProfileFromDbAsync(id, cancel),
    cancellationToken: cancellationToken
);
```

### Channel-based Communication

Use System.Threading.Channels for background processing:

```csharp
private readonly ChannelWriter<UserInterestsProsessingMessage> _channelWriter;

await _channelWriter.WriteAsync(new UserInterestsProsessingMessage
{
    UserId = userId,
    Tags = tags
}, cancellationToken);
```

## Package Management

-   Use **Central Package Management** via `Directory.Packages.props`
-   All package versions are defined centrally
-   Projects reference packages without version attributes:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" />
```

## Build Configuration

-   Target Framework: `net9.0`
-   Nullable Reference Types: `enable`
-   Implicit Usings: `enable`
-   Analysis Level: `latest` with mode `All`
-   Code style enforcement: `EnforceCodeStyleInBuild=true`

## Code Organization

1. **Group using statements** - System namespaces first, then third-party, then project
2. **One class per file** - Exception: nested/partial classes
3. **Folder structure matches namespaces**
4. **Separate concerns** - Controllers handle HTTP, Services handle business logic
5. **Keep Program.cs clean** - Extract configuration to extension methods

## Comments and Documentation

-   Use XML documentation comments for public APIs:

```csharp
/// <summary>
/// Gets the current authenticated user with populated profile.
/// </summary>
/// <returns>The user with profile or null if not authenticated.</returns>
public async Task<User?> GetCurrentUserAsync()
```

-   Avoid obvious comments
-   Explain "why" not "what" when commenting

## Additional Guidelines

1. **Prefer composition over inheritance**
2. **Keep methods small and focused** - Single Responsibility Principle
3. **Use dependency injection** - Avoid service locator pattern
4. **Validate early** - Use guard clauses and FluentValidation
5. **Handle nulls properly** - Use nullable reference types and null-conditional operators
6. **Use modern C# features** - Pattern matching, records, primary constructors when appropriate
7. **Configuration over code** - Use appsettings.json for environment-specific values
8. **Security first** - Always use `[Authorize]` attribute unless specifically public
9. **Performance matters** - Use AsNoTracking, avoid N+1 queries, use async properly
10. **Observable systems** - Implement proper logging, health checks, and telemetry

## Anti-Patterns to Avoid

-   ❌ Don't use `.Result` or `.Wait()` on async operations
-   ❌ Don't expose entities directly via API - use DTOs
-   ❌ Don't use `var` when type is not obvious
-   ❌ Don't catch generic exceptions without logging
-   ❌ Don't mix sync and async code
-   ❌ Don't perform I/O in constructors
-   ❌ Don't use magic strings - use constants or configuration
-   ❌ Don't create God classes - keep classes focused

## References

-   Architecture: Clean Architecture with Domain-Driven Design principles
-   Database: PostgreSQL with Entity Framework Core 9
-   Authentication: Firebase Authentication with JWT
-   Real-time: SignalR for notifications and chat
-   Caching: Redis with FusionCache/HybridCache
-   Logging: Serilog with structured logging
-   API: RESTful with OpenAPI/Swagger documentation
-   Testing: xUnit with FluentAssertions
-   AI/ML: Groq API for LLM, Ollama for embeddings, Pgvector for similarity search
