# ExpertBridge.Data - GitHub Copilot Instructions

## Project Purpose

Data access layer responsible for Entity Framework Core configuration, database context, migrations, and data persistence logic.

## Architecture Role

**Infrastructure/Persistence Layer** - Handles all database operations, entity configurations, and data access patterns.

## Key Responsibilities

-   EF Core DbContext implementation
-   Database migrations
-   Entity type configurations
-   Query interceptors
-   Database seeding
-   Connection management
-   Change tracking configuration

## Project Dependencies

```xml
Dependencies:
- ExpertBridge.Core (entities, interfaces)

External:
- Microsoft.EntityFrameworkCore
- Npgsql.EntityFrameworkCore.PostgreSQL
- Pgvector.EntityFrameworkCore
```

## DbContext Pattern

### Main DbContext Implementation

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ExpertBridge.Data.DatabaseContexts;

public sealed class ExpertBridgeDbContext : DbContext
{
    public ExpertBridgeDbContext(DbContextOptions<ExpertBridgeDbContext> options)
        : base(options)
    {
        // Automatically update timestamps on tracked entities
        ChangeTracker.Tracked += UpdateTimestamps;
        ChangeTracker.StateChanged += UpdateTimestamps;
    }

    // DbSets for all entities
    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobPosting> JobPostings { get; set; }
    // ... additional DbSets

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Register Pgvector extension
        modelBuilder.HasPostgresExtension("vector");

        // Apply all entity configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ExpertBridgeDbContext).Assembly);

        // Apply entity configurations from Core assembly
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(BaseModel).Assembly);
    }

    /// <summary>
    /// Automatically updates CreatedAt and LastModified timestamps.
    /// </summary>
    private static void UpdateTimestamps(object? sender, EntityEntryEventArgs e)
    {
        if (e.Entry.Entity is not ITimestamped entity)
        {
            return;
        }

        var now = DateTime.UtcNow;

        switch (e.Entry.State)
        {
            case EntityState.Added:
                entity.CreatedAt = now;
                entity.LastModified = now;
                break;

            case EntityState.Modified:
                entity.LastModified = now;
                break;
        }
    }
}
```

## Interceptors Pattern

### Soft Delete Interceptor

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ExpertBridge.Data.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is null)
        {
            return result;
        }

        HandleSoftDeletes(eventData.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
        {
            return new ValueTask<InterceptionResult<int>>(result);
        }

        HandleSoftDeletes(eventData.Context);
        return new ValueTask<InterceptionResult<int>>(result);
    }

    private static void HandleSoftDeletes(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry is not { State: EntityState.Deleted, Entity: ISoftDeletable entity })
            {
                continue;
            }

            // Convert hard delete to soft delete
            entry.State = EntityState.Modified;
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
    }
}
```

## Extension Methods

### Service Registration

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpertBridge.Data;

public static class Extensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ExpertBridgeDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                {
                    npgsqlOptions.UseVector(); // Enable Pgvector
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                })
                .EnableSensitiveDataLogging(false) // Disable in production
                .EnableDetailedErrors(false) // Disable in production
                .AddInterceptors(
                    serviceProvider.GetRequiredService<SoftDeleteInterceptor>());
        });

        // Register interceptors
        services.AddScoped<SoftDeleteInterceptor>();

        return services;
    }

    public static async Task ApplyMigrationAtStartup(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ExpertBridgeDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }
    }
}
```

## Migration Management

### Create Migration

```bash
# From project root
dotnet ef migrations add MigrationName \
    --project ExpertBridge.Data \
    --startup-project ExpertBridge.Api \
    --context ExpertBridgeDbContext
```

### Update Database

```bash
dotnet ef database update \
    --project ExpertBridge.Data \
    --startup-project ExpertBridge.Api \
    --context ExpertBridgeDbContext
```

### Remove Last Migration

```bash
dotnet ef migrations remove \
    --project ExpertBridge.Data \
    --startup-project ExpertBridge.Api \
    --context ExpertBridge.Data.DatabaseContexts.ExpertBridgeDbContext
```

### Generate SQL Script

```bash
dotnet ef migrations script \
    --project ExpertBridge.Data \
    --startup-project ExpertBridge.Api \
    --context ExpertBridgeDbContext \
    --output last-version.sql
```

## Entity Configuration Examples

### Simple Entity Configuration

```csharp
using ExpertBridge.Core.Entities.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Tags;

public class TagEntityConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.Name).IsUnique();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);
    }
}
```

### Complex Entity Configuration with Relationships

```csharp
using ExpertBridge.Core.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Profiles;

