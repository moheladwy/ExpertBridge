# ExpertBridge Server

## Overview

ExpertBridge is an enterprise-grade professional networking and knowledge-sharing platform built with .NET 10.0. It combines social networking features (profiles, posts, comments, tags, messaging, notifications) with a freelance marketplace (job postings, applications, offers, reviews) and advanced AI-powered capabilities (automatic content tagging, content moderation, semantic search, and intelligent recommendations).

The solution follows Clean Architecture principles with Domain-Driven Design (DDD), featuring strong typing, async-first patterns, comprehensive observability, and distributed system capabilities. It can run locally via the .NET CLI, in Docker containers (with pgvector for semantic embeddings), or as a fully orchestrated distributed application using .NET Aspire.

## Architecture

### Architectural Style

-   **Clean Architecture** with layered separation (Domain, Application, Infrastructure, Presentation)
-   **Domain-Driven Design (DDD)** patterns with aggregate roots and rich domain models
-   **Event-Driven Architecture** using message broker for async workflows
-   Cross-cutting concerns isolated in extension libraries

### Key Components

-   **Orchestration**: .NET Aspire AppHost manages API, Admin, Worker, and infrastructure dependencies (RabbitMQ, Redis, Seq, Ollama)
-   **Persistence**: PostgreSQL with pgvector extension for vector embeddings; EF Core 10.0 with retry policies, soft-delete interceptors, and global query filters
-   **Messaging**: MassTransit over RabbitMQ for event-driven async pipelines and background processing
-   **Real-time**: SignalR hub for live notifications and messaging
-   **Caching**: FusionCache with hybrid (memory + Redis distributed) caching strategy
-   **Observability**: OpenTelemetry (traces, metrics, logs) with Prometheus exporter, OTLP support, and Serilog with Seq sink
-   **Authentication**: Firebase Admin SDK for JWT token validation
-   **Media Storage**: AWS S3 for file uploads and media assets
-   **AI/ML**: Microsoft.Extensions.AI with Ollama for embeddings, Groq SDK for LLM-powered content analysis

