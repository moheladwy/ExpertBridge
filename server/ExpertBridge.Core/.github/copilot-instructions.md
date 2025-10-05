# ExpertBridge.Core - GitHub Copilot Instructions

## Project Purpose

Core domain layer containing entities, value objects, DTOs, interfaces, and domain-specific logic. This is the heart of the application with no external dependencies.

## Architecture Role

**Domain Layer** - Central to clean architecture. Contains business entities and rules. All other projects depend on Core, but Core depends on nothing.

## Key Responsibilities

-   Domain entities and value objects
-   Business interfaces (contracts)
-   DTOs (Request/Response models)
-   Domain exceptions
-   Query extension methods
-   Entity configurations
-   Domain constants and enums
-   Validation rules (FluentValidation)

## Project Dependencies

```xml
<!-- Minimal external dependencies -->
- FirebaseAdmin (for auth contracts)
- FluentValidation (for validation)
- Microsoft.EntityFrameworkCore (for entity attributes)
- Pgvector (for vector similarity)

<!-- NO dependencies on other ExpertBridge projects -->
```

## Entity Patterns

### Base Entity Template

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities.Users;

public class User : BaseModel, ISoftDeletable
{
    // Required properties (use 'required' keyword for non-nullable)
    public required string ProviderId { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }

    // Optional properties
    public string? FirstName { get; set; }
    public string? PhoneNumber { get; set; }

    // Boolean flags
    public bool IsBanned { get; set; }
    public bool IsEmailVerified { get; set; }

    // Soft delete properties (from ISoftDeletable)
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation properties (use [JsonIgnore])
    [JsonIgnore]
    public Profile Profile { get; set; }

    [JsonIgnore]
    public List<Post> Posts { get; set; } = new();
}
```

### Interface Implementations

```csharp
// ITimestamped - For entities that track creation/modification
public interface ITimestamped
{
    DateTime? CreatedAt { get; set; }
    DateTime? LastModified { get; set; }
}

// ISoftDeletable - For entities that support soft delete
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}

// IRecommendableContent - For entities with embeddings
public interface IRecommendableContent
{
    Vector? ContentEmbedding { get; set; }
}
```

### Entity Configuration Pattern

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Users;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name
        builder.ToTable("Users");

        // Primary key
        builder.HasKey(u => u.Id);

        // Indexes
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.ProviderId).IsUnique();

        // Required fields with max length
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(UserEntityConstraints.EmailMaxLength);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(UserEntityConstraints.UsernameMaxLength);

        // Optional fields
        builder.Property(u => u.FirstName)
            .HasMaxLength(UserEntityConstraints.FirstNameMaxLength);

        // Relationships
        builder.HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<Profile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Query filters (soft delete)
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
```

### Entity Constraints

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities.Users;

public static class UserEntityConstraints
{
    public const int EmailMaxLength = 255;
    public const int UsernameMinLength = 3;
    public const int UsernameMaxLength = 50;
    public const int FirstNameMaxLength = 100;
    public const int LastNameMaxLength = 100;
    public const int PhoneNumberMaxLength = 20;
}
```

## Request/Response DTOs

### Request DTO Pattern

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

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

### Request Validator Pattern

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Requests.RegisterUser;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.ProviderId)
            .NotEmpty()
            .WithMessage("Provider ID is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(UserEntityConstraints.EmailMaxLength)
            .WithMessage("Valid email is required.");

        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(UserEntityConstraints.UsernameMinLength)
            .MaximumLength(UserEntityConstraints.UsernameMaxLength)
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage("Username must contain only letters, numbers, and underscores.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(UserEntityConstraints.FirstNameMaxLength);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(UserEntityConstraints.LastNameMaxLength);
    }
}
```

### Response DTO Pattern

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Users;

namespace ExpertBridge.Core.Responses;