public class ProfileEntityConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("Profiles");

        builder.HasKey(p => p.Id);

        // Unique indexes
        builder.HasIndex(p => p.Username).IsUnique();
        builder.HasIndex(p => p.Email).IsUnique();
        builder.HasIndex(p => p.PhoneNumber).IsUnique();

        // Required fields
        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(ProfileEntityConstraints.EmailMaxLength);

        builder.Property(p => p.Username)
            .IsRequired()
            .HasMaxLength(ProfileEntityConstraints.UsernameMaxLength);

        // Optional fields
        builder.Property(p => p.Bio)
            .HasMaxLength(ProfileEntityConstraints.BioMaxLength);

        builder.Property(p => p.JobTitle)
            .HasMaxLength(ProfileEntityConstraints.JobTitleMaxLength);

        // Relationships
        builder.HasOne(p => p.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<Profile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Posts)
            .WithOne(post => post.Profile)
            .HasForeignKey(post => post.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Comments)
            .WithOne(c => c.Profile)
            .HasForeignKey(c => c.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Vector column for embeddings
        builder.Property(p => p.UserInterestEmbedding)
            .HasColumnType("vector(1024)");

        // Soft delete query filter
        builder.HasQueryFilter(p => !p.IsDeleted);

        // Default values
        builder.Property(p => p.Rating)
            .HasDefaultValue(0.0);

        builder.Property(p => p.RatingCount)
            .HasDefaultValue(0);
    }
}
```

### Many-to-Many Configuration

```csharp
using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkills;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkills;

public class ProfileSkillEntityConfiguration : IEntityTypeConfiguration<ProfileSkill>
{
    public void Configure(EntityTypeBuilder<ProfileSkill> builder)
    {
        builder.ToTable("ProfileSkills");

        // Composite primary key
        builder.HasKey(ps => new { ps.ProfileId, ps.SkillId });

        // Relationships
        builder.HasOne(ps => ps.Profile)
            .WithMany(p => p.ProfileSkills)
            .HasForeignKey(ps => ps.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ps => ps.Skill)
            .WithMany(s => s.ProfileSkills)
            .HasForeignKey(ps => ps.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for reverse lookups
        builder.HasIndex(ps => ps.SkillId);
    }
}
```

## Database Seeding

### Seed Data Configuration

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpertBridge.Core.Entities.Skills;

public class SkillEntityConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("Skills");

        builder.HasKey(s => s.Id);

        builder.HasIndex(s => s.Name).IsUnique();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Seed data
        builder.HasData(
            new Skill { Id = "1", Name = "C#", CreatedAt = DateTime.UtcNow },
            new Skill { Id = "2", Name = "Python", CreatedAt = DateTime.UtcNow },
            new Skill { Id = "3", Name = "JavaScript", CreatedAt = DateTime.UtcNow },
            new Skill { Id = "4", Name = "SQL", CreatedAt = DateTime.UtcNow },
            new Skill { Id = "5", Name = "Docker", CreatedAt = DateTime.UtcNow }
        );
    }
}
```

## Query Performance Optimization

### Use Compiled Queries for Frequent Operations

```csharp
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Data.DatabaseContexts;

public sealed class ExpertBridgeDbContext : DbContext
{
    // Compiled query - evaluated once, cached, reused
    private static readonly Func<ExpertBridgeDbContext, string, Task<User?>> GetUserByEmailQuery =
        EF.CompileAsyncQuery((ExpertBridgeDbContext context, string email) =>
            context.Users
                .Include(u => u.Profile)
                .FirstOrDefault(u => u.Email == email));

    public Task<User?> GetUserByEmailAsync(string email)
    {
        return GetUserByEmailQuery(this, email);
    }
}
```

## Connection Resilience

### Configure Retry Logic

```csharp
services.AddDbContext<ExpertBridgeDbContext>(options =>
{
    options.UseNpgsql(
        connectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: new[] { "40001", "40P01" }); // PostgreSQL deadlock codes

            npgsqlOptions.CommandTimeout(30);
            npgsqlOptions.MigrationsAssembly("ExpertBridge.Data");
        });
});
```

## Best Practices

1. **Use file-scoped namespaces** - Matches solution style
2. **Apply configurations from assembly** - Automatic discovery
3. **Use interceptors for cross-cutting concerns** - Soft delete, auditing, etc.
4. **Enable Pgvector** - For AI/ML embeddings
5. **Configure query filters** - For soft deletes and multi-tenancy
6. **Use migrations** - Never modify database schema manually
7. **Seed reference data** - Via entity configurations
8. **Configure indexes** - For frequently queried columns
9. **Set max lengths** - Prevent unbounded strings
10. **Use appropriate delete behaviors** - Cascade, SetNull, Restrict

## Anti-Patterns to Avoid

-   ❌ Don't put business logic in DbContext
-   ❌ Don't use lazy loading (explicit Include is better)
-   ❌ Don't expose DbSet properties as settable
-   ❌ Don't perform raw SQL unless absolutely necessary
-   ❌ Don't forget to configure relationships
-   ❌ Don't ignore migration history
-   ❌ Don't use default conventions for everything (be explicit)
-   ❌ Don't forget query filters for soft deletes
-   ❌ Don't create migrations without reviewing SQL
-   ❌ Don't deploy without testing migrations

## Testing with In-Memory Database

```csharp
using Microsoft.EntityFrameworkCore;
using Xunit;

public class DbContextTests
{
    [Fact]
    public async Task CanAddAndRetrieveUser()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ExpertBridgeDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        // Act
        using (var context = new ExpertBridgeDbContext(options))
        {
            var user = new User
            {
                Email = "test@example.com",
                Username = "testuser",
                ProviderId = "firebase123"
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = new ExpertBridgeDbContext(options))
        {
            var user = await context.Users.FirstOrDefaultAsync();
            Assert.NotNull(user);
            Assert.Equal("test@example.com", user.Email);
        }
    }
}
```
