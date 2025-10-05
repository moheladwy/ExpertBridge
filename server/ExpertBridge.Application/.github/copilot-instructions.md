# ExpertBridge.Application - GitHub Copilot Instructions

## Project Purpose

Application layer containing business logic, domain services, orchestration, and use cases. This layer coordinates between the domain and infrastructure layers.

## Architecture Role

**Application Layer** - Implements use cases and application-specific business rules. Orchestrates domain entities and infrastructure services to fulfill business requirements.

## Key Responsibilities

-   Domain services (business logic)
-   Use case implementations
-   Application-specific DTOs and models
-   Service coordination and orchestration
-   Business workflow management
-   External service integration (AI, embeddings)
-   Data transformation and mapping
-   Application settings and configuration

## Project Dependencies

```xml
Dependencies:
- ExpertBridge.Core (domain entities, interfaces, DTOs)
- ExpertBridge.Data (database context)
- ExpertBridge.Notifications (real-time notifications)

External:
- Entity Framework Core
- FluentValidation
- Microsoft.Extensions.AI (AI services)
- Bogus (data generation)
```

## Folder Structure

```
ExpertBridge.Application/
├── DomainServices/
│   ├── UserService.cs
│   ├── ProfileService.cs
│   ├── PostService.cs
│   ├── CommentService.cs
│   ├── JobPostingService.cs
│   └── ...
├── Services/
│   ├── FirebaseAuthService.cs
│   ├── S3Service.cs
│   ├── GroqPostTaggingService.cs
│   └── ...
├── Helpers/
│   ├── AuthorizationHelper.cs
│   └── ...
├── Models/
│   └── IPC/ (Inter-process communication)
│       ├── PostEmbeddingMessage.cs
│       └── UserInterestsProsessingMessage.cs
├── Settings/
│   ├── CacheSettings.cs
│   ├── FirebaseAuthSettings.cs
│   └── ...
├── EmbeddingService/
│   └── EmbeddingService.cs
├── DataGenerator/
│   └── Generator.cs (Bogus fake data)
└── LlmOutputFormat/
    └── (AI/LLM response formats)
```

## Domain Service Pattern

### Service Template

```csharp
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Requests;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpertBridge.Application.DomainServices;

public class ProfileService
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(
        ExpertBridgeDbContext dbContext,
        ILogger<ProfileService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ProfileResponse> GetProfileByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var profile = await _dbContext.Profiles
            .FullyPopulatedProfileQuery()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new ProfileNotFoundException($"Profile with id {id} not found.");

        return new ProfileResponse(profile);
    }

    public async Task<List<ProfileResponse>> GetAllProfilesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Profiles
            .AsNoTracking()
            .FullyPopulatedProfileQuery()
            .SelectProfileResponseFromProfile()
            .ToListAsync(cancellationToken);
    }

    public async Task<ProfileResponse> CreateProfileAsync(
        CreateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        // Validate business rules
        await ValidateProfileCreationAsync(request, cancellationToken);

        // Create entity
        var profile = new Profile
        {
            UserId = request.UserId,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Username = request.Username,
            Bio = request.Bio,
            JobTitle = request.JobTitle
        };

        _dbContext.Profiles.Add(profile);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Profile created for user {UserId} with id {ProfileId}",
            request.UserId,
            profile.Id);

        return new ProfileResponse(profile);
    }

    public async Task<ProfileResponse> UpdateProfileAsync(
        string id,
        UpdateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var profile = await _dbContext.Profiles
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new ProfileNotFoundException($"Profile with id {id} not found.");

        // Check business rules
        if (request.Username != profile.Username)
        {
            var usernameExists = await _dbContext.Profiles
                .AnyAsync(p => p.Username == request.Username && p.Id != id, cancellationToken);

            if (usernameExists)
            {
                throw new ProfileUserNameAlreadyExistsException(
                    $"Username {request.Username} is already taken.");
            }
        }

        // Update entity
        profile.FirstName = request.FirstName;
        profile.LastName = request.LastName;
        profile.Username = request.Username;
        profile.Bio = request.Bio;
        profile.JobTitle = request.JobTitle;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile {ProfileId} updated", id);

        return new ProfileResponse(profile);
    }

    public async Task DeleteProfileAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var profile = await _dbContext.Profiles
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new ProfileNotFoundException($"Profile with id {id} not found.");

        // Soft delete
        profile.IsDeleted = true;
        profile.DeletedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile {ProfileId} soft deleted", id);
    }

    private async Task ValidateProfileCreationAsync(
        CreateProfileRequest request,
        CancellationToken cancellationToken)
    {
        // Check if username is unique
        var usernameExists = await _dbContext.Profiles
            .AnyAsync(p => p.Username == request.Username, cancellationToken);

        if (usernameExists)
        {
            throw new ProfileUserNameAlreadyExistsException(
                $"Username {request.Username} is already taken.");
        }

        // Check if email is unique
        var emailExists = await _dbContext.Profiles
            .AnyAsync(p => p.Email == request.Email, cancellationToken);

        if (emailExists)
        {
            throw new BadHttpRequestException(
                $"Email {request.Email} is already in use.");
        }
    }
}
```

