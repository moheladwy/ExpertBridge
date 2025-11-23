---
applyTo: "**"
---

# ExpertBridge - AI Code Assistant Instructions

## Project Overview

ExpertBridge is a professional networking and knowledge-sharing platform that combines expert hiring, job matching, and community engagement. The project consists of two main components:

-   **Backend (Server):** .NET 10.0 API with Clean Architecture, DDD patterns, and microservices-ready design
-   **Frontend (Client):** React 19 SPA with TypeScript, Vite, Redux Toolkit, and Firebase Auth

**Key Features:**

-   Expert profiles and portfolios
-   Job postings and contractor matching
-   Content creation and sharing (posts, comments)
-   Real-time notifications (SignalR)
-   AI-powered content moderation and tagging
-   Semantic search with vector embeddings
-   Payment processing and dispute resolution
-   Multi-platform support (web, mobile-ready)

---

## Architecture Overview

### Full Stack Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Client (React SPA)                   │
│  React 19 • TypeScript • Redux Toolkit • Firebase Auth │
└─────────────────────┬───────────────────────────────────┘
                      │ HTTPS/REST API
                      │ SignalR WebSocket
┌─────────────────────▼───────────────────────────────────┐
│                  API Gateway (ASP.NET Core)             │
│         JWT Auth • Rate Limiting • CORS • Caching       │
└─────────────────────┬───────────────────────────────────┘
                      │
         ┌────────────┴────────────┬──────────────────┐
         │                         │                  │
┌────────▼─────────┐  ┌───────────▼──────┐  ┌────────▼────────┐
│  API (.NET 10)   │  │  Worker Service  │  │ Admin (Blazor)  │
│  REST Endpoints  │  │  Background Jobs │  │   Dashboard     │
└────────┬─────────┘  └───────────┬──────┘  └────────┬────────┘
         │                        │                   │
         └────────────┬───────────┴───────────────────┘
                      │
         ┌────────────┴────────────┬──────────────────┐
         │                         │                  │
┌────────▼─────────┐  ┌───────────▼──────┐  ┌────────▼────────┐
│   PostgreSQL     │  │    RabbitMQ      │  │      Redis      │
│  + Pgvector      │  │  Message Broker  │  │  Cache + Queue  │
└──────────────────┘  └──────────────────┘  └─────────────────┘
```

### Backend Architecture (Clean Architecture)

```
┌──────────────────────────────────────────────────────────┐
│              Presentation Layer                          │
│  ExpertBridge.Api • ExpertBridge.Admin • Worker          │
└──────────────────┬───────────────────────────────────────┘
                   │
┌──────────────────▼───────────────────────────────────────┐
│              Application Layer                           │
│  ExpertBridge.Application (Services, Business Logic)     │
└──────────────────┬───────────────────────────────────────┘
                   │
┌──────────────────▼───────────────────────────────────────┐
│              Infrastructure Layer                        │
│  ExpertBridge.Extensions (AWS, Caching, MessageBroker)   │
│  ExpertBridge.Data (EF Core, Migrations)                 │
│  ExpertBridge.Notifications (SignalR)                    │
└──────────────────┬───────────────────────────────────────┘
                   │