### Dependency Flow

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ Api (REST)   │  │ Admin (UI)   │  │ Worker (Jobs)│  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└────────────────────────┬────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────┐
│                   Application Layer                      │
│  ┌──────────────────────────────────────────────────┐   │
│  │  Services (Firebase, S3, Tagging, AI Detection) │   │
│  └──────────────────────────────────────────────────┘   │
└────────────────────────┬────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────┐
│                     Domain Layer                         │
│  ┌──────────────────────────────────────────────────┐   │
│  │  Core (Entities, Interfaces, Domain Logic)       │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                         ▲
┌────────────────────────┴────────────────────────────────┐
│                  Infrastructure Layer                    │
│  ┌──────────┐  ┌──────────┐  ┌────────────────────┐    │
│  │   Data   │  │Extensions│  │   Notifications    │    │
│  └──────────┘  └──────────┘  └────────────────────┘    │
└─────────────────────────────────────────────────────────┘
```

**Clean Architecture Layers:**

1. **Domain (Core)**: Pure business entities, interfaces, domain logic - no external dependencies
2. **Application**: Use cases, application services, business workflows
3. **Infrastructure**: Data access (EF Core), external integrations (AWS, Firebase, Groq), cross-cutting concerns
4. **Presentation**: REST API, Admin dashboard, Background workers

## Project Structure

```
ExpertBridge.sln
├─ ExpertBridge.Api/                   # ASP.NET Core Web API - Public REST endpoints
│  ├─ Controllers/                     # REST API controllers (Posts, Comments, Jobs, Profiles, etc.)
│  ├─ Extensions/                      # API-specific extensions and configuration
│  ├─ Filters/                         # Validation filters and action filters
│  ├─ Middleware/                      # Global exception handling, request logging
│  ├─ Services/                        # API-layer services
│  ├─ Program.cs                       # API bootstrapping with middleware pipeline
│  ├─ appsettings.json                 # Configuration for API
│  └─ Dockerfile                       # Container image definition
│
├─ ExpertBridge.Admin/                 # Blazor Server - Admin dashboard
│  ├─ Components/                      # Blazor components (Pages, Layout, Account)
│  │  ├─ Pages/                        # Admin pages for moderation and management
│  │  ├─ Layout/                       # Shared layout components
│  │  └─ Account/                      # Identity management components
│  ├─ Database/                        # Admin-specific DbContext
│  ├─ Services/                        # Admin services (moderation, reports)
│  ├─ Program.cs                       # Blazor Server host setup
│  └─ Dockerfile
│
├─ ExpertBridge.Application/           # Application Layer - Business logic
│  ├─ Services/                        # Domain services (Firebase, S3, Tagging)
│  ├─ Settings/                        # Configuration classes (NsfwThresholds, etc.)
│  ├─ DataGenerator/                   # Seed data generators
│  ├─ EmbeddingService/                # Vector embedding services
│  ├─ Helpers/                         # Utility helpers
│  ├─ Models/                          # Application DTOs and models
│  └─ LlmOutputFormat/                 # JSON schemas for AI structured outputs
│
├─ ExpertBridge.Core/                  # Domain Layer - Entities and interfaces
│  ├─ Entities/                        # Domain entities organized by aggregate
│  │  ├─ Posts/, Comments/             # Content aggregates
│  │  ├─ Profiles/, Users/             # User aggregates
│  │  ├─ Jobs/, JobPostings/           # Job marketplace aggregates
│  │  ├─ Messages/, Chats/             # Messaging aggregates
│  │  ├─ Notifications/                # Notification entities
│  │  ├─ Tags/, Skills/                # Taxonomy aggregates
│  │  └─ BaseModel.cs                  # Base entity with Id, timestamps
│  ├─ Interfaces/                      # Domain interfaces (ISoftDeletable, ISafeContent, etc.)
│  ├─ Exceptions/                      # Custom domain exceptions
│  └─ EntityConfiguration/             # Shared constraints
│
├─ ExpertBridge.Data/                  # Infrastructure - Data access
│  ├─ DatabaseContexts/                # EF Core DbContext
│  ├─ Interceptors/                    # EF Core interceptors (soft-delete, timestamps)
│  ├─ Migrations/                      # EF Core migrations
│  └─ Extensions.cs                    # Database DI registration
│
├─ ExpertBridge.Extensions/            # Infrastructure - Cross-cutting concerns
│  ├─ AWS/                             # S3 service integration
│  ├─ Caching/                         # FusionCache configuration
│  ├─ CORS/                            # CORS policy definitions
│  ├─ Embeddings/                      # Ollama embedding service setup
│  ├─ HealthChecks/                    # Health check configurations
│  ├─ Logging/                         # Serilog setup and enrichers
│  ├─ MessageBroker/                   # MassTransit/RabbitMQ configuration
│  ├─ OpenTelemetry/                   # Telemetry and metrics
│  └─ Resilience/                      # Polly resilience policies
│
├─ ExpertBridge.Notifications/         # Infrastructure - Real-time notifications
│  ├─ NotificationsHub.cs              # SignalR hub (/api/notificationsHub)
│  ├─ NotificationFacade.cs            # Notification abstraction layer
│  └─ Extensions/                      # SignalR registration extensions
│
├─ ExpertBridge.Contract/              # Shared contracts - DTOs
│  ├─ Requests/                        # API request DTOs (by feature)
│  │  ├─ CreatePost/, EditPost/        # Post operations
│  │  ├─ CreateComment/, EditComment/  # Comment operations
│  │  ├─ CreateJobPosting/             # Job operations
│  │  └─ ...                           # Other request contracts
│  ├─ Responses/                       # API response DTOs
│  ├─ Messages/                        # Message broker contracts
│  └─ Queries/                         # Query parameter objects
│
├─ ExpertBridge.Worker/                # Background processing service
│  ├─ Consumers/                       # MassTransit message consumers
│  │  ├─ PostProcessingPipelineConsumer.cs
│  │  ├─ PostTaggingConsumer.cs
│  │  ├─ InappropriateContentDetectionConsumer.cs
│  │  ├─ PostEmbeddingConsumer.cs
│  │  ├─ NotificationSendingPipelineConsumer.cs
│  │  └─ UserInterestsUpdatedConsumer.cs
│  ├─ Services/                        # Worker-specific services
│  ├─ PeriodicJobs/                    # Quartz scheduled jobs
│  ├─ QuartzDatabase/                  # Quartz persistent store setup
│  ├─ LlmOutputFormat/                 # AI output schemas
│  ├─ Program.cs                       # Worker host builder
│  └─ Dockerfile
│
├─ ExpertBridge.Host/                  # .NET Aspire AppHost - Orchestration
│  ├─ Program.cs                       # Infrastructure and project wiring
│  ├─ manifest.json                    # Aspire deployment manifest
│  └─ Resources/                       # Aspire resource configurations
│
├─ ExpertBridge.Tests.Unit/            # Unit tests
│  └─ Contract/                        # Contract validation tests
│
├─ compose.yaml                        # Docker Compose - All services
├─ postgresql-compose.yaml             # Docker Compose - PostgreSQL + pgAdmin
├─ Directory.Packages.props            # Central package version management
├─ Directory.Build.props               # Global build settings (net10.0, nullable)
└─ global.json                         # .NET SDK version (10.0.0)
```

### Project Roles and Responsibilities

#### **ExpertBridge.Api** (Presentation Layer)

-   **Purpose**: Public REST API serving mobile and web clients
-   **Responsibilities**:
    -   HTTP endpoints for all user-facing operations
    -   JWT authentication via Firebase Admin SDK
    -   Request validation and response formatting
    -   OpenAPI documentation via Scalar.AspNetCore
    -   Health checks and monitoring endpoints
    -   Response caching with cache profiles
    -   Global exception handling
-   **Dependencies**: Application, Core, Data, Extensions, Notifications, Contract
-   **Key Features**:
    -   Automatic EF migrations in Development
    -   Prometheus metrics endpoint
    -   CORS policies for web clients
    -   Serilog request logging with OpenTelemetry

#### **ExpertBridge.Admin** (Presentation Layer)

-   **Purpose**: Blazor Server admin dashboard for moderation and operations
-   **Responsibilities**:
    -   Content moderation interface
    -   User management
    -   System monitoring and reporting
    -   Admin authentication via ASP.NET Core Identity
-   **Dependencies**: Core, Data, Extensions
-   **Key Features**:
    -   Radzen UI component library
    -   Real-time updates via SignalR
    -   Health checks dashboard
    -   Admin-specific database context

#### **ExpertBridge.Application** (Application Layer)

-   **Purpose**: Business logic orchestration and application services
-   **Responsibilities**:
    -   Domain service implementations (Firebase auth, S3 uploads, tagging)
    -   AI service coordination (NSFW detection, auto-tagging, embeddings)
    -   Data transformation and validation
    -   Business workflow coordination
-   **Dependencies**: Core, Data, Extensions, Notifications, Contract
-   **Key Features**:
    -   FluentValidation integration
    -   LLM output format schemas for structured AI responses
    -   Bulk operations via Z.EntityFramework.Extensions
    -   Service layer abstractions

#### **ExpertBridge.Core** (Domain Layer)

-   **Purpose**: Pure domain model with business entities and rules
-   **Responsibilities**:
    -   Domain entities with rich behavior
    -   Domain interfaces and contracts
    -   Business rule enforcement
    -   Domain exceptions
-   **Dependencies**: EF Core abstractions, Pgvector (minimal)
-   **Key Features**:
    -   BaseModel with Id, CreatedAt, LastModified
    -   Marker interfaces (ISoftDeletable, ISafeContent, IRecommendableContent)
    -   Organized by aggregate roots (Posts/, Comments/, Jobs/, Users/, etc.)
    -   No infrastructure dependencies

#### **ExpertBridge.Data** (Infrastructure Layer)

-   **Purpose**: Data access and persistence via Entity Framework Core
-   **Responsibilities**:
    -   DbContext configuration and management
    -   Database migrations
    -   Entity configurations using Fluent API
    -   Soft-delete and timestamp interceptors
-   **Dependencies**: Core, Npgsql, PostgreSQL, Pgvector
-   **Key Features**:
    -   Global query filter for soft-deleted entities
    -   Pgvector extension for semantic search
    -   Connection resilience and retry policies
    -   User secrets for development credentials

#### **ExpertBridge.Extensions** (Infrastructure Layer)

-   **Purpose**: Cross-cutting concerns as reusable extension methods
-   **Responsibilities**:
    -   Infrastructure service registration
    -   Configuration management
    -   External service integration
-   **Modules**:
    -   **AWS/**: S3 service for media storage
    -   **Caching/**: FusionCache with Redis backend
    -   **CORS/**: CORS policy configuration
    -   **Embeddings/**: Ollama integration for vector embeddings
    -   **HealthChecks/**: Database and Redis health checks
    -   **Logging/**: Serilog configuration with multiple sinks
    -   **MessageBroker/**: MassTransit/RabbitMQ setup
    -   **OpenTelemetry/**: Distributed tracing and metrics
    -   **Resilience/**: Polly policies for fault tolerance
-   **Key Features**:
    -   Extension method pattern for clean DI registration
    -   Environment-aware configuration
    -   Service discovery support

#### **ExpertBridge.Notifications** (Infrastructure Layer)

-   **Purpose**: Real-time notifications via SignalR
-   **Responsibilities**:
    -   SignalR hub implementation
    -   Notification broadcasting
    -   Message broker integration for distributed notifications
-   **Key Features**:
    -   Strongly-typed hub methods
    -   Connection lifecycle management
    -   Integration with MassTransit for scalability

#### **ExpertBridge.Contract** (Shared Layer)

-   **Purpose**: Data transfer objects for API contracts
-   **Responsibilities**:
    -   Request DTOs organized by feature
    -   Response DTOs
    -   Message broker contracts
    -   Query parameter objects
-   **Key Features**:
    -   Framework-agnostic POCOs
    -   Validation-ready structures
    -   Immutable records where appropriate

#### **ExpertBridge.Worker** (Background Processing)

-   **Purpose**: Asynchronous background job processing
-   **Responsibilities**:
    -   MassTransit message consumption
    -   Scheduled job execution via Quartz.NET
    -   AI processing pipelines
    -   Batch operations
-   **Key Consumers**:
    -   PostProcessingPipelineConsumer - Orchestrates multi-step post processing
    -   PostTaggingConsumer - AI-powered automatic tagging
    -   InappropriateContentDetectionConsumer - NSFW/toxic content detection
    -   PostEmbeddingConsumer - Vector embedding generation
    -   NotificationSendingPipelineConsumer - Notification delivery
    -   UserInterestsUpdatedConsumer - Interest profile updates
-   **Dependencies**: Application, Data, Extensions
-   **Key Features**:
    -   Quartz.NET with PostgreSQL persistence
    -   Idempotent consumer pattern
    -   Retry policies for fault tolerance
    -   Automatic EF migrations on startup

#### **ExpertBridge.Host** (Orchestration)

-   **Purpose**: .NET Aspire AppHost for local development orchestration
-   **Responsibilities**:
    -   Infrastructure container orchestration (RabbitMQ, Redis, Seq, Ollama)
    -   Service discovery configuration
    -   Connection string management
    -   Resource coordination
-   **Key Features**:
    -   One-command startup for full stack
    -   Automatic service wiring
    -   Development-optimized configuration
    -   GPU support for Ollama

## Technologies Used

### Core Frameworks

-   **.NET 10.0**: Latest preview with enhanced performance and new language features
-   **ASP.NET Core 10.0**: Web API framework with minimal APIs support
-   **Blazor Server**: Server-side rendering for admin dashboard
-   **Entity Framework Core 10.0**: ORM with advanced query capabilities

### Data & Storage

-   **PostgreSQL 18** with **Pgvector** extension: Primary database with vector similarity search
-   **Npgsql 10.0**: High-performance .NET data provider for PostgreSQL
-   **Redis**: Distributed caching and session management
-   **FusionCache**: Hybrid caching (memory + distributed Redis)
-   **AWS S3**: Scalable object storage for media files
-   **Z.EntityFramework.Extensions**: Bulk operations for EF Core

### Messaging & Background Processing

-   **MassTransit**: Distributed application framework for message-based systems
-   **RabbitMQ**: Message broker for event-driven architecture
-   **Quartz.NET**: Enterprise job scheduler with PostgreSQL persistence

### AI & Machine Learning

-   **Microsoft.Extensions.AI**: Unified AI abstractions for .NET
-   **Ollama**: Local LLM for embeddings generation (model: `snowflake-arctic-embed2:latest`)
-   **Groq SDK**: Cloud LLM API for content moderation and categorization
-   **Pgvector**: Vector embeddings storage and similarity search

### Authentication & Authorization

-   **Firebase Admin SDK**: JWT token validation and user management
-   **ASP.NET Core Identity**: Authentication for admin dashboard
-   **JWT Bearer**: Token-based API authentication

### API Documentation

-   **Scalar.AspNetCore**: Modern OpenAPI documentation (replaced Swagger)
-   OpenAPI 3.0 specification with bearer token support

### Observability & Monitoring

-   **OpenTelemetry**: Distributed tracing, metrics, and logs
-   **Serilog**: Structured logging with multiple sinks (Console, File, Seq, OpenTelemetry)
-   **Prometheus**: Metrics collection and export
-   **AspNetCore.HealthChecks**: Health monitoring for dependencies (PostgreSQL, Redis, RabbitMQ)
-   **Seq**: Centralized log aggregation and search

### Resilience & Performance

-   **Polly**: Resilience policies (retry, circuit breaker, timeout, rate limiting)
-   **Microsoft.Extensions.Resilience**: Resilience pipeline builder
-   **Response Caching**: HTTP cache profiles for API optimization
-   **Service Discovery**: Dynamic service endpoint resolution

### Real-Time Communication

-   **SignalR**: WebSocket-based real-time notifications
-   Hub endpoint: `/api/notificationsHub`

### Validation & Data Generation

-   **FluentValidation**: Strongly-typed validation rules
-   **Bogus**: Fake data generation for testing and seeding

### Orchestration & Deployment

-   **.NET Aspire**: Local development orchestration and service discovery
-   **Docker**: Containerization for all services
-   **Docker Compose**: Multi-container orchestration (PostgreSQL, pgAdmin, full stack)

### Development Tools

-   **Central Package Management**: Version control via `Directory.Packages.props`
-   **User Secrets**: Secure development credential storage
-   **Hot Reload**: `dotnet watch` for rapid development
-   **Nullable Reference Types**: Enhanced null safety
-   **Code Analysis**: Roslyn analyzers enabled

## Getting Started

### Prerequisites

-   **.NET SDK 10.0** or later ([download preview](https://dotnet.microsoft.com/download/dotnet/10.0))
-   **Docker Desktop** with Docker Compose (for containerized infrastructure)
-   **Git** for version control
-   Required (if not using Aspire orchestration):
    -   **PostgreSQL 18** with `pgvector` extension
    -   **Redis 7+** for caching
    -   **RabbitMQ 3.13+** for messaging
    -   **Seq** (for centralized logging)
    -   **Ollama** (for local embeddings - requires GPU for optimal performance)

## Quick Start - Two Methods

Choose ONE of the following. Prefer the **Aspire Host** for day-to-day development. Use **Docker Compose** only when you explicitly want a standalone container environment (and you are NOT running Aspire simultaneously).

### Method 1: .NET Aspire Host (Recommended)

Runs all dependencies (PostgreSQL 18, Redis, RabbitMQ, Ollama, telemetry) automatically with service discovery.

**Prerequisites**

-   .NET SDK 10.0 installed
-   User Secrets configured for Host, API, Admin, Worker (see Configuration section)

**Run:**

```bash
dotnet run --project ExpertBridge.Host/ExpertBridge.Host.csproj --launch-profile http
```

**The Aspire Host provides:**

-   Containers/services for PostgreSQL 18, RabbitMQ, Redis, Seq (if configured), Ollama
-   Automatic service discovery and connection string wiring
-   Unified telemetry and health reporting

### Method 2: Docker Compose (Non-Aspire Standalone)

Use the merged `docker-compose.yaml` when you do NOT run the Aspire Host (e.g., for production parity testing or deployment prep).

**Prerequisites**

-   Docker Desktop running
-   Complete `.env` file (see Environment Variables section below)

**Run full stack:**

```bash
docker compose up -d --pull=missing
```

**Included Services:** PostgreSQL 18 + pgAdmin, RabbitMQ (management UI), Redis, Ollama (embeddings), API, Admin, Worker, optional Aspire dashboard container.

**Important Notes:**

-   Do NOT run Docker Compose and Aspire Host at the same time (port & resource conflicts)
-   Every required variable in `.env` must be set before starting; missing values will cause failures
-   Remove `aspire-dashboard` service if you later switch to the Aspire Host

## Configuration

### User Secrets Configuration (for Aspire Host Method)

Configure user secrets for each project using the `dotnet user-secrets` command or Visual Studio's "Manage User Secrets" feature.

#### Host Project User Secrets

```bash
cd ExpertBridge.Host
dotnet user-secrets set "Parameters:Redis-password" "aU1y2Az7CXrXhw9J8Pvrjq"
dotnet user-secrets set "Parameters:Postgresql-password" "TbsP_WHGWRQTB4(!q0YY43"
dotnet user-secrets set "AppHost:OtlpApiKey" "your-otlp-api-key"
dotnet user-secrets set "AppHost:McpApiKey" "your-mcp-api-key"
```

Or create `secrets.json` with:

```json
{
    "Parameters:Redis-password": "aU1y2Az7CXrXhw9J8Pvrjq",
    "Parameters:Postgresql-password": "TbsP_WHGWRQTB4(!q0YY43",
    "AppHost:OtlpApiKey": "your-otlp-api-key",
    "AppHost:McpApiKey": "your-mcp-api-key"
}
```

#### API Project User Secrets

```json
{
    "ConnectionStrings": {
        "Postgresql": "Host=localhost;Port=5432;Username=root;Password=root;Database=ExpertBridgeDb",
        "Redis": "localhost:6379,password=aU1y2Az7CXrXhw9J8Pvrjq"
    },
    "Firebase": {
        "Type": "service_account",
        "ProjectId": "your-firebase-project-id",
        "PrivateKeyId": "your-private-key-id",
        "PrivateKey": "-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n",
        "ClientEmail": "your-service-account@your-project.iam.gserviceaccount.com",
        "ClientId": "your-client-id",
        "AuthUri": "https://accounts.google.com/o/oauth2/auth",
        "TokenUri": "https://oauth2.googleapis.com/token",
        "AuthProviderX509CertUrl": "https://www.googleapis.com/oauth2/v1/certs",
        "ClientX509CertUrl": "https://www.googleapis.com/robot/v1/metadata/x509/your-service-account",
        "UniverseDomain": "googleapis.com",
        "AuthenticationTokenUri": "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=YOUR_API_KEY"
    },
    "Authentication": {
        "Firebase": {
            "Issuer": "https://securetoken.google.com/your-project-id",
            "Audience": "your-project-id",
            "TokenUri": "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=YOUR_API_KEY"
        }
    },
    "AwsS3": {
        "Region": "your-region",
        "BucketName": "your-bucket-name",
        "AwsKey": "your-aws-access-key",
        "AwsSecret": "your-aws-secret-key",
        "BucketUrl": "https://your-bucket.s3.your-region.amazonaws.com",
        "CacheControl": "public,max-age=31536000",
        "MaxFileSize": 157286400
    },
    "Ollama": {
        "Endpoint": "http://localhost:11434",
        "ModelId": "snowflake-arctic-embed2"
    },
    "Groq": {
        "ApiKey": "your-groq-api-key",
        "Model": "openai/gpt-oss-120b"
    },
    "MessageBroker": {
        "Host": "amqp://localhost:5672/",
        "Username": "guest",
        "Password": "guest"
    }
}
```

#### Worker Project User Secrets

```json
{
    "ConnectionStrings": {
        "Postgresql": "Host=localhost;Port=5432;Username=root;Password=root;Database=ExpertBridgeDb",
        "QuartzDatabase": "Host=localhost;Port=5432;Username=root;Password=root;Database=QuartzDb",
        "Redis": "localhost:6379,password=aU1y2Az7CXrXhw9J8Pvrjq"
    },
    "Firebase": {
        "Type": "service_account",
        "ProjectId": "your-firebase-project-id",
        "PrivateKeyId": "your-private-key-id",
        "PrivateKey": "-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n",
        "ClientEmail": "your-service-account@your-project.iam.gserviceaccount.com",
        "ClientId": "your-client-id",
        "AuthUri": "https://accounts.google.com/o/oauth2/auth",
        "TokenUri": "https://oauth2.googleapis.com/token",
        "AuthProviderX509CertUrl": "https://www.googleapis.com/oauth2/v1/certs",
        "ClientX509CertUrl": "https://www.googleapis.com/robot/v1/metadata/x509/your-service-account",
        "UniverseDomain": "googleapis.com",
        "AuthenticationTokenUri": "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=YOUR_API_KEY"
    },
    "Authentication": {
        "Firebase": {
            "Issuer": "https://securetoken.google.com/your-project-id",
            "Audience": "your-project-id",
            "TokenUri": "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=YOUR_API_KEY"
        }
    },
    "AwsS3": {
        "Region": "your-region",
        "BucketName": "your-bucket-name",
        "AwsKey": "your-aws-access-key",
        "AwsSecret": "your-aws-secret-key",
        "BucketUrl": "https://your-bucket.s3.your-region.amazonaws.com",
        "CacheControl": "public,max-age=31536000",
        "MaxFileSize": 157286400
    },
    "Ollama": {
        "Endpoint": "http://localhost:11434",
        "ModelId": "snowflake-arctic-embed2"
    },
    "Groq": {
        "ApiKey": "your-groq-api-key",
        "Model": "openai/gpt-oss-120b"
    },
    "MessageBroker": {
        "Host": "amqp://localhost:5672/",
        "Username": "guest",
        "Password": "guest"
    }
}
```

#### Admin Project User Secrets

```json
{
    "ConnectionStrings": {
        "Postgresql": "Host=localhost;Port=5432;Username=root;Password=root;Database=ExpertBridgeDb",
        "AdminDb": "Host=localhost;Port=5432;Username=root;Password=root;Database=AdminDb",
        "Redis": "localhost:6379,password=aU1y2Az7CXrXhw9J8Pvrjq"
    },
    "AwsS3": {
        "Region": "your-region",
        "BucketName": "your-bucket-name",
        "AwsKey": "your-aws-access-key",
        "AwsSecret": "your-aws-secret-key",
        "BucketUrl": "https://your-bucket.s3.your-region.amazonaws.com",
        "CacheControl": "public,max-age=31536000",
        "MaxFileSize": 157286400
    },
    "MessageBroker": {
        "Host": "amqp://localhost:5672/",
        "Username": "guest",
        "Password": "guest"
    }
}
```

### Docker Compose Configuration (for Docker Method)

**IMPORTANT:** Before running `docker compose up`, you **must** create a `.env` file in the repository root with all required environment variables. The compose file will not start without proper configuration.

Create a `.env` file with the following structure (based on `docker-compose.yaml`):

```bash
# OpenTelemetry Configuration
OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES=true
OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES=true
OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY=in_memory
OTEL_EXPORTER_OTLP_ENDPOINT=http://aspire-dashboard:18889