## Service with Multiple Dependencies

### Complex Service Pattern

```csharp
namespace ExpertBridge.Application.DomainServices;

public class CommentService
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly MediaAttachmentService _mediaService;
    private readonly NotificationFacade _notificationFacade;
    private readonly TaggingService _taggingService;
    private readonly ChannelWriter<DetectInappropriateCommentMessage> _inappropriateCommentChannel;
    private readonly ILogger<CommentService> _logger;

    public CommentService(
        ExpertBridgeDbContext dbContext,
        MediaAttachmentService mediaService,
        NotificationFacade notificationFacade,
        TaggingService taggingService,
        Channel<DetectInappropriateCommentMessage> inappropriateCommentChannel,
        ILogger<CommentService> logger)
    {
        _dbContext = dbContext;
        _mediaService = mediaService;
        _notificationFacade = notificationFacade;
        _taggingService = taggingService;
        _inappropriateCommentChannel = inappropriateCommentChannel.Writer;
        _logger = logger;
    }

    public async Task<CommentResponse> CreateCommentAsync(
        CreateCommentRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Create comment entity
        var comment = new Comment
        {
            PostId = request.PostId,
            ProfileId = request.ProfileId,
            Content = request.Content,
            ParentCommentId = request.ParentCommentId
        };

        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 2. Process media attachments if any
        if (request.MediaUrls?.Any() == true)
        {
            await _mediaService.AttachMediaToCommentAsync(
                comment.Id,
                request.MediaUrls,
                cancellationToken);
        }

        // 3. Extract and tag entities mentioned in comment
        await _taggingService.TagCommentAsync(comment.Id, cancellationToken);

        // 4. Queue for inappropriate content detection
        await _inappropriateCommentChannel.WriteAsync(
            new DetectInappropriateCommentMessage { CommentId = comment.Id },
            cancellationToken);

        // 5. Send notification to post author
        await _notificationFacade.SendCommentNotificationAsync(comment);

        _logger.LogInformation(
            "Comment {CommentId} created on post {PostId} by profile {ProfileId}",
            comment.Id, request.PostId, request.ProfileId);

        return new CommentResponse(comment);
    }
}
```

## Authorization Helper Pattern

### Current User Access

