# ExpertBridge.Admin - GitHub Copilot Instructions

## Project Purpose

Blazor Server admin dashboard for managing and monitoring the ExpertBridge platform. Provides administrative tools for content moderation, user management, and system monitoring.

## Architecture Role

**Administrative UI** - Separate web application for administrative tasks, built with Blazor Server for real-time updates and server-side rendering.

## Key Responsibilities

-   Admin dashboard and analytics
-   Content moderation (posts, comments)
-   User management
-   System monitoring
-   Data visualization
-   Real-time updates via SignalR

## Project Dependencies

```xml
Dependencies:
- ExpertBridge.Data (database access)
- ExpertBridge.Extensions (infrastructure)

External:
- Blazor Server components
- FusionCache
- Serilog
```

## Blazor Component Structure

### Page Component Template

```razor
@page "/deleted-posts"
@using ExpertBridge.Data.DatabaseContexts
@using ExpertBridge.Core.Entities.Posts
@inject ExpertBridgeDbContext DbContext
@inject ILogger<DeletedPosts> Logger

<PageTitle>Deleted Posts</PageTitle>

<h1>Deleted Posts Management</h1>

@if (posts == null)
{
    <p><em>Loading...</em></p>
}
else if (!posts.Any())
{
    <p><em>No deleted posts found.</em></p>
}
else
{
    <table class="table">
        <thead>
            <tr>
                <th>ID</th>
                <th>Author</th>
                <th>Content</th>
                <th>Deleted At</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var post in posts)
            {
                <tr>
                    <td>@post.Id</td>
                    <td>@post.Profile?.Username</td>
                    <td>@post.Content?.Substring(0, Math.Min(50, post.Content.Length ?? 0))...</td>
                    <td>@post.DeletedAt?.ToString("g")</td>
                    <td>
                        <button class="btn btn-sm btn-primary" @onclick="() => RestorePost(post.Id)">
                            Restore
                        </button>
                        <button class="btn btn-sm btn-danger" @onclick="() => PermanentlyDeletePost(post.Id)">
                            Delete Permanently
                        </button>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private List<Post>? posts;

    protected override async Task OnInitializedAsync()
    {
        await LoadDeletedPostsAsync();
    }

    private async Task LoadDeletedPostsAsync()
    {
        try
        {
            // Ignore query filter to get soft-deleted items
            posts = await DbContext.Posts
                .IgnoreQueryFilters()
                .Where(p => p.IsDeleted)
                .Include(p => p.Profile)
                .OrderByDescending(p => p.DeletedAt)
                .Take(100)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load deleted posts");
        }
    }

    private async Task RestorePost(string postId)
    {
        try
        {
            var post = await DbContext.Posts
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post != null)
            {
                post.IsDeleted = false;
                post.DeletedAt = null;
                await DbContext.SaveChangesAsync();

                Logger.LogInformation("Post {PostId} restored by admin", postId);
                await LoadDeletedPostsAsync(); // Refresh list
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to restore post {PostId}", postId);
        }
    }

    private async Task PermanentlyDeletePost(string postId)
    {
        try
        {
            var post = await DbContext.Posts
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post != null)
            {
                DbContext.Posts.Remove(post);
                await DbContext.SaveChangesAsync();

                Logger.LogWarning("Post {PostId} permanently deleted by admin", postId);
                await LoadDeletedPostsAsync(); // Refresh list
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to permanently delete post {PostId}", postId);
        }
    }
}
```

### Component with Form

```razor
@page "/user-management"
@using ExpertBridge.Data.DatabaseContexts
@using ExpertBridge.Core.Entities.Users
@inject ExpertBridgeDbContext DbContext
@inject ILogger<UserManagement> Logger

<PageTitle>User Management</PageTitle>

<h1>User Management</h1>

<div class="row mb-3">
    <div class="col-md-6">
        <input type="text" class="form-control" placeholder="Search by email or username"
               @bind="searchTerm" @bind:event="oninput" />
    </div>
    <div class="col-md-2">
        <button class="btn btn-primary" @onclick="SearchUsers">Search</button>
    </div>
</div>

@if (users == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <table class="table table-striped">
        <thead>
            <tr>
                <th>Email</th>
                <th>Username</th>
                <th>Status</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var user in users)
            {
                <tr class="@(user.IsBanned ? "table-danger" : "")">
                    <td>@user.Email</td>
                    <td>@user.Username</td>
                    <td>
                        @if (user.IsBanned) { <span class="badge bg-danger">Banned</span> }
                        else if (!user.IsEmailVerified) { <span class="badge bg-warning">Unverified</span> }
                        else { <span class="badge bg-success">Active</span> }
                    </td>
                    <td>
                        @if (!user.IsBanned)
                        {
                            <button class="btn btn-sm btn-warning" @onclick="() => BanUser(user.Id)">
                                Ban
                            </button>
                        }
                        else
                        {
                            <button class="btn btn-sm btn-success" @onclick="() => UnbanUser(user.Id)">
                                Unban
                            </button>
                        }
                    </td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private List<User>? users;
    private string searchTerm = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        users = await DbContext.Users
            .OrderByDescending(u => u.CreatedAt)
            .Take(50)
            .ToListAsync();
    }

    private async Task SearchUsers()
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            await LoadUsersAsync();
            return;
        }

        users = await DbContext.Users
            .Where(u => u.Email.Contains(searchTerm) || u.Username.Contains(searchTerm))
            .Take(50)
            .ToListAsync();
    }

    private async Task BanUser(string userId)
    {
        var user = await DbContext.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsBanned = true;
            await DbContext.SaveChangesAsync();
            Logger.LogWarning("User {UserId} banned by admin", userId);
            await LoadUsersAsync();
        }
    }

    private async Task UnbanUser(string userId)
    {
        var user = await DbContext.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsBanned = false;
            await DbContext.SaveChangesAsync();
            Logger.LogInformation("User {UserId} unbanned by admin", userId);
            await LoadUsersAsync();
        }
    }
}
```