# ASP.NET Core Configuration
ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ASPNETCORE_ENVIRONMENT=Production
HTTP_PORTS=8080

# Connection Strings
ConnectionStrings__rabbitmq=amqp://guest:guest@rabbitmq:5672/
ConnectionStrings__Redis=redis:6379,password=your-redis-password
ConnectionStrings__Postgresql=Host=postgres;Port=5432;Database=expertbridge;Username=postgres;Password=your-postgres-password
ConnectionStrings__QuartzDatabase=Host=postgres;Port=5432;Database=quartz;Username=postgres;Password=your-postgres-password
ConnectionStrings__AdminDb=Host=postgres;Port=5432;Database=admin;Username=postgres;Password=your-postgres-password

# Message Broker
MessageBroker__Host=amqp://rabbitmq:5672/
MessageBroker__Username=guest
MessageBroker__Password=guest

# Firebase Configuration (replace with your values)
Firebase__Type=service_account
Firebase__ProjectId=your-project-id
Firebase__PrivateKeyId=your-private-key-id
Firebase__PrivateKey=your-private-key-with-newlines
Firebase__ClientEmail=your-service-account-email
Firebase__ClientId=your-client-id
Firebase__AuthUri=https://accounts.google.com/o/oauth2/auth
Firebase__TokenUri=https://oauth2.googleapis.com/token
Firebase__ClientX509CertUrl=your-cert-url
Firebase__UniverseDomain=googleapis.com
Firebase__AuthProviderX509CertUrl=https://www.googleapis.com/oauth2/v1/certs
Firebase__AuthenticationTokenUri=your-auth-token-uri