```csharp
using System.Security.Claims;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Application.Helpers;

public class AuthorizationHelper
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationHelper(
        ExpertBridgeDbContext dbContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the currently authenticated user with Profile navigation property populated.
    /// </summary>
    /// <returns>
    /// The user model from the database with Profile navigation property, or null if not authenticated.
    /// </returns>
    public async Task<User?> GetCurrentUserAsync()
    {
        string? email = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(email))
        {
            return null;
        }

        var user = await _dbContext.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == email);

        return user;
    }

    /// <summary>
    /// Gets the current user's profile ID or empty string if not found.
    /// </summary>
    public async Task<string> GetCurrentUserProfileIdOrEmptyAsync()
    {
        var user = await GetCurrentUserAsync();
        return user?.Profile?.Id ?? string.Empty;
    }

    /// <summary>
    /// Gets the current user's profile or throws UnauthorizedException.
    /// </summary>
    /// <exception cref="UnauthorizedException">User is not authenticated or profile not found.</exception>
    public async Task<Profile> GetCurrentUserProfileOrThrowAsync()
    {
        var user = await GetCurrentUserAsync();

        if (user == null)
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        if (user.Profile == null)
        {
            throw new UnauthorizedGetMyProfileException(
                "User profile not found. Please complete onboarding.");
        }

        return user.Profile;
    }

    /// <summary>
    /// Checks if the current user owns the specified resource.
    /// </summary>
    public async Task<bool> IsResourceOwnerAsync(string profileId)
    {
        var currentProfileId = await GetCurrentUserProfileIdOrEmptyAsync();
        return currentProfileId == profileId;
    }

    /// <summary>
    /// Throws ForbiddenAccessException if current user doesn't own the resource.
    /// </summary>
    public async Task EnsureResourceOwnershipAsync(string profileId)
    {
        if (!await IsResourceOwnerAsync(profileId))
        {
            throw new ForbiddenAccessException("You don't have permission to access this resource.");
        }
    }
}
```

## External Service Integration

### Firebase Auth Service

```csharp
using System.Net.Http.Json;
using ExpertBridge.Application.Settings;
using FirebaseAdmin.Auth;

namespace ExpertBridge.Application.Services;

/// <summary>
/// Provides authentication services using Firebase functionality.
/// </summary>
public class FirebaseAuthService
{
    private readonly FirebaseAuth _auth = FirebaseAuth.DefaultInstance;
    private readonly HttpClient _httpClient;

    public FirebaseAuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Registers a new user with Firebase Authentication.
    /// </summary>
    public async Task<string> RegisterAsync(string email, string password)
    {
        var userArgs = new UserRecordArgs
        {
            Email = email,
            Password = password,
            Disabled = false,
            DisplayName = email,
            EmailVerified = true
        };

        var userRecord = await _auth.CreateUserAsync(userArgs);
        return userRecord.Uid;
    }

    /// <summary>
    /// Authenticates a user and returns Firebase ID token.
    /// </summary>
    public async Task<string> LoginAsync(string email, string password)
    {
        var request = new { email, password, returnSecureToken = true };
        var response = await _httpClient.PostAsJsonAsync("", request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<FirebaseAuthResponse>();
        return result?.IdToken ?? throw new RemoteServiceCallFailedException("Failed to get auth token");
    }

    /// <summary>
    /// Verifies a Firebase ID token.
    /// </summary>
    public async Task<FirebaseToken> VerifyIdTokenAsync(string idToken)
    {
        return await _auth.VerifyIdTokenAsync(idToken);
    }

    /// <summary>
    /// Deletes a user from Firebase.
    /// </summary>
    public async Task DeleteUserAsync(string uid)
    {
        await _auth.DeleteUserAsync(uid);
    }

    private class FirebaseAuthResponse
    {
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
    }
}
```

### AI Service Integration

```csharp
using Microsoft.Extensions.AI;

namespace ExpertBridge.Application.Services;

public class GroqPostTaggingService
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<GroqPostTaggingService> _logger;

    public GroqPostTaggingService(
        IChatClient chatClient,
        ILogger<GroqPostTaggingService> logger)
    {
        _chatClient = chatClient;
        _logger = logger;
    }

    public async Task<List<string>> ExtractTagsFromPostAsync(
        string content,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var systemPrompt = """
                You are a tag extraction expert. Extract 3-5 relevant tags from the given post content.
                Return only the tags as a comma-separated list, without any explanation.
                Tags should be lowercase, single words or short phrases.
                """;

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, systemPrompt),
                new(ChatRole.User, content)
            };

            var response = await _chatClient.CompleteAsync(
                messages,
                cancellationToken: cancellationToken);

            var tagsString = response.Message.Text;
            var tags = tagsString
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim().ToLowerInvariant())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Take(5)
                .ToList();

            _logger.LogInformation(
                "Extracted {Count} tags from post: {Tags}",
                tags.Count,
                string.Join(", ", tags));

            return tags;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract tags from post content");
            return new List<string>();
        }
    }
}
```