## Program.cs Configuration

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Admin.Components;
using ExpertBridge.Data;
using ExpertBridge.Extensions.Caching;
using ExpertBridge.Extensions.CORS;
using ExpertBridge.Extensions.HealthChecks;
using ExpertBridge.Extensions.Logging;
using ExpertBridge.Extensions.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDatabase(builder.Configuration);

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

// Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseCors(CorsPolicyNames.AllowAll);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapDefaultEndpoints();

app.Run();
```

## Navigation Menu

```razor
<!-- Components/Layout/NavMenu.razor -->
<div class="top-row ps-3 navbar navbar-dark">
    <div class="container-fluid">
        <a class="navbar-brand" href="">Expert Bridge Admin Site</a>
    </div>
</div>

<input type="checkbox" title="Navigation menu" class="navbar-toggler"/>

<div class="nav-scrollable" onclick="document.querySelector('.navbar-toggler').click()">
    <nav class="nav flex-column">
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="" Match="NavLinkMatch.All">
                <span class="bi bi-house-door-fill-nav-menu" aria-hidden="true"></span> Home
            </NavLink>
        </div>

        <div class="nav-item px-3">
            <NavLink class="nav-link" href="/deleted-posts">
                <span class="bi bi-plus-square-fill-nav-menu" aria-hidden="true"></span> Deleted Posts
            </NavLink>
        </div>

        <div class="nav-item px-3">
            <NavLink class="nav-link" href="/deleted-comments">
                <span class="bi bi-plus-square-fill-nav-menu" aria-hidden="true"></span> Deleted Comments
            </NavLink>
        </div>

        <div class="nav-item px-3">
            <NavLink class="nav-link" href="/post-tags">
                <span class="bi bi-plus-square-fill-nav-menu" aria-hidden="true"></span> All Posts Tags
            </NavLink>
        </div>
    </nav>
</div>
```

## ViewModels Pattern

Create ViewModels folder for complex data structures:

```csharp
// ViewModels/DashboardStatistics.cs
namespace ExpertBridge.Admin.ViewModels;

public class DashboardStatistics
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int BannedUsers { get; set; }
    public int TotalPosts { get; set; }
    public int DeletedPosts { get; set; }
    public int TotalComments { get; set; }
    public int PendingReports { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
```

## Best Practices

1. **Use @inject for dependency injection** - Clean and declarative
2. **Leverage cascading parameters** - Share data across component hierarchy
3. **Use EventCallback for component communication** - Type-safe events
4. **Implement loading states** - Show user feedback during async operations
5. **Handle exceptions gracefully** - Log errors and show user-friendly messages
6. **Use IgnoreQueryFilters** - For admin operations on soft-deleted data
7. **Log administrative actions** - Audit trail for security
8. **Validate permissions** - Ensure only authorized users access admin features
9. **Use interactive server rendering** - For real-time admin dashboard
10. **Optimize database queries** - Admin dashboards can query large datasets

## Anti-Patterns to Avoid

-   ❌ Don't put business logic in Razor components
-   ❌ Don't perform heavy operations in OnInitializedAsync without loading indicators
-   ❌ Don't expose sensitive data without proper authorization
-   ❌ Don't forget to dispose of DbContext and services
-   ❌ Don't use synchronous database calls
-   ❌ Don't modify production data without confirmation dialogs
-   ❌ Don't forget to refresh UI after data modifications
-   ❌ Don't hardcode connection strings or secrets
-   ❌ Don't skip logging of administrative actions
-   ❌ Don't allow permanent deletion without safeguards