┌──────────────────▼───────────────────────────────────────┐
│                Domain Layer                              │
│  ExpertBridge.Core (Entities, Interfaces, Domain Logic)  │
│  ExpertBridge.Contract (DTOs, Requests, Responses)       │
└──────────────────────────────────────────────────────────┘
```

### Frontend Architecture (Feature-First)

```
src/
├── app/                    # Store configuration
│   ├── store.ts           # Redux store setup
│   └── hooks.ts           # Typed Redux hooks
├── features/              # Feature modules (RTK Query slices)
│   ├── api/              # Base API slice
│   ├── auth/             # Authentication state
│   ├── posts/            # Posts feature
│   ├── comments/         # Comments feature
│   ├── profiles/         # User profiles
│   ├── jobs/             # Job management
│   └── [other features]
├── views/
│   ├── pages/            # Route components
│   └── components/       # UI components
├── lib/
│   ├── firebase/         # Firebase config
│   ├── services/         # Auth & Token managers
│   └── util/             # Utility functions
├── hooks/                # Custom React hooks
└── routes.tsx            # Route definitions
```

---

## Technology Stack

### Backend Technologies

| Category            | Technology                         | Purpose                             |
| ------------------- | ---------------------------------- | ----------------------------------- |
| **Framework**       | .NET 10.0                          | Core framework                      |
| **Database**        | PostgreSQL + Pgvector              | Primary database with vector search |
| **ORM**             | Entity Framework Core 10           | Data access                         |
| **Cache**           | Redis + FusionCache                | Distributed caching                 |
| **Message Broker**  | RabbitMQ + MassTransit             | Async messaging                     |
| **Auth**            | Firebase Admin SDK                 | JWT validation                      |
| **Real-time**       | SignalR                            | WebSocket notifications             |
| **Background Jobs** | Quartz.NET                         | Scheduled tasks                     |
| **AI/ML**           | Ollama, Groq SDK                   | Embeddings, content moderation      |
| **Storage**         | AWS S3                             | Media files                         |
| **Observability**   | Serilog, OpenTelemetry, Prometheus | Logging, tracing, metrics           |
| **API Docs**        | Scalar (OpenAPI)                   | API documentation                   |
| **Resilience**      | Polly                              | Retry, circuit breaker              |

### Frontend Technologies

| Category          | Technology                  | Purpose                |
| ----------------- | --------------------------- | ---------------------- |
| **Framework**     | React 19                    | UI framework           |
| **Language**      | TypeScript 5.6              | Type safety            |
| **Build Tool**    | Vite 6                      | Fast builds            |
| **Routing**       | React Router v7             | Client-side routing    |
| **State**         | Redux Toolkit + RTK Query   | State management + API |
| **Auth**          | Firebase Auth v11           | Authentication         |
| **Styling**       | Tailwind CSS v4 + shadcn/ui | UI styling             |
| **Notifications** | react-hot-toast             | Toast messages         |
| **Theme**         | next-themes                 | Dark/light mode        |
| **Deployment**    | Docker + Nginx              | Production serving     |

---

## Development Workflow

### General Principles

1. **Follow Clean Architecture:** Dependencies always point inward (Domain ← Application ← Infrastructure ← Presentation)
2. **Domain-Driven Design:** Model business logic in domain entities, not in services
3. **Feature-First Structure:** Organize by feature/domain, not by technical layer
4. **Type Safety:** Use TypeScript/C# generics and interfaces extensively
5. **Async/Await:** All I/O operations must be asynchronous
6. **Logging:** Use structured logging with correlation IDs
7. **Error Handling:** Use global exception handlers, return problem details
8. **Testing:** Write unit tests for business logic, integration tests for APIs
9. **Security:** Validate all inputs, sanitize outputs, use least privilege
10. **Documentation:** Document all public APIs with XML/JSDoc comments

### Code Review Checklist

Before submitting any code, ensure:

-   [ ] **Naming conventions** followed (see sections below)
-   [ ] **Type safety** enforced (no `any` in TypeScript, no raw SQL in C#)
-   [ ] **Error handling** implemented (try-catch, error boundaries)
-   [ ] **Validation** applied (FluentValidation backend, form validation frontend)
-   [ ] **Logging** added for important operations
-   [ ] **Tests** written or updated
-   [ ] **Documentation** added (XML comments backend, JSDoc frontend)
-   [ ] **No secrets** in code (use environment variables)
-   [ ] **Dependencies** up to date
-   [ ] **Linting** passes (`dotnet format`, `npm run lint`)
-   [ ] **Build** succeeds without warnings

---

## Backend Development Guidelines

### Project Structure Standards

```
ExpertBridge.Core/
├── Entities/              # Domain entities
│   ├── Users/
│   ├── Posts/
│   ├── Jobs/
│   └── [Other aggregates]
├── Interfaces/            # Domain interfaces
├── Constraints/           # Entity constraints
└── ValueObjects/          # Value objects