# Firebase Authentication
Authentication__Firebase__Issuer=your-issuer
Authentication__Firebase__Audience=your-audience
Authentication__Firebase__TokenUri=your-token-uri

# AWS S3 Configuration (replace with your values)
AwsS3__Region=your-region
AwsS3__BucketName=your-bucket-name
AwsS3__AwsKey=your-aws-key
AwsS3__AwsSecret=your-aws-secret
AwsS3__BucketUrl=your-bucket-url
AwsS3__CacheControl=public,max-age=31536000
AwsS3__MaxFileSize=157286400

# AI Services
PostCategorizer__BaseUrl=http://post-categorizer:8000
Ollama__Endpoint=http://ollama:11434
Ollama__ModelId=snowflake-arctic-embed2

# Groq Configuration (replace with your API key)
Groq__Apikey=your-groq-api-key
Groq__Model=openai/gpt-oss-120b
```

**Security Notes:**

-   Never commit the `.env` file to version control
-   Add `.env` to your `.gitignore`
-   Replace all placeholder values with your actual credentials
-   For Firebase PrivateKey, preserve the newline characters (\n)

### Accessing Services

**After starting with Aspire Host:**

-   Aspire Dashboard: `http://localhost:7000`
-   API: Port assigned by Aspire (check dashboard)
-   Admin: Port assigned by Aspire (check dashboard)
-   SignalR Hub: `/api/notificationsHub`

