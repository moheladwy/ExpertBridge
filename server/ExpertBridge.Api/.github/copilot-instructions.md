# ExpertBridge.Api - GitHub Copilot Instructions

## Project Purpose

Web API project providing RESTful endpoints for the ExpertBridge platform. Handles HTTP requests, authentication, authorization, and orchestrates business logic through domain services.

## Architecture Role

**Presentation Layer** - Entry point for client applications (web, mobile). Responsible for HTTP concerns, not business logic.

## Key Responsibilities

-   HTTP endpoint definitions (Controllers)
-   Request/Response handling
-   Authentication & Authorization (Firebase JWT)
-   API documentation (Swagger/OpenAPI)
-   Global exception handling
-   SignalR hubs for real-time communication
-   Background service hosting
-   Health checks and monitoring

## Project Dependencies

```xml
Dependencies:
- ExpertBridge.Application (business logic)
- ExpertBridge.Core (domain entities, DTOs)
- ExpertBridge.Data (EF Core context)
- ExpertBridge.Extensions (infrastructure)
- ExpertBridge.Notifications (SignalR)
```

## Controller Patterns

### Standard Controller Template

```csharp
using ExpertBridge.Application.DomainServices;
using ExpertBridge.Core.Requests;
using ExpertBridge.Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly ProfileService _profileService;
    private readonly ILogger<ProfilesController> _logger;

    public ProfilesController(
        ExpertBridgeDbContext dbContext,
        ProfileService profileService,
        ILogger<ProfilesController> logger)
    {
        _dbContext = dbContext;
        _profileService = profileService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ResponseCache(CacheProfileName = CacheProfiles.Default)]
    public async Task<ActionResult<ProfileResponse>> GetProfile([FromRoute] string id)
    {
        var profile = await _profileService.GetProfileByIdAsync(id);
        return Ok(profile);
    }

    [HttpPost]
    public async Task<ActionResult<ProfileResponse>> CreateProfile(
        [FromBody] CreateProfileRequest request)
    {
        var profile = await _profileService.CreateProfileAsync(request);
        return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, profile);
    }
}
```

### Route Templates

-   `[HttpGet]` - List/collection endpoints
-   `[HttpGet("{id}")]` - Single resource by ID
-   `[HttpPost]` - Create new resource
-   `[HttpPut("{id}")]` - Full update
-   `[HttpPatch("{id}")]` - Partial update
-   `[HttpDelete("{id}")]` - Delete resource

### Authorization Patterns

```csharp
// Require authentication for all endpoints
[Authorize]
public class SecureController : ControllerBase { }

// Allow anonymous access to specific endpoint
[AllowAnonymous]
[HttpPost("register")]
public async Task<IActionResult> Register() { }

// Require specific role/claim
[Authorize(Roles = "Admin")]
[HttpDelete("delete/{id}")]
public async Task<IActionResult> Delete(string id) { }
```

## Program.cs Configuration

### Service Registration Pattern

```csharp
var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

// Health checks
builder.AddDefaultHealthChecks();

// Service discovery
builder.Services.AddServiceDiscovery();

// HTTP client with resilience
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler();
    http.AddServiceDiscovery();
});

// Domain services
builder.AddExpertBridgeServices();

// Real-time notifications
builder.Services.AddExpertBridgeNotifications();

// API documentation
builder.AddSwaggerGen();

// CORS
builder.AddCors();

// Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
```

### Middleware Pipeline Pattern

```csharp
var app = builder.Build();

// Global exception handling (first)
app.UseMiddleware<GlobalExceptionMiddleware>();

// Logging
app.UseSerilogRequestLogging();

// Swagger (development only)
if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrationAtStartup();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirect (production only)
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// Routing
app.UseRouting();

// Rate limiting
app.UseRateLimiter();

// CORS
app.UseCors(CorsPolicyNames.SignalRClients);

// Authentication before Authorization
app.UseAuthentication();
app.UseAuthorization();

// Custom middleware
app.UseMiddleware<EmailVerifiedMiddleware>();

// Response caching
app.UseResponseCaching();

// Endpoints
app.MapControllers();
app.MapHub<NotificationsHub>("/api/notificationsHub");
app.MapHealthChecks("/health");

await app.RunAsync();
```

## Response Caching

### Cache Profiles

```csharp
// Configure in Program.cs
builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add(CacheProfiles.Default,
        new CacheProfile
        {
            Duration = 300,
            Location = ResponseCacheLocation.Any
        });

    options.CacheProfiles.Add(CacheProfiles.PersonalizedContent,
        new CacheProfile
        {
            Duration = 180,
            Location = ResponseCacheLocation.Any,
            VaryByHeader = "Authorization",
        });
});

// Use in controllers
[ResponseCache(CacheProfileName = CacheProfiles.Default)]
public async Task<ActionResult<List<PostResponse>>> GetPosts() { }
```

## Middleware Patterns

### Custom Middleware Template

```csharp
namespace ExpertBridge.Api.Middleware;

internal class EmailVerifiedMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        // Pre-processing logic
        var endpoint = httpContext.GetEndpoint();

        if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
        {
            await next(httpContext);
            return;
        }

        // Validation logic
        var user = await GetCurrentUserAsync(httpContext);
        if (user?.IsEmailVerified == false)
        {
            await Results.Unauthorized().ExecuteAsync(httpContext);
            return;
        }

        // Continue pipeline
        await next(httpContext);
    }
}

// Registration
app.UseMiddleware<EmailVerifiedMiddleware>();
```