ExpertBridge.Contract/
├── Requests/              # API request DTOs
│   ├── CreatePost/
│   ├── EditComment/
│   └── [Other actions]
├── Responses/             # API response DTOs
├── Messages/              # Message broker contracts
└── Queries/               # Query parameters

ExpertBridge.Application/
├── Services/              # Domain services
├── Settings/              # Configuration classes
└── Validators/            # FluentValidation rules

ExpertBridge.Extensions/
├── AWS/                   # S3 integration
├── Caching/               # FusionCache setup
├── MessageBroker/         # MassTransit config
├── Embeddings/            # AI embeddings
└── [Other infrastructure]
```

### Backend Naming Conventions

| Type            | Convention                  | Example                                     |
| --------------- | --------------------------- | ------------------------------------------- |
| **Entities**    | Singular noun, PascalCase   | `Post`, `Comment`, `User`                   |
| **Collections** | Plural                      | `Posts`, `Comments`                         |
| **Services**    | `{Domain}Service`           | `PostService`, `TaggingService`             |
| **Controllers** | `{Resource}Controller`      | `PostsController`                           |
| **Requests**    | `{Action}{Resource}Request` | `CreatePostRequest`                         |
| **Responses**   | `{Resource}Response`        | `PostResponse`                              |
| **Consumers**   | `{Action}Consumer`          | `PostTaggingConsumer`                       |
| **Extensions**  | `{Feature}.cs`              | `Cors.cs`, `MessageBrokerConfigurations.cs` |
| **Interfaces**  | `I{Name}`                   | `ISoftDeletable`, `IPostService`            |

### Entity Guidelines

```csharp
/// <summary>
/// Represents a post in the platform.
/// </summary>
/// <remarks>
/// Supports AI-powered tagging, NSFW detection, and semantic search via embeddings.
/// </remarks>
public class Post : BaseModel, ISoftDeletable, ISafeContent, IRecommendableContent
{
    /// <summary>
    /// Post title (max 200 characters).
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Post content (max 5000 characters).
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Author of the post.
    /// </summary>
    public virtual User Author { get; set; } = null!;