public class UserResponse
{
    public string Id { get; set; }
    public string FirebaseId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsBanned { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsDeleted { get; set; }

    // Constructor for mapping from entity
    public UserResponse(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        Id = user.Id;
        FirebaseId = user.ProviderId;
        Email = user.Email;
        Username = user.Username;
        PhoneNumber = user.PhoneNumber;
        FirstName = user.FirstName;
        LastName = user.LastName;
        IsBanned = user.IsBanned;
        IsEmailVerified = user.IsEmailVerified;
        IsDeleted = user.IsDeleted;
    }

    // Parameterless constructor for JSON deserialization
    public UserResponse() { }
}
```

## Query Extensions

### Query Extension Pattern

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Core.Queries;

public static class ProfileQueries
{
    /// <summary>
    /// Eagerly loads all navigation properties for a profile query.
    /// </summary>
    public static IQueryable<Profile> FullyPopulatedProfileQuery(
        this IQueryable<Profile> query)
    {
        return query
            .AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.ProfileSkills)
                .ThenInclude(ps => ps.Skill)
            .Include(p => p.Comments)
                .ThenInclude(c => c.Votes)
            .Include(p => p.ProfileBadges)
                .ThenInclude(pb => pb.Badge);
    }

    /// <summary>
    /// Applies a predicate after loading navigation properties.
    /// </summary>
    public static IQueryable<Profile> FullyPopulatedProfileQuery(
        this IQueryable<Profile> query,
        Expression<Func<Profile, bool>> predicate)
    {
        return query
            .FullyPopulatedProfileQuery()
            .Where(predicate);
    }

    /// <summary>
    /// Projects profile entities to ProfileResponse DTOs.
    /// Use this for efficient data transfer without loading full entities.
    /// </summary>
    public static IQueryable<ProfileResponse> SelectProfileResponseFromProfile(
        this IQueryable<Profile> query)
    {
        return query
            .Select(p => new ProfileResponse
            {
                Id = p.Id,
                UserId = p.UserId,
                CreatedAt = p.CreatedAt.Value,
                Email = p.Email,
                FirstName = p.FirstName,
                LastName = p.LastName,
                IsBanned = p.IsBanned,
                JobTitle = p.JobTitle,
                Bio = p.Bio,
                PhoneNumber = p.PhoneNumber,
                ProfilePictureUrl = p.ProfilePictureUrl,
                Rating = p.Rating,
                RatingCount = p.RatingCount,
                Username = p.Username,
                IsOnboarded = p.User.IsOnboarded,
                Skills = p.SelectSkillsNamesFromProfile(),
                CommentsUpvotes = p.Comments.Sum(c => c.Votes.Count(v => v.IsUpvote)),
                CommentsDownvotes = p.Comments.Sum(c => c.Votes.Count(v => !v.IsUpvote)),
                Reputation = p.SelectReputationFromProfile(),
            });
    }

    /// <summary>
    /// Projects profile to lightweight author response.
    /// Use for comments, posts, etc. where full profile data isn't needed.
    /// </summary>
    public static AuthorResponse? SelectAuthorResponseFromProfile(
        this Profile? profile)
    {
        return profile == null ? null : new AuthorResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            JobTitle = profile.JobTitle,
            ProfilePictureUrl = profile.ProfilePictureUrl,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Username = profile.Username
        };
    }

    // Helper methods (private)
    private static List<string> SelectSkillsNamesFromProfile(this Profile profile)
    {
        return profile?.ProfileSkills
            .Select(ps => ps.Skill.Name)
            .ToList() ?? [];
    }

    private static int SelectReputationFromProfile(this Profile profile)
    {
        return profile.Comments.Sum(c => c.Votes.Count(v => v.IsUpvote))
               - profile.Comments.Sum(c => c.Votes.Count(v => !v.IsUpvote));
    }
}
```

## Exception Patterns

### Custom Exception Hierarchy

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Exceptions;

// Base exception for 404 Not Found
public abstract class HttpNotFoundException : Exception
{
    protected HttpNotFoundException(string message) : base(message) { }
}

// Specific not found exceptions
public class UserNotFoundException : HttpNotFoundException
{
    public UserNotFoundException(string message) : base(message) { }
}

public class ProfileNotFoundException : HttpNotFoundException
{
    public ProfileNotFoundException(string message) : base(message) { }
}

public class PostNotFoundException : HttpNotFoundException
{
    public PostNotFoundException(string message) : base(message) { }
}

// Base exception for 401 Unauthorized
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}

public class UnauthorizedGetMyProfileException : UnauthorizedException
{
    public UnauthorizedGetMyProfileException(string message) : base(message) { }
}

// 403 Forbidden
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message) : base(message) { }
}

// Business rule violations
public class ProfileUserNameAlreadyExistsException : Exception
{
    public ProfileUserNameAlreadyExistsException(string message) : base(message) { }
}

public class ProfilePhoneNumberAlreadyExistsException : Exception
{
    public ProfilePhoneNumberAlreadyExistsException(string message) : base(message) { }
}