**After starting with Docker Compose:**

-   API: Check exposed ports in `docker-compose.yaml`
-   Admin: Check exposed ports in `docker-compose.yaml`
-   Aspire Dashboard: `http://localhost:18888`
-   Health endpoints: `/health`, `/health/ready`, `/alive`
-   Prometheus metrics: `/metrics`

## Database and EF Core

### Automatic Migrations

The API and Worker projects apply EF Core migrations automatically at startup in Development environment.

### Manual Migration Commands

**Add a new migration:**

```bash
dotnet ef migrations add <MigrationName> \
  --project ExpertBridge.Data \
  --startup-project ExpertBridge.Api
```

**Update database to latest migration:**

```bash
dotnet ef database update \
  --project ExpertBridge.Data \
  --startup-project ExpertBridge.Api
```

**Remove last migration:**

```bash
dotnet ef migrations remove \
  --project ExpertBridge.Data \
  --startup-project ExpertBridge.Api
```

**List all migrations:**

```bash
dotnet ef migrations list \
  --project ExpertBridge.Data \
  --startup-project ExpertBridge.Api
```

### Database Features

-   **Soft Delete**: Global query filter automatically excludes soft-deleted entities
-   **Timestamp Management**: `CreatedAt` and `LastModified` automatically tracked via interceptors
-   **Pgvector Extension**: Enabled for vector similarity search
-   **Connection Resilience**: Automatic retry policies for transient failures
-   **Query Filters**: Applied globally for `ISoftDeletable` entities
-   **Bulk Operations**: Optimized with Z.EntityFramework.Extensions