## Embedding Service Pattern

```csharp
using Microsoft.Extensions.AI;
using Pgvector;

namespace ExpertBridge.Application.EmbeddingService;

public class EmbeddingService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly ILogger<EmbeddingService> _logger;

    public EmbeddingService(
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        ILogger<EmbeddingService> logger)
    {
        _embeddingGenerator = embeddingGenerator;
        _logger = logger;
    }

    public async Task<Vector?> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            _logger.LogWarning("Attempted to generate embedding for empty text");
            return null;
        }

        try
        {
            var embeddings = await _embeddingGenerator.GenerateAsync(
                new[] { text },
                cancellationToken: cancellationToken);

            var embedding = embeddings.FirstOrDefault();
            if (embedding == null)
            {
                _logger.LogWarning("No embedding generated for text");
                return null;
            }

            // Convert to Pgvector format
            var vector = new Vector(embedding.Vector.ToArray());

            _logger.LogDebug(
                "Generated embedding with dimension {Dimension} for text length {Length}",
                vector.Count,
                text.Length);

            return vector;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate embedding for text");
            return null;
        }
    }

    public async Task<List<Vector>> GenerateBatchEmbeddingsAsync(
        IEnumerable<string> texts,
        CancellationToken cancellationToken = default)
    {
        var textList = texts.ToList();

        if (textList.Count == 0)
        {
            return new List<Vector>();
        }

        try
        {
            var embeddings = await _embeddingGenerator.GenerateAsync(
                textList,
                cancellationToken: cancellationToken);

            return embeddings
                .Select(e => new Vector(e.Vector.ToArray()))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate batch embeddings");
            return new List<Vector>();
        }
    }
}
```

## Settings Pattern

```csharp
namespace ExpertBridge.Application.Settings;

public sealed class FirebaseAuthSettings
{
    public const string SectionName = "FirebaseAuth";

    public string ApiKey { get; set; } = string.Empty;
    public string AuthDomain { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public string TokenUri { get; set; } = string.Empty;
}

public sealed class CacheSettings
{
    public const string SectionName = "Cache";

    public int DefaultDurationSeconds { get; set; } = 300;
    public int UserProfileDurationSeconds { get; set; } = 600;
    public int PostListDurationSeconds { get; set; } = 180;
}
```

## Inter-Process Communication Models

```csharp
namespace ExpertBridge.Application.Models.IPC;

public class PostEmbeddingMessage
{
    public string PostId { get; set; }
    public string Content { get; set; }
}

public class UserInterestsProsessingMessage
{
    public string UserId { get; set; }
    public List<string> Tags { get; set; } = new();
}

public class DetectInappropriateCommentMessage
{
    public string CommentId { get; set; }
}
```

## Best Practices

1. **Single Responsibility** - Each service handles one domain aggregate
2. **Dependency Injection** - Use constructor injection for all dependencies
3. **Async all the way** - All I/O operations should be async
4. **Cancellation tokens** - Accept and propagate cancellation tokens
5. **Structured logging** - Use semantic logging with proper context
6. **Exception handling** - Let domain exceptions bubble up, log infrastructure exceptions
7. **Transaction management** - Use DbContext transactions for multi-step operations
8. **Validation** - Perform business rule validation in services
9. **Null safety** - Use nullable reference types and null checks
10. **Testing** - Services should be easily unit testable

## Anti-Patterns to Avoid

-   ❌ Don't put HTTP concerns in services (no HttpContext, no status codes)
-   ❌ Don't return entities directly (use DTOs/responses)
-   ❌ Don't catch and swallow exceptions without logging
-   ❌ Don't perform authorization checks in domain services (use helpers)
-   ❌ Don't create huge "manager" or "orchestrator" services
-   ❌ Don't mix data access logic (use repositories or DbContext directly)
-   ❌ Don't use static dependencies or service locator
-   ❌ Don't ignore cancellation tokens
-   ❌ Don't perform synchronous I/O
-   ❌ Don't create circular dependencies between services