// External service failures
public class RemoteServiceCallFailedException : Exception
{
    public RemoteServiceCallFailedException(string message) : base(message) { }
    public RemoteServiceCallFailedException(string message, Exception innerException)
        : base(message, innerException) { }
}

// Bad request
public class BadHttpRequestException : Exception
{
    public BadHttpRequestException(string message) : base(message) { }
}
```

## Interface Patterns

### Service Interface Template

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Interfaces;

public interface IFirebaseAuthService
{
    Task<string> RegisterAsync(string email, string password);
    Task<string> LoginAsync(string email, string password);
    Task<FirebaseToken> VerifyIdTokenAsync(string idToken);
    Task DeleteUserAsync(string uid);
}
```

## Folder Organization

```
ExpertBridge.Core/
├── Entities/
│   ├── BaseModel.cs
│   ├── GlobalEntitiesConstraints.cs
│   ├── Users/
│   │   ├── User.cs
│   │   ├── UserEntityConfiguration.cs
│   │   ├── UserEntityConstraints.cs
│   │   └── UserEntityValidator.cs
│   ├── Profiles/
│   ├── Posts/
│   └── ...
├── Interfaces/
│   ├── ITimestamped.cs
│   ├── ISoftDeletable.cs
│   ├── IRecommendableContent.cs
│   └── IFirebaseAuthService.cs
├── Requests/
│   ├── RegisterUser/
│   │   ├── RegisterUserRequest.cs
│   │   └── RegisterUserRequestValidator.cs
│   ├── UpdateProfileRequest/
│   └── ...
├── Responses/
│   ├── UserResponse.cs
│   ├── ProfileResponse.cs
│   ├── PostResponse.cs
│   └── ...
├── Queries/
│   ├── ProfileQueries.cs
│   ├── PostQueries.cs
│   └── ...
├── Exceptions/
│   └── ExpertBridgeExceptions.cs
└── EntityConfiguration/
    └── (if needed for shared configs)
```

## Best Practices

1. **Pure domain logic** - No infrastructure concerns (no database, HTTP, file system)
2. **Self-contained entities** - All validation rules live with the entity
3. **Rich domain models** - Entities can have behavior, not just data
4. **Immutable value objects** - Use records for value objects
5. **Query separation** - Keep query logic separate from entities
6. **Consistent naming** - Follow established patterns (EntityConfiguration, EntityConstraints)
7. **Guard clauses** - Validate constructor parameters
8. **Null safety** - Use nullable reference types properly
9. **Documentation** - Document public interfaces and complex logic
10. **No magic strings** - Use constants for validation messages

## Anti-Patterns to Avoid

-   ❌ Don't reference infrastructure projects (Data, Extensions, Api)
-   ❌ Don't put database queries in entities
-   ❌ Don't expose collections directly (use IReadOnlyCollection)
-   ❌ Don't use anemic domain models (entities should have behavior)
-   ❌ Don't couple DTOs to entities (separate concerns)
-   ❌ Don't put UI concerns in domain layer
-   ❌ Don't use DateTime.Now directly (inject time provider)
-   ❌ Don't catch exceptions in domain layer (propagate up)
-   ❌ Don't perform I/O operations in constructors
-   ❌ Don't use mutable value objects

## Testing Core Layer

Core layer should be easily testable:

```csharp
// Entity tests
[Fact]
public void User_Constructor_SetsIdToGuid()
{
    var user = new User { /* properties */ };
    Assert.NotNull(user.Id);
    Assert.NotEqual(Guid.Empty.ToString(), user.Id);
}

// Validator tests
[Fact]
public async Task RegisterUserRequest_WithInvalidEmail_FailsValidation()
{
    var validator = new RegisterUserRequestValidator();
    var request = new RegisterUserRequest { Email = "invalid" };

    var result = await validator.TestValidateAsync(request);

    result.ShouldHaveValidationErrorFor(x => x.Email);
}

// Query extension tests (requires in-memory database)
[Fact]
public async Task FullyPopulatedProfileQuery_LoadsAllNavigationProperties()
{
    // Arrange
    var options = new DbContextOptionsBuilder<ExpertBridgeDbContext>()
        .UseInMemoryDatabase("TestDb")
        .Options;

    using var context = new ExpertBridgeDbContext(options);
    // Add test data

    // Act
    var profile = await context.Profiles
        .FullyPopulatedProfileQuery()
        .FirstOrDefaultAsync();

    // Assert
    Assert.NotNull(profile.User);
    Assert.NotEmpty(profile.ProfileSkills);
}
```