### Database Contexts

-   **ExpertBridgeDbContext** (ExpertBridge.Data): Main application database
-   **QuartzContext** (ExpertBridge.Worker): Quartz.NET job persistence
-   **AdminDbContext** (ExpertBridge.Admin): Admin-specific context with Identity

### Connection Strings

Configure via `appsettings.json` or environment variables:

```json
{
    "ConnectionStrings": {
        "Postgresql": "Host=localhost;Port=5432;Database=expertbridge;Username=root;Password=root",
        "QuartzDatabase": "Host=localhost;Port=5432;Database=quartz;Username=root;Password=root",
        "Redis": "localhost:6379"
    }
}
```

### Important Notes

-   Always test migrations in development before applying to production
-   Use `dotnet ef migrations script` to generate SQL scripts for production deployment
-   Pgvector extension must be installed on PostgreSQL 18: `CREATE EXTENSION IF NOT EXISTS vector;`
-   Connection strings support User Secrets in development (right-click project → Manage User Secrets)
-   See Configuration section above for complete user secrets examples

## Docker Deployment

### Building Container Images

Multi-stage Dockerfiles are provided for optimized production builds:

```bash
# Build API image
docker build -f ExpertBridge.Api/Dockerfile -t expertbridge-api:latest .

# Build Admin dashboard image
docker build -f ExpertBridge.Admin/Dockerfile -t expertbridge-admin:latest .

# Build Worker image
docker build -f ExpertBridge.Worker/Dockerfile -t expertbridge-worker:latest .
```