## Background Services

### Location

Place background services in:

-   `BackgroundServices/PeriodicJobs/` - Scheduled tasks
-   `BackgroundServices/Handlers/` - Channel/event handlers

### Periodic Worker Pattern

```csharp
namespace ExpertBridge.Api.BackgroundServices.PeriodicJobs;

public class PostTaggingPeriodicWorker : PeriodicWorker<PostTaggingPeriodicWorker>
{
    private readonly IServiceProvider _serviceProvider;

    public PostTaggingPeriodicWorker(
        IServiceProvider serviceProvider,
        ILogger<PostTaggingPeriodicWorker> logger)
        : base(logger)
    {
        _serviceProvider = serviceProvider;
    }

    protected override TimeSpan Interval => TimeSpan.FromMinutes(5);

    protected override async Task ExecuteTaskAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<MyService>();

        await service.ProcessAsync(stoppingToken);
    }
}
```

### Channel Handler Pattern

```csharp
namespace ExpertBridge.Api.BackgroundServices.Handlers;

public class PostEmbeddingHandlerWorker
{
    private readonly Channel<PostEmbeddingMessage> _channel;
    private readonly IServiceProvider _serviceProvider;

    public PostEmbeddingHandlerWorker(
        Channel<PostEmbeddingMessage> channel,
        IServiceProvider serviceProvider)
    {
        _channel = channel;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<EmbeddingService>();

            await service.GenerateEmbeddingAsync(message.PostId, cancellationToken);
        }
    }
}
```

## Extension Methods

Create extension methods in `Extensions/` folder:

```csharp
namespace ExpertBridge.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddExpertBridgeServices(
        this WebApplicationBuilder builder)
    {
        // Register domain services
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<ProfileService>();

        // Register helpers
        builder.Services.AddScoped<AuthorizationHelper>();

        // Register channels
        builder.Services.AddSingleton(Channel.CreateUnbounded<PostEmbeddingMessage>());

        return builder;
    }

    public static WebApplicationBuilder AddSwaggerGen(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ExpertBridge API",
                Version = "v1"
            });

            // JWT authentication
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });
        });

        return builder;
    }
}
```

## Validation

### Request Validation

```csharp
[HttpPost]
public async Task<ActionResult> CreateProfile(
    [FromBody] CreateProfileRequest request)
{
    // Manual validation with FluentValidation
    var validator = new CreateProfileRequestValidator();
    var validationResult = await validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
        return BadRequest(validationResult.Errors);
    }

    // Or use automatic validation via filters
    var profile = await _profileService.CreateProfileAsync(request);
    return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, profile);
}
```

## Error Responses

### Return Proper Status Codes

```csharp
// 200 OK - Successful GET, PUT, PATCH
return Ok(data);

// 201 Created - Successful POST
return CreatedAtAction(nameof(GetById), new { id }, data);

// 204 No Content - Successful DELETE
return NoContent();

// 400 Bad Request - Validation error
return BadRequest(validationErrors);

// 401 Unauthorized - Authentication required
return Unauthorized();

// 403 Forbidden - Insufficient permissions
return Forbid();

// 404 Not Found - Resource doesn't exist
return NotFound();

// 409 Conflict - Duplicate/constraint violation
return Conflict();

// 500 Internal Server Error - Unhandled exception (via middleware)
```

## Health Checks

### Custom Health Check

```csharp
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ExpertBridgeDbContext _context;

    public DatabaseHealthCheck(ExpertBridgeDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.CanConnectAsync(cancellationToken);
            return HealthCheckResult.Healthy("Database connection is healthy.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database connection failed.", ex);
        }
    }
}

// Registration
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "live" });
```

## Testing Controllers

Controllers should be thin - delegate to services:

```csharp
[HttpPost]
public async Task<ActionResult<PostResponse>> CreatePost(
    [FromBody] CreatePostRequest request)
{
    // ❌ Don't put business logic here
    // ✅ Delegate to service
    var post = await _postService.CreatePostAsync(request);
    return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
}
```

## Best Practices

1. **Keep controllers thin** - No business logic, only orchestration
2. **Use action filters** - For cross-cutting concerns (logging, validation)
3. **Version your API** - Use URL versioning or header versioning
4. **Document endpoints** - Use XML comments and Swagger annotations
5. **Async all the way** - Never block on async operations
6. **Validate input** - Use FluentValidation or Data Annotations
7. **Return DTOs** - Never expose entities directly
8. **Handle exceptions globally** - Use middleware, not try-catch everywhere
9. **Use dependency injection** - Don't use service locator pattern
10. **Respect REST principles** - Use proper HTTP verbs and status codes

## Anti-Patterns to Avoid

-   ❌ Don't access HttpContext directly in services (inject IHttpContextAccessor)
-   ❌ Don't perform business logic in controllers
-   ❌ Don't catch and swallow exceptions
-   ❌ Don't use synchronous I/O operations
-   ❌ Don't create entities directly from request DTOs without validation
-   ❌ Don't expose internal implementation details in responses
-   ❌ Don't forget to use CancellationToken
-   ❌ Don't mix authentication concerns with business logic