    /// <summary>
    /// Foreign key to Author.
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Comments on this post.
    /// </summary>
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    // Implement interface properties
    public bool IsDeleted { get; set; }
    public bool IsFlagged { get; set; }
    public Vector? Embedding { get; set; }
}
```

**Key Rules:**

-   All entities inherit from `BaseModel` (provides `Id`, `CreatedAt`, `LastModified`)
-   Use marker interfaces for cross-cutting concerns (`ISoftDeletable`, `ISafeContent`)
-   Navigation properties use `virtual` for lazy loading
-   Collections initialized to empty list
-   XML documentation required for all public members

### Service Guidelines

```csharp
public class PostService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PostService> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public PostService(
        ApplicationDbContext context,
        ILogger<PostService> logger,
        IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<PostResponse> CreateAsync(CreatePostRequest request, Guid userId)
    {
        var post = new Post
        {
            Title = request.Title,
            Content = request.Content,
            AuthorId = userId
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Post {PostId} created by user {UserId}", post.Id, userId);

        // Publish event for async processing
        await _publishEndpoint.Publish(new PostCreated { PostId = post.Id });

        return PostResponseTransformer.Transform(post);
    }
}
```

**Key Rules:**

-   Services are stateless
-   Inject dependencies via constructor
-   Use async/await for all I/O
-   Log important operations
-   Return DTOs, never domain entities
-   Publish domain events for async processing

### Controller Guidelines

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
[ResponseCache(CacheProfileName = CacheProfiles.PersonalizedContent)]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    /// <summary>
    /// Creates a new post.
    /// </summary>
    /// <param name="request">Post creation request.</param>
    /// <returns>Created post.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PostResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
    {
        var userId = User.GetUserId();
        var response = await _postService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }
}
```

**Key Rules:**

-   Apply `[Authorize]` by default, use `[AllowAnonymous]` for exceptions
-   Use `[ResponseCache]` with predefined cache profiles
-   XML documentation for OpenAPI generation
-   Return proper HTTP status codes
-   Controllers orchestrate, don't contain business logic

### Message Broker Patterns

**Publishing Events:**

```csharp
await _publishEndpoint.Publish(new PostCreated
{
    PostId = post.Id,
    Timestamp = DateTime.UtcNow
});
```

**Consuming Messages:**

```csharp
public class PostTaggingConsumer : IConsumer<PostCreated>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PostTaggingConsumer> _logger;

    public async Task Consume(ConsumeContext<PostCreated> context)
    {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITaggingService>();

        try
        {
            _logger.LogInformation("Processing tags for post {PostId}", context.Message.PostId);
            await service.ProcessTagsAsync(context.Message.PostId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process tags for post {PostId}", context.Message.PostId);
            throw; // Rethrow for retry policy
        }
    }
}
```

**Key Rules:**

-   Consumers must be idempotent
-   Use `IServiceScopeFactory` for scoped dependencies
-   Log all processing steps
-   Implement retry policies for transient failures
-   Message types: Commands (imperative), Events (past tense), Queries (request-response)

### Database Migrations

```bash
# Add migration
dotnet ef migrations add MigrationName --project ExpertBridge.Data --startup-project ExpertBridge.Api

# Update database
dotnet ef database update --project ExpertBridge.Data --startup-project ExpertBridge.Api

# Remove last migration
dotnet ef migrations remove --project ExpertBridge.Data --startup-project ExpertBridge.Api
```

**Key Rules:**

-   Use Fluent API for entity configuration (avoid Data Annotations)
-   Apply query filters for soft-deleted entities
-   Index foreign keys and frequently queried columns
-   Use migrations for all schema changes
-   Test migrations in both directions (up and down)

---

## Frontend Development Guidelines

### Component Structure

```
src/views/
├── pages/                      # Route components (default export)
│   ├── auth/
│   │   ├── Login.tsx
│   │   └── SignUp.tsx
│   ├── feed/
│   │   └── HomePage.tsx
│   ├── posts/
│   │   └── PostDetail.tsx
│   └── profile/
│       ├── MyProfile.tsx
│       └── UserProfile.tsx
└── components/                 # Reusable components
    ├── common/                 # Feature components
    │   ├── posts/
    │   │   ├── PostCard.tsx
    │   │   └── PostList.tsx
    │   └── comments/
    │       └── CommentCard.tsx
    ├── custom/                 # Custom UI components
    │   ├── FileUpload.tsx
    │   └── TimeAgo.tsx
    └── ui/                     # shadcn/ui primitives
        ├── button.tsx
        └── card.tsx
```

### Frontend Naming Conventions

| Type           | Convention                  | Example                             |
| -------------- | --------------------------- | ----------------------------------- |
| **Components** | PascalCase                  | `PostCard`, `AuthButtons`           |
| **Hooks**      | camelCase with `use` prefix | `useAuthCheck`, `useRefetchOnLogin` |
| **Utilities**  | camelCase                   | `config.ts`, `utils.ts`             |
| **Types**      | PascalCase                  | `Post`, `PostResponse`              |
| **Slices**     | camelCase with feature      | `postsSlice.ts`, `authSlice.ts`     |
| **Files**      | Match export name           | `PostCard.tsx`, `useAuthCheck.ts`   |

### Component Guidelines

```tsx
import { cn } from "@/lib/util/utils";
import { useAppSelector } from "@/app/hooks";
import { Post } from "@/features/posts/types";

interface PostCardProps {
	post: Post;
	onAction?: () => void;
	className?: string;
}

/**
 * Displays a post card with title, content, and actions.
 */
const PostCard = ({ post, onAction, className }: PostCardProps) => {
	// 1. Hooks first
	const user = useAppSelector((state) => state.auth.user);

	// 2. Event handlers
	const handleClick = () => {
		onAction?.();
	};

	// 3. Render
	return (
		<div className={cn("rounded-lg border bg-card p-4", className)}>
			<h3 className="text-lg font-semibold">{post.title}</h3>
			<p className="text-muted-foreground">{post.content}</p>
			<button onClick={handleClick}>Action</button>
		</div>
	);
};

export default PostCard;
```

**Key Rules:**

-   Use functional components with hooks (no class components)
-   Define props interface above component
-   Order: imports → interface → component
-   Use `cn()` utility for className merging
-   Default export for pages, named export for utilities
-   Add JSDoc for complex components

### RTK Query Patterns

```typescript
import { apiSlice } from "../api/apiSlice";
import { Post, PostResponse, CreatePostRequest } from "./types";
import { postResponseTransformer } from "./transformers";

export const postsApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		getPosts: builder.query<Post[], void>({
			query: () => "/posts",
			transformResponse: (response: PostResponse[]) =>
				response.map(postResponseTransformer),
			providesTags: (result) =>
				result
					? [
							...result.map(({ id }) => ({
								type: "Post" as const,
								id,
							})),
							{ type: "Post", id: "LIST" },
					  ]
					: [{ type: "Post", id: "LIST" }],
		}),
		createPost: builder.mutation<Post, CreatePostRequest>({
			query: (body) => ({
				url: "/posts",
				method: "POST",
				body,
			}),
			invalidatesTags: [{ type: "Post", id: "LIST" }],
		}),
	}),
});

export const { useGetPostsQuery, useCreatePostMutation } = postsApiSlice;
```

**Key Rules:**

-   Extend base `apiSlice` using `injectEndpoints()`
-   Name endpoints: `get[Entity]`, `create[Entity]`, `update[Entity]`, `delete[Entity]`
-   Use `transformResponse` to normalize API data
-   Define `providesTags` and `invalidatesTags` for cache management
-   Export generated hooks

### Authentication Patterns

**NEVER create new Firebase auth listeners.** Always use centralized services:

```typescript
// ✅ CORRECT: Use centralized auth hook
import { useCurrentUser } from "@/lib/services/AuthStateManager";

const MyComponent = () => {
	const user = useCurrentUser();
	// user is the Firebase User object or null
};

// ✅ CORRECT: Use token manager for API calls
import { tokenManager } from "@/lib/services/TokenManager";

const token = await tokenManager.getToken();

// ❌ WRONG: Don't create new listeners
const MyComponent = () => {
	useEffect(() => {
		const unsubscribe = onAuthStateChanged(auth, (user) => {
			// This creates a duplicate listener!
		});
		return unsubscribe;
	}, []);
};
```

**Key Rules:**

-   Use `useCurrentUser()` hook to access Firebase user
-   Use `useAuthReady()` to check if auth is initialized
-   Use `tokenManager.getToken()` for API tokens (cached, auto-refreshes)
-   Never create new `onAuthStateChanged` listeners
-   Protected routes use `<ProtectedRoute>` wrapper

### Styling Patterns

```tsx
import { cn } from "@/lib/util/utils";

// ✅ CORRECT: Use Tailwind classes with cn() utility
<div className={cn(
    "rounded-lg border bg-card p-4",
    isActive && "border-primary",
    className
)}>

// ✅ CORRECT: Use CSS variables from theme
<div className="bg-background text-foreground">

// ❌ WRONG: Don't use inline styles
<div style={{ backgroundColor: "#fff" }}>

// ❌ WRONG: Don't hardcode colors
<div className="bg-white text-black">
```

**Key Rules:**

-   Use Tailwind utility classes
-   Use `cn()` utility for conditional classes
-   Use CSS variables for colors (e.g., `bg-background`, `text-foreground`)
-   Support dark mode via `next-themes`
-   Use shadcn/ui components for complex UI

---

## Configuration Management

### Backend Configuration

**Settings Pattern:**

```csharp
public sealed class FeatureSettings
{
    public const string Section = "FeatureName";
    public string Property { get; set; } = default!;
}

// Registration in Program.cs
builder.Services.Configure<FeatureSettings>(
    builder.Configuration.GetSection(FeatureSettings.Section));

// Usage in services
public class MyService
{
    private readonly FeatureSettings _settings;

    public MyService(IOptions<FeatureSettings> settings)
    {
        _settings = settings.Value;
    }
}
```

**Configuration Sources (Priority Order):**

1. Environment variables
2. User secrets (development)
3. appsettings.{Environment}.json
4. appsettings.json

### Frontend Configuration

**Environment Variables (must be prefixed with `VITE_`):**

```bash
# .env file
VITE_SERVER_URL=http://localhost:5027
VITE_API_KEY="your-firebase-api-key"
VITE_ENABLE_DEBUG_LOGGING=true
```

**Access in Code:**

```typescript
const apiUrl = import.meta.env.VITE_SERVER_URL;
const isDebug = import.meta.env.VITE_ENABLE_DEBUG_LOGGING === "true";
```

**Key Rules:**

-   All env vars must start with `VITE_`
-   Never commit secrets to `.env` files (use `.env.example`)
-   Pass env vars to Docker build via `ARG` in Dockerfile
-   Vite embeds env vars at build time (not runtime)

---

## Security Guidelines

### Authentication & Authorization

**Backend:**

-   Firebase JWT tokens validated via Firebase Admin SDK
-   Use `[Authorize]` attribute on controllers/actions
-   Extract user ID from claims: `User.GetUserId()`
-   Role-based authorization for admin features
-   No anonymous access by default

**Frontend:**

-   Firebase Auth for user login/registration
-   Store tokens securely (handled by Firebase SDK)
-   Protected routes redirect to login if not authenticated
-   Token refresh handled automatically by `TokenManager`

### Input Validation

**Backend:**

-   Use FluentValidation for all request DTOs
-   Validate in Application layer, not Controllers
-   Sanitize user input before storage
-   Use parameterized queries (EF Core does this automatically)

**Frontend:**

-   Validate forms before submission
-   Use HTML5 validation attributes
-   Display validation errors clearly
-   Never trust client-side validation alone

### Content Security

**Backend:**

-   NSFW detection via Groq API for all user content
-   Soft delete for sensitive operations (`ISoftDeletable`)
-   Audit trails for moderation actions
-   Rate limiting on API endpoints

**Frontend:**

-   Sanitize HTML content (use DOMPurify if rendering user HTML)
-   Escape user input in displays
-   Use CSP headers (configured in Nginx)
-   HTTPS only in production

---

## Error Handling

### Backend Error Handling

**Global Exception Middleware:**

-   Catches all unhandled exceptions
-   Returns problem details (RFC 7807)
-   Logs with correlation IDs
-   Maps exception types to HTTP status codes

**Custom Exceptions:**

```csharp
public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string resourceType, Guid id)
        : base($"{resourceType} with ID {id} not found.")
    {
    }
}
```

**Usage in Services:**

```csharp
var post = await _context.Posts.FindAsync(postId);
if (post == null)
{
    throw new ResourceNotFoundException(nameof(Post), postId);
}
```

### Frontend Error Handling

**Error Boundaries:**

```tsx
import { ErrorBoundary } from "@/components/errors/ErrorBoundary";

<ErrorBoundary fallback={<ErrorFallback />}>
	<MyComponent />
</ErrorBoundary>;
```

**API Error Handling:**

```typescript
const { data, error, isLoading } = useGetPostsQuery();

if (isLoading) return <Loader />;
if (error) {
	toast.error("Failed to load posts");
	return <ErrorMessage />;
}
```

**Key Rules:**

-   Wrap all route components in `<ErrorBoundary>`
-   Display user-friendly error messages
-   Log errors for debugging
-   Use toast notifications for transient errors

---

## Testing Guidelines

### Backend Testing

**Unit Tests:**

```csharp
public class PostServiceTests
{
    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsPostResponse()
    {
        // Arrange
        var service = CreateService();
        var request = new CreatePostRequest { Title = "Test", Content = "Content" };

        // Act
        var result = await service.CreateAsync(request, Guid.NewGuid());

        // Assert
        result.ShouldNotBeNull();
        result.Title.ShouldBe("Test");
    }

    private PostService CreateService()
    {
        // Setup mocks and return service
    }
}
```

**Integration Tests:**

```csharp
public class PostsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PostsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ValidPost_Returns201()
    {
        // Arrange
        var request = new CreatePostRequest { Title = "Test", Content = "Content" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/posts", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
```

### Frontend Testing (Future)

**Unit Tests:**

```typescript
import { render, screen } from "@testing-library/react";
import PostCard from "./PostCard";

test("renders post title", () => {
	const post = { id: "1", title: "Test Post", content: "Content" };
	render(<PostCard post={post} />);
	expect(screen.getByText("Test Post")).toBeInTheDocument();
});
```

**Key Rules:**

-   Follow AAA pattern (Arrange, Act, Assert)
-   Use Shouldly for assertions (backend)
-   Mock external dependencies
-   Test business logic, not implementation details
-   Aim for >80% code coverage

---

## Performance Optimization

### Backend Performance

-   **Caching:** Use FusionCache for frequently accessed data (5-minute default)
-   **Database:** Index foreign keys, use `AsNoTracking()` for read-only queries
-   **Bulk Operations:** Use Z.EntityFramework.Extensions for batch inserts/updates
-   **Async Processing:** Offload heavy operations to Worker via message broker
-   **Response Caching:** Use `[ResponseCache]` attribute with cache profiles
-   **Pagination:** Always paginate list endpoints

### Frontend Performance

-   **Code Splitting:** Use `lazyWithRetry()` for route-level components
-   **Memoization:** Use `React.memo()`, `useMemo`, `useCallback` judiciously
-   **RTK Query Caching:** 5-minute cache by default, use `keepUnusedDataFor` to adjust
-   **Token Caching:** `TokenManager` caches tokens for 1 minute
-   **Virtual Scrolling:** Use for large lists (e.g., infinite scroll feeds)
-   **Image Optimization:** Use WebP format, lazy load images

---

## Deployment

### Backend Deployment

**Docker:**

-   Multi-stage builds for optimization
-   Use official .NET images
-   Health checks on `/health` endpoint
-   Environment variables for configuration

**Required Services:**

-   PostgreSQL (with Pgvector extension)
-   Redis
-   RabbitMQ
-   AWS S3 (or compatible)

### Frontend Deployment

**Docker:**

-   Build stage: Node Alpine, `npm ci`, `npm run build`
-   Serve stage: Nginx Alpine, SPA fallback routing
-   Health check on `/health`
-   Gzip compression enabled

**Netlify:**

-   `netlify.toml` configured for SPA routing
-   Environment variables in Netlify dashboard
-   Automatic deployments from Git

---

## Monitoring & Observability

### Backend Monitoring

-   **Logging:** Serilog with structured logging to console, file, OpenTelemetry
-   **Tracing:** OpenTelemetry distributed tracing
-   **Metrics:** Prometheus metrics export
-   **Health Checks:** Database, Redis, RabbitMQ connectivity
-   **Alerts:** Set up alerts for 5xx errors, high latency, dependency failures

### Frontend Monitoring

-   **Error Tracking:** Use Sentry or similar (not yet implemented)
-   **Performance:** Use Web Vitals tracking (not yet implemented)
-   **Analytics:** Google Analytics or PostHog (not yet implemented)
-   **Logging:** Console logs in development, structured logs in production

---

## Common Patterns

### CQRS (Informal)

-   Read models (responses) differ from write models (requests)
-   Queries optimized separately from commands
-   No formal CQRS framework, but principle applied

### Event-Driven Architecture

-   Domain events published via MassTransit/RabbitMQ
-   Consumers process events asynchronously
-   Sagas for multi-step workflows

### Repository Pattern

-   Not explicitly used in backend (services use DbContext directly)
-   Abstractions added where needed for testability

### Facade Pattern

-   `NotificationFacade` abstracts SignalR complexity
-   `TokenManager` abstracts Firebase token management

---

## Common Pitfalls & Solutions

### Backend Pitfalls

1. **N+1 Queries:** Use `Include()` or projections to eager load related data
2. **Missing Migrations:** Always create migrations for schema changes
3. **Blocking Async Code:** Never use `.Result` or `.Wait()`, always `await`
4. **Large Payloads:** Use pagination and compression
5. **No Retry Logic:** Use Polly for transient failures

### Frontend Pitfalls

1. **Multiple Auth Listeners:** Always use `useCurrentUser()`, never create new listeners
2. **Token Refresh:** Use `tokenManager.getToken()`, not direct Firebase calls
3. **Chunk Load Failures:** Use `lazyWithRetry()` for route components
4. **API Health:** Check API health on app mount
5. **Missing Error Boundaries:** Wrap all routes in `<ErrorBoundary>`

---

## Development Commands

### Backend Commands

```bash
# Build
dotnet build

# Run API
dotnet run --project ExpertBridge.Api

# Run Worker
dotnet run --project ExpertBridge.Worker

# Run Host (Aspire)
dotnet run --project ExpertBridge.Host --launch-profile http

# Run tests
dotnet test

# Format code
dotnet format

# Create migration
dotnet ef migrations add MigrationName --project ExpertBridge.Data --startup-project ExpertBridge.Api
```

### Frontend Commands

```bash
# Development
npm run dev              # Start Vite dev server (port 5173)

# Build
npm run build            # Production build

# Preview
npm run preview          # Preview production build

# Linting
npm run lint             # Run ESLint

# Docker
docker-compose up --build   # Build and run
docker-compose down        # Stop containers
```

---

## Quick Reference

### When to Use What

| Task                 | Backend                      | Frontend                  |
| -------------------- | ---------------------------- | ------------------------- |
| **State Management** | EF Core + DbContext          | Redux Toolkit + RTK Query |
| **API Calls**        | HttpClient (external APIs)   | RTK Query                 |
| **Validation**       | FluentValidation             | Form libraries + HTML5    |
| **Caching**          | FusionCache (Redis + Memory) | RTK Query cache           |
| **Background Jobs**  | MassTransit + Quartz         | N/A                       |
| **Real-time**        | SignalR                      | SignalR client            |
| **Auth**             | Firebase Admin SDK           | Firebase Auth             |
| **Logging**          | Serilog                      | console.log (dev)         |
| **Error Handling**   | Exception middleware         | Error boundaries          |

### Import Aliases

**Backend:** N/A (use full namespaces)

**Frontend:**

-   `@/` → `./src/`
-   `@assets/` → `./src/assets/`
-   `@views/` → `./src/views/`

---

## Additional Resources

### Backend Resources

-   [ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core)
-   [Entity Framework Core Docs](https://docs.microsoft.com/ef/core)
-   [MassTransit Docs](https://masstransit-project.com/)
-   [Clean Architecture by Jason Taylor](https://github.com/jasontaylordev/CleanArchitecture)

### Frontend Resources

-   [React Docs](https://react.dev/)
-   [Redux Toolkit Docs](https://redux-toolkit.js.org/)
-   [RTK Query Docs](https://redux-toolkit.js.org/rtk-query/overview)
-   [React Router v7 Docs](https://reactrouter.com/)
-   [Tailwind CSS Docs](https://tailwindcss.com/)
-   [shadcn/ui Docs](https://ui.shadcn.com/)

---

## Project Contacts & Links

-   **Repository:** moheladwy/ExpertBridge
-   **Branch:** main
-   **Documentation:** `/docs` directory
-   **Architecture Diagrams:** `/docs/ArchitectureDiagrams`
-   **Sequence Diagrams:** `/docs/SequenceDiagrams`

---

**Last Updated:** November 23, 2025
**Version:** 1.0.0
**Maintained By:** ExpertBridge Development Team