### Docker Compose - Full Stack

⚠️ **IMPORTANT:** Before running Docker Compose, you **MUST** configure all environment variables in a `.env` file in the repository root. The compose file will fail to start without proper configuration.

Run all application services (API, Admin, Worker) with Docker Compose:

```bash
docker compose up -d
```

**Default Ports:**

-   API: Check `docker-compose.yaml` for exposed ports
-   Admin: Check `docker-compose.yaml` for exposed ports
-   Worker: Background service (no exposed port)
-   Aspire Dashboard: `http://localhost:18888`

**Configuration:**

-   **All environment variables must be set in `.env` file before starting**
-   Environment variables defined in `docker-compose.yaml` reference `.env` values
-   See Docker Compose Configuration section for complete `.env` template
-   Modify connection strings to point to containerized infrastructure
-   Ensure RabbitMQ, Redis, PostgreSQL 18, and Seq are accessible

### Environment Variables for Containers

⚠️ **CRITICAL:** You **MUST** create a `.env` file with all required variables before running `docker compose`. See the **Docker Compose Configuration** section in the Configuration chapter for the complete `.env` template with all required variables.

**Required Categories:**

-   OpenTelemetry configuration
-   ASP.NET Core settings
-   Connection strings (PostgreSQL 18, Redis, RabbitMQ, Quartz)
-   Message Broker credentials
-   Firebase configuration (service account details)
-   Firebase Authentication settings
-   AWS S3 credentials and settings
-   AI Services (Ollama endpoint, Groq API key)

**Security Warning:**

-   Never commit `.env` file to version control
-   Add `.env` to `.gitignore`
-   Keep sensitive credentials (Firebase, AWS, Groq API keys) secure

### Health Checks

All containers expose health check endpoints:

-   API: `http://localhost:5027/health`
-   Admin: `http://localhost:5028/health`
-   Worker: Internal health checks (no HTTP endpoint)

### Production Considerations

1. **Secrets Management**: Use Docker secrets or environment variable injection
2. **Networking**: Configure custom networks for service isolation
3. **Volumes**: Persist PostgreSQL data and logs
4. **Resource Limits**: Set CPU/memory limits in compose file
5. **Reverse Proxy**: Use Nginx or Traefik for SSL termination
6. **Monitoring**: Integrate with Prometheus for metrics collection

## Observability & Monitoring

### Structured Logging with Serilog

**Log Sinks:**

-   **Console**: Development logging with colored output
-   **File**: Rolling file logs in `logs/` directory
-   **Seq**: Centralized log aggregation (configure `ConnectionStrings:Seq`)
-   **OpenTelemetry**: Distributed tracing integration

**Log Enrichment:**

-   Environment name (Development/Production)
-   Thread ID and Process ID
-   Correlation IDs for distributed tracing

### OpenTelemetry Integration

**Instrumentation:**

-   HTTP client and server requests
-   Database queries (EF Core)
-   Message broker operations (MassTransit)
-   Custom business metrics

**Exporters:**

-   **Prometheus**: Metrics scraping at `/metrics` endpoint
-   **OTLP**: Traces and metrics export (configure `OTEL_EXPORTER_OTLP_ENDPOINT`)
-   **Console**: Development diagnostics

### Health Checks

Comprehensive health monitoring for all dependencies:

**Endpoints:**

-   `/health`: Overall health status (includes all checks)
-   `/health/ready`: Readiness probe (for Kubernetes)
-   `/alive`: Liveness probe (basic availability)

**Monitored Resources:**

-   PostgreSQL database connectivity
-   Redis cache availability
-   RabbitMQ message broker
-   Disk space and memory
-   API responsiveness

### Metrics

**Custom Metrics:**

-   API request duration and count
-   Message processing throughput
-   AI operation latency (NSFW detection, tagging)
-   Cache hit/miss ratios
-   Background job execution time

**Access Prometheus Metrics:**

```bash
curl http://localhost:5027/metrics
```

### Distributed Tracing

-   Trace context propagated across all services (API → Worker → External APIs)
-   Correlation IDs in logs for request tracking
-   Span attributes for detailed operation context

## Testing

### Test Projects

-   **ExpertBridge.Tests.Unit**: Unit tests for business logic

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific test project
dotnet test ExpertBridge.Tests.Unit

# Run tests with verbose output
dotnet test --logger "console;verbosity=detailed"
```

### Test Framework

-   **xUnit**: Test framework
-   **Shouldly**: Fluent assertion library
-   **Microsoft.AspNetCore.Mvc.Testing**: Integration testing with WebApplicationFactory
-   **Coverlet**: Code coverage collection

### Testing Guidelines

-   Follow AAA pattern (Arrange, Act, Assert)
-   Mock external dependencies
-   Use in-memory database for integration tests
-   Test naming: `MethodName_Scenario_ExpectedResult`
-   Aim for high coverage on business logic (Application and Core layers)

## Development Guidelines

### Code Organization

-   **Group by feature/aggregate**, not by technical layer
-   Keep files under 500 lines; split if larger
-   One entity per file
-   Separate folder for each aggregate root

### Naming Conventions

-   **Entities**: Singular nouns (`Post`, `Comment`, `User`)
-   **Collections**: Plural (`Posts`, `Comments`, `Users`)
-   **Services**: `{Domain}Service` (`PostService`, `TaggingService`)
-   **Controllers**: `{Resource}Controller` (`PostsController`)
-   **Requests**: `{Action}{Resource}Request` (`CreatePostRequest`)
-   **Responses**: `{Resource}Response` (`PostResponse`)
-   **Consumers**: `{Action}Consumer` (`PostTaggingConsumer`)

### Architectural Principles

-   **Clean Architecture**: Dependencies point inward toward Core
-   **Domain-Driven Design**: Rich domain entities with behavior
-   **CQRS Pattern**: Separate read/write models (informal)
-   **Event-Driven**: Publish domain events via message broker
-   **Repository Pattern**: Services interact with DbContext directly (no formal repositories)

### Best Practices

-   Use `async`/`await` for all I/O operations
-   Apply XML documentation for public APIs
-   Implement validation using FluentValidation
-   Use dependency injection for all dependencies
-   Log at appropriate levels (Information, Warning, Error)
-   Apply `[Authorize]` by default; opt-in with `[AllowAnonymous]`
-   Return proper HTTP status codes (200, 201, 204, 400, 401, 403, 404, 500)
-   Use nullable reference types (enabled globally)
-   Dispose unmanaged resources properly (`IDisposable`/`IAsyncDisposable`)

### Entity Guidelines

-   Always inherit from `BaseModel` for `Id`, `CreatedAt`, `LastModified`
-   Apply marker interfaces: `ISoftDeletable`, `ISafeContent`, `IRecommendableContent`
-   Keep domain logic in entities (rich models)
-   Use navigation properties as `ICollection<T>`
-   Add XML documentation with business rules

### Service Guidelines

-   Services should be stateless
-   Register as Scoped or Transient (not Singleton with state)
-   Return DTOs, not entities
-   Implement proper exception handling
-   Use bulk operations for large datasets

### Message Consumer Guidelines

-   Implement idempotency for all consumers
-   Use `IServiceScopeFactory` to resolve scoped services
-   Log all processing steps
-   Apply retry policies for transient failures
-   Keep consumer logic focused (single responsibility)

### Performance Guidelines

-   Use `AsNoTracking()` for read-only queries
-   Eager load with `Include()` judiciously
-   Prefer projection (`Select`) over full entity loading
-   Implement pagination for list endpoints
-   Cache frequently accessed data
-   Use compiled queries for hot paths

## Contributing

### Getting Started

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Follow the development guidelines above
4. Write tests for new functionality
5. Ensure all tests pass: `dotnet test`
6. Commit with clear messages: `git commit -m "feat: add feature description"`
7. Push to your fork: `git push origin feature/my-feature`
8. Create a Pull Request

### Pull Request Guidelines

-   Include description of changes
-   Reference related issues
-   Ensure CI/CD checks pass
-   Add tests for new features
-   Update documentation as needed
-   Follow code review feedback

### Code Review Checklist

-   [ ] Follows naming conventions
-   [ ] XML documentation for public APIs
-   [ ] Async/await for I/O operations
-   [ ] Proper exception handling
-   [ ] Input validation
-   [ ] Logging at appropriate levels
-   [ ] Unit/integration tests included
-   [ ] No hardcoded secrets
-   [ ] Database migrations included (if schema changed)
-   [ ] CQRS pattern respected
-   [ ] Null reference checks

## Additional Resources

-   [Architecture Instructions](.github/instructions/ExpertBridgeServerIntructions.instructions.md) - Comprehensive development guidelines
-   [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
-   [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core)
-   [MassTransit Documentation](https://masstransit-project.com/)
-   [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire)

## License

This project is licensed under the MIT License. Source files include MIT license headers.

## Support

For issues, questions, or contributions, please open an issue on the GitHub repository.
