# 🛡️ Validation Integration Plan for ExpertBridge

**Generated:** October 26, 2025  
**Branch:** enhance-validation-in-all-projects  
**Analyzed Projects:** ExpertBridge.Core, ExpertBridge.Application, ExpertBridge.Api

---

## 📋 Executive Summary

### Current Validation Coverage Status

| Metric                                        | Count | Percentage   |
| --------------------------------------------- | ----- | ------------ |
| **Total Request DTOs**                        | 25    | 100%         |
| **Requests with FluentValidation Validators** | 25    | **100%** ✅  |
| **Requests Used in Services**                 | 16    | 64%          |
| **Requests Used in Controllers**              | 15    | 60%          |
| **Services with Validation Enforcement**      | 1     | **6.25%** ⚠️ |
| **Controllers with Validation Enforcement**   | 0     | **0%** ❌    |

### 🔴 Critical Finding

**While 100% of Request DTOs have validators defined, only 6.25% of service methods actually invoke validation before processing.**

The application has a **comprehensive validator infrastructure** but **severely underutilized validation enforcement**. Controllers rely entirely on implicit validation (which is **NOT configured**), and services perform only basic null checks with manual validation logic instead of leveraging FluentValidation.

### High-Level Risk Assessment

| Risk                                          | Impact    | Likelihood | Priority    |
| --------------------------------------------- | --------- | ---------- | ----------- |
| **Invalid data bypassing business rules**     | 🔴 High   | 🔴 High    | 🔴 Critical |
| **Inconsistent error responses**              | 🟡 Medium | 🔴 High    | 🔴 Critical |
| **Duplicate validation logic**                | 🟡 Medium | 🟡 Medium  | 🟡 Medium   |
| **Security vulnerabilities (injection, XSS)** | 🔴 High   | 🟡 Medium  | 🔴 Critical |
| **Database integrity violations**             | 🔴 High   | 🟡 Medium  | 🔴 Critical |

---

## 📊 Validation Coverage Report

### Request DTOs Inventory

| #   | Request Name                   | Has Validator | Used In Service                   | Used In Controller      | Validation Status |
| --- | ------------------------------ | ------------- | --------------------------------- | ----------------------- | ----------------- |
| 1   | `ApplyToJobPostingRequest`     | ✅            | `JobPostingService`               | `JobPostingsController` | ❌ Not validated  |
| 2   | `CreateCommentRequest`         | ✅            | `CommentService`                  | `CommentsController`    | ❌ Not validated  |
| 3   | `CreateJobOfferRequest`        | ✅            | `JobService`                      | `JobsController`        | ❌ Not validated  |
| 4   | `CreateJobPostingRequest`      | ✅            | `JobPostingService`               | `JobPostingsController` | ❌ Not validated  |
| 5   | `CreateMessageRequest`         | ✅            | `MessagingService`                | `MessagesController`    | ❌ Not validated  |
| 6   | `CreatePostRequest`            | ✅            | `PostService`                     | `PostsController`       | ❌ Not validated  |
| 7   | `EditCommentRequest`           | ✅            | `CommentService`                  | `CommentsController`    | ❌ Not validated  |
| 8   | `EditJobPostingRequest`        | ✅            | `JobPostingService`               | `JobPostingsController` | ❌ Not validated  |
| 9   | `EditPostRequest`              | ✅            | `PostService`                     | `PostsController`       | ❌ Not validated  |
| 10  | `GeneratePresignedUrlsRequest` | ✅            | ❌ Not used                       | `MediaController`       | ❌ Not validated  |
| 11  | `InitiateJobOfferRequest`      | ✅            | ❌ Not used                       | ❌ Not used             | ❌ Not validated  |
| 12  | `JobPostingsPaginationRequest` | ✅            | `JobPostingService`               | `JobPostingsController` | ❌ Not validated  |
| 13  | `MediaObjectRequest`           | ✅            | `MediaAttachmentService` (nested) | ❌ Not used             | ❌ Not validated  |
| 14  | `OnboardUserRequest`           | ✅            | ❌ Not used                       | `ProfilesController`    | ❌ Not validated  |
| 15  | `PatchCommentRequest`          | ✅            | ❌ Not used                       | ❌ Not used             | ❌ Not validated  |
| 16  | `PostsCursorRequest`           | ✅            | `PostService`                     | `PostsController`       | ❌ Not validated  |
| 17  | `RegisterUserRequest`          | ✅            | ❌ Not used                       | ❌ Not used             | ❌ Not validated  |
| 18  | `RespondToJobOfferRequest`     | ✅            | ❌ Not used                       | ❌ Not used             | ❌ Not validated  |
| 19  | `SearchJobPostsRequest`        | ✅            | ❌ Not used                       | `SearchController`      | ❌ Not validated  |
| 20  | `SearchPostRequest`            | ✅            | ❌ Not used                       | `SearchController`      | ❌ Not validated  |
| 21  | `SearchUserRequest`            | ✅            | ❌ Not used                       | `SearchController`      | ❌ Not validated  |
| 22  | `UpdateJobStatusRequest`       | ✅            | `JobService`                      | `JobsController`        | ❌ Not validated  |
| 23  | `UpdateProfileRequest`         | ✅            | `ProfileService`                  | `ProfilesController`    | ✅ **VALIDATED**  |
| 24  | `UpdateProfileSkillsRequest`   | ✅            | `ProfileService` (nested)         | ❌ Not used             | ❌ Not validated  |
| 25  | `UpdateUserRequest`            | ✅            | ❌ Not used                       | `UsersController`       | ❌ Not validated  |

---

## 🔍 Detailed Analysis by Layer

### 1. Controller Layer (API Entry Points)

**Current State:** ❌ **No explicit validation** in any controller

**FluentValidation Auto-Validation Configuration:** ❌ **NOT ENABLED**

**Evidence:**

-   `Program.cs` does NOT include `.AddFluentValidationAutoValidation()`
-   No `ModelState` checks in controllers
-   No validator injection in controllers
-   Controllers pass raw requests directly to services

**Impact:**

-   Invalid payloads can reach business logic layer
-   No standardized error response format for validation failures
-   Security vulnerabilities (SQL injection, XSS, path traversal)

**Controllers Processing Requests:**

| Controller              | Endpoints with Requests                                   | Validation Method         |
| ----------------------- | --------------------------------------------------------- | ------------------------- |
| `PostsController`       | `Create`, `GetFeed`, `Edit`                               | ❌ None                   |
| `CommentsController`    | `Create`, `Edit`                                          | ❌ None                   |
| `JobPostingsController` | `CreateJobPosting`, `GetOffsetPaginated`, `Edit`, `Apply` | ❌ None                   |
| `JobsController`        | `InitiateJobOffer`, `UpdateJobStatus`                     | ❌ None                   |
| `ProfilesController`    | `OnboardUser`, `UpdateProfile`                            | ❌ None                   |
| `MessagesController`    | `CreateMessage`                                           | ❌ None                   |
| `SearchController`      | `SearchPosts`, `SearchUsers`, `SearchJobPosts`            | ❌ None                   |
| `MediaController`       | `GenerateUrls`                                            | ❌ Manual null check only |
| `UsersController`       | `Update`                                                  | ❌ None                   |

---

### 2. Service Layer (Business Logic)

**Current State:** ⚠️ **Only 1 service validates systematically**

**Validation Pattern:**

-   **ProfileService**: ✅ Uses `IValidator<UpdateProfileRequest>` injection + `ValidateAsync()`
-   **All Other Services**: ❌ Manual null checks + basic business rules

**Service-Level Request Processing:**

| Service                  | Methods with Requests                                                         | Validation Approach                                                  |
| ------------------------ | ----------------------------------------------------------------------------- | -------------------------------------------------------------------- |
| `PostService`            | `CreatePostAsync`, `EditPostAsync`, `GetRecommendedPostsCursorPageAsync`      | ❌ `ArgumentNullException` + manual string checks                    |
| `CommentService`         | `CreateCommentAsync`, `EditCommentAsync`                                      | ❌ `ArgumentNullException` + manual string checks                    |
| `JobPostingService`      | `CreateAsync`, `EditAsync`, `GetRecommendedJobsOffsetPageAsync`, `ApplyAsync` | ❌ `ArgumentNullException` + manual string checks                    |
| `JobService`             | `CreateJobOfferAsync`, `UpdateJobOfferStatusAsync`                            | ❌ `ArgumentNullException` + domain validation                       |
| `ProfileService`         | `UpdateProfileAsync`, `UpdateProfileSkillsAsync`                              | ✅ **FluentValidation injection + ValidateAsync()**                  |
| `MessagingService`       | `CreateAsync`                                                                 | ❌ `ArgumentNullException` only                                      |
| `MediaAttachmentService` | `ProcessAndAttachMediaAsync`                                                  | ❌ No validation (processes nested `MediaObjectRequest` collections) |

**ProfileService Validation Example (ONLY SERVICE DOING THIS):**

```csharp
// ExpertBridge.Application/DomainServices/ProfileService.cs
private readonly IValidator<UpdateProfileRequest> _updateProfileRequestValidator;

public async Task<ProfileResponse> UpdateProfileAsync(
    User user,
    UpdateProfileRequest request,
    CancellationToken cancellationToken = default)
{
    // Validate the request using the validator from FluentValidation.
    var validationResult = await _updateProfileRequestValidator
        .ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
    {
        throw new ValidationException(validationResult.Errors);
    }

    // ... business logic
}
```

**Typical Pattern in Other Services (PROBLEMATIC):**

```csharp
// ExpertBridge.Application/DomainServices/PostService.cs
public async Task<PostResponse> CreatePostAsync(CreatePostRequest request, Profile authorProfile)
{
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(authorProfile);

    // Manual validation (bypasses FluentValidation rules!)
    if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
    {
        throw new BadHttpRequestException("Title and Content are required.");
    }

    // ... business logic
}
```

**Issues with Current Manual Validation:**

1. ❌ Bypasses all FluentValidation rules (max length, regex, custom rules)
2. ❌ Inconsistent error messages
3. ❌ No localization support
4. ❌ Duplicate validation logic across services
5. ❌ Manual validation rules can drift from validator definitions

---

### 3. Missing Validation Summary

**Critical: Requests NOT Validated Anywhere**

| Request                        | Risk Level | Reason                              |
| ------------------------------ | ---------- | ----------------------------------- |
| `CreatePostRequest`            | 🔴 High    | Public content - XSS/injection risk |
| `CreateCommentRequest`         | 🔴 High    | Public content - XSS/injection risk |
| `CreateJobPostingRequest`      | 🔴 High    | Public content - XSS/injection risk |
| `EditPostRequest`              | 🔴 High    | Public content - XSS/injection risk |
| `EditCommentRequest`           | 🔴 High    | Public content - XSS/injection risk |
| `EditJobPostingRequest`        | 🔴 High    | Public content - XSS/injection risk |
| `CreateMessageRequest`         | 🟡 Medium  | Private content - data integrity    |
| `CreateJobOfferRequest`        | 🟡 Medium  | Financial data - business rules     |
| `UpdateJobStatusRequest`       | 🟡 Medium  | State machine - integrity           |
| `OnboardUserRequest`           | 🔴 High    | User registration - security        |
| `UpdateUserRequest`            | 🔴 High    | Profile data - integrity            |
| `GeneratePresignedUrlsRequest` | 🟡 Medium  | S3 operations - resource abuse      |
| `SearchPostRequest`            | 🟢 Low     | Query-only - performance            |
| `SearchUserRequest`            | 🟢 Low     | Query-only - performance            |
| `SearchJobPostsRequest`        | 🟢 Low     | Query-only - performance            |
| `ApplyToJobPostingRequest`     | 🟡 Medium  | Application data - integrity        |
| `PostsCursorRequest`           | 🟢 Low     | Pagination - performance            |
| `JobPostingsPaginationRequest` | 🟢 Low     | Pagination - performance            |

**Unused Requests (Still Need Validation for Future Use):**

-   `RegisterUserRequest` - Not used (AuthController uses Microsoft.AspNetCore.Identity.Data.RegisterRequest)
-   `InitiateJobOfferRequest` - Not currently invoked
-   `RespondToJobOfferRequest` - Not currently invoked
-   `PatchCommentRequest` - Not currently invoked
-   `MediaObjectRequest` - Nested within other requests, validated indirectly

---

## 🎯 Recommended Approach

### Strategy Overview

Implement a **three-tier validation architecture**:

1. **Controller Level (API Layer):** Automatic FluentValidation middleware
2. **Service Level (Business Layer):** Explicit validator injection for critical operations
3. **Pipeline Level (Cross-Cutting):** Global validation behavior

### A. Controller-Level Validation (PRIMARY RECOMMENDATION)

**Goal:** Automatic validation for all API requests before hitting business logic

**Implementation Steps:**

#### Step 1: Enable FluentValidation Auto-Validation

**File:** `ExpertBridge.Api/Program.cs`

```csharp
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ... existing configuration ...

builder.Services.AddControllers(options =>
{
    // ... existing cache profiles ...
})
.AddFluentValidationAutoValidation()  // ← ADD THIS
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Validators are already registered in AddDomainServices()
// via services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>()
```

**What This Does:**

-   Automatically validates `[FromBody]`, `[FromQuery]`, `[FromRoute]` parameters
-   Returns `400 Bad Request` with structured error details
-   Uses all validators registered in DI container
-   No code changes needed in controllers

**Benefit:** ✅ **Zero code changes** - works with existing controllers

---

### B. Service-Level Validation (SECONDARY - For Critical Operations)

**Goal:** Explicit validation for complex business rules or nested validation

**When to Use:**

-   Operations with multi-step validation logic
-   Nested request objects (e.g., `MediaObjectRequest` within `CreatePostRequest`)
-   Custom business rule validation beyond field-level checks

**Pattern to Follow (Based on ProfileService):**

#### Step 1: Inject Validator in Service Constructor

```csharp
public class PostService
{
    private readonly IValidator<CreatePostRequest> _createPostValidator;
    private readonly IValidator<EditPostRequest> _editPostValidator;
    private readonly ExpertBridgeDbContext _dbContext;

    public PostService(
        IValidator<CreatePostRequest> createPostValidator,
        IValidator<EditPostRequest> editPostValidator,
        ExpertBridgeDbContext dbContext)
    {
        _createPostValidator = createPostValidator;
        _editPostValidator = editPostValidator;
        _dbContext = dbContext;
    }
}
```

#### Step 2: Validate Before Processing

```csharp
public async Task<PostResponse> CreatePostAsync(
    CreatePostRequest request,
    Profile authorProfile,
    CancellationToken cancellationToken = default)
{
    // Explicit validation (if controller-level validation is NOT enabled)
    var validationResult = await _createPostValidator.ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
    {
        throw new ValidationException(validationResult.Errors);
    }

    // Business logic...
}
```

**Note:** If controller-level auto-validation is enabled, service-level validation is **redundant** for simple CRUD. Only use for:

-   Complex nested validation
-   Dynamic validation rules based on user context
-   Batch operations

---

### C. Middleware Integration (GLOBAL ENFORCEMENT)

**Current Middleware:** `GlobalExceptionMiddleware`

**Enhancement:** Handle `ValidationException` globally

**File:** `ExpertBridge.Api/Middleware/GlobalExceptionMiddleware.cs`

```csharp
catch (ValidationException validationEx)
{
    var errors = validationEx.Errors
        .GroupBy(e => e.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray()
        );

    await Results.ValidationProblem(
        errors,
        title: "Validation Failed",
        statusCode: StatusCodes.Status400BadRequest
    ).ExecuteAsync(httpContext);
}
```

**Benefit:** Consistent error handling for both controller and service-level validation

---

### D. Code Consistency Standards

#### Naming Conventions (ALREADY CORRECT ✅)

All validators follow the pattern: `{RequestName}Validator`

**Examples:**

-   `CreatePostRequestValidator`
-   `UpdateProfileRequestValidator`
-   `SearchPostRequestValidator`

#### Namespace Convention (ALREADY CORRECT ✅)

`ExpertBridge.Core.Requests.[DomainContext]`

**Examples:**

-   `ExpertBridge.Core.Requests.CreatePost`
-   `ExpertBridge.Core.Requests.UpdateProfileRequest`

#### Registration (ALREADY CORRECT ✅)

Validators are registered in both:

-   `ExpertBridge.Api/Extensions/DomainServices.cs`
-   `ExpertBridge.Application/Extensions.cs`

```csharp
services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
```

**Note:** This scans the entire `ExpertBridge.Core` assembly and registers all validators. No per-validator registration needed.

---

## 📅 Implementation Roadmap

### Phase 1: Enable Global Validation Infrastructure ⚡ (1-2 hours)

**Goal:** Zero-effort validation for all existing endpoints

| Task                                 | File                           | Estimated Time |
| ------------------------------------ | ------------------------------ | -------------- |
| Add FluentValidation auto-validation | `Program.cs`                   | 15 min         |
| Configure error response format      | `Program.cs`                   | 15 min         |
| Update `GlobalExceptionMiddleware`   | `GlobalExceptionMiddleware.cs` | 30 min         |
| Test existing validators             | Postman/Integration Tests      | 30 min         |

**Expected Output:**

-   ✅ All 25 request validators automatically enforced
-   ✅ Consistent 400 error responses
-   ✅ No controller code changes

**Testing Checklist:**

-   [ ] POST `/api/posts` with empty `Title` → Returns validation error
-   [ ] POST `/api/comments` with `Content` > max length → Returns validation error
-   [ ] POST `/api/profiles/onboard` with invalid email → Returns validation error
-   [ ] PATCH `/api/jobs/offers/{id}/status` with invalid enum → Returns validation error

---

### Phase 2: Service-Level Validation for Critical Operations ⚙️ (4-6 hours)

**Goal:** Add explicit validation for high-risk operations and nested requests

**Priority Services (Based on Risk Assessment):**

| Service             | Methods to Enhance                                 | Validators to Inject                                                                                               | Estimated Time |
| ------------------- | -------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------ | -------------- |
| `PostService`       | `CreatePostAsync`, `EditPostAsync`                 | `IValidator<CreatePostRequest>`, `IValidator<EditPostRequest>`                                                     | 45 min         |
| `CommentService`    | `CreateCommentAsync`, `EditCommentAsync`           | `IValidator<CreateCommentRequest>`, `IValidator<EditCommentRequest>`                                               | 45 min         |
| `JobPostingService` | `CreateAsync`, `EditAsync`, `ApplyAsync`           | `IValidator<CreateJobPostingRequest>`, `IValidator<EditJobPostingRequest>`, `IValidator<ApplyToJobPostingRequest>` | 1 hour         |
| `JobService`        | `CreateJobOfferAsync`, `UpdateJobOfferStatusAsync` | `IValidator<CreateJobOfferRequest>`, `IValidator<UpdateJobStatusRequest>`                                          | 45 min         |
| `MessagingService`  | `CreateAsync`                                      | `IValidator<CreateMessageRequest>`                                                                                 | 30 min         |

**Implementation Pattern:**

**Before:**

```csharp
public async Task<PostResponse> CreatePostAsync(CreatePostRequest request, Profile authorProfile)
{
    ArgumentNullException.ThrowIfNull(request);
    if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
    {
        throw new BadHttpRequestException("Title and Content are required.");
    }
    // ... business logic
}
```

**After:**

```csharp
private readonly IValidator<CreatePostRequest> _createPostValidator;

public PostService(
    IValidator<CreatePostRequest> createPostValidator,
    // ... other dependencies
)
{
    _createPostValidator = createPostValidator;
}

public async Task<PostResponse> CreatePostAsync(
    CreatePostRequest request,
    Profile authorProfile,
    CancellationToken cancellationToken = default)
{
    var validationResult = await _createPostValidator.ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
    {
        throw new ValidationException(validationResult.Errors);
    }

    // ... business logic (remove manual validation)
}
```

**Benefits:**

-   ✅ Removes duplicate validation logic
-   ✅ Leverages all FluentValidation rules
-   ✅ Consistent error handling
-   ✅ Easier to test and maintain

---

### Phase 3: Enhanced Validator Rules 🔧 (Ongoing - 2-3 hours per domain)

**Goal:** Strengthen existing validators with comprehensive rules

**Current Validators to Enhance:**

#### 3.1 Content Creation Validators

**Validators:** `CreatePostRequestValidator`, `CreateCommentRequestValidator`, `CreateJobPostingRequestValidator`

**Enhancements:**

```csharp
public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200)
            .Must(NotContainScriptTags)  // ← Add XSS prevention
            .WithMessage("Title must not contain HTML/script tags");

        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(10000)
            .Must(NotContainScriptTags)  // ← Add XSS prevention
            .WithMessage("Content must not contain script tags");

        RuleForEach(x => x.Media)
            .SetValidator(new MediaObjectRequestValidator());  // ← Validate nested media
    }

    private bool NotContainScriptTags(string input)
    {
        if (string.IsNullOrEmpty(input)) return true;
        return !Regex.IsMatch(input, @"<script[^>]*>", RegexOptions.IgnoreCase);
    }
}
```

#### 3.2 Search/Query Validators

**Validators:** `SearchPostRequestValidator`, `SearchUserRequestValidator`, `SearchJobPostsRequestValidator`

**Enhancements:**

```csharp
public class SearchPostRequestValidator : AbstractValidator<SearchPostRequest>
{
    public SearchPostRequestValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty()
            .MinimumLength(2)  // ← Prevent single-char searches
            .MaximumLength(100);

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 100)  // ← Prevent resource abuse
            .When(x => x.Limit.HasValue);
    }
}
```

#### 3.3 User Management Validators

**Validators:** `UpdateUserRequestValidator`, `OnboardUserRequestValidator`

**Enhancements:**

```csharp
public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[a-zA-Z\s\-']+$")  // ← Letters, spaces, hyphens, apostrophes only
            .WithMessage("First name contains invalid characters");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$")  // ← E.164 format
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Invalid phone number format");
    }
}
```

#### 3.4 Financial/Business Validators

**Validators:** `CreateJobOfferRequestValidator`, `CreateJobPostingRequestValidator`

**Enhancements:**

```csharp
public class CreateJobOfferRequestValidator : AbstractValidator<CreateJobOfferRequest>
{
    public CreateJobOfferRequestValidator()
    {
        RuleFor(x => x.Budget)
            .GreaterThan(0)  // ← Business rule
            .LessThan(1000000)  // ← Prevent unrealistic values
            .WithMessage("Budget must be between $1 and $1,000,000");

        RuleFor(x => x.WorkerId)
            .NotEmpty()
            .Must(BeValidGuid)  // ← Validate ID format
            .WithMessage("Invalid worker ID format");
    }

    private bool BeValidGuid(string id)
    {
        return Guid.TryParse(id, out _);
    }
}
```

---

# 4 Documentation Updates

**File:** `docs/API_VALIDATION_GUIDE.md` (Create new)

```markdown
# API Validation Guide

## Validation Error Response Format

All validation errors return HTTP 400 with the following structure:

{
"title": "Validation Failed",
"status": 400,
"errors": {
"PropertyName": ["Error message 1", "Error message 2"]
},
"traceId": "..."
}

## Common Validation Rules

### Posts

-   **Title**: Required, max 200 characters, no script tags
-   **Content**: Required, max 10,000 characters, no script tags

### Users

-   **Email**: Required, valid email format, max 255 characters
-   **FirstName**: Required, max 50 characters, letters/spaces/hyphens only

[... additional documentation ...]
```

---

## 🔧 Implementation Priority Matrix

| Phase                           | Impact    | Effort    | Priority        | Blockers            |
| ------------------------------- | --------- | --------- | --------------- | ------------------- |
| **Phase 1: Global Middleware**  | 🔴 High   | 🟢 Low    | 🔴 **Critical** | None                |
| **Phase 2: Service Validation** | 🟡 Medium | 🟡 Medium | 🟡 Medium       | Phase 1 recommended |
| **Phase 3: Enhanced Rules**     | 🟡 Medium | 🟡 Medium | 🟢 Low          | Phase 1 required    |
| **Phase 4: Testing**            | 🟡 Medium | 🟡 Medium | 🟡 Medium       | Phase 1-3           |

---

## 📦 Deliverables Checklist

### Phase 1 Completion Criteria

-   [ ] `AddFluentValidationAutoValidation()` added to `Program.cs`
-   [ ] Custom error response format configured
-   [ ] `GlobalExceptionMiddleware` handles `ValidationException`
-   [ ] All 25 validators automatically invoked on API requests
-   [ ] Integration tests verify validation works for at least 5 endpoints

### Phase 2 Completion Criteria

-   [ ] `PostService` validates `CreatePostRequest` and `EditPostRequest`
-   [ ] `CommentService` validates `CreateCommentRequest` and `EditCommentRequest`
-   [ ] `JobPostingService` validates all job-related requests
-   [ ] `JobService` validates offer-related requests
-   [ ] Manual validation logic removed from services

### Phase 3 Completion Criteria

-   [ ] XSS prevention rules added to content validators
-   [ ] Length/range limits enforced for all string and numeric fields
-   [ ] Nested object validation (e.g., `MediaObjectRequest`) implemented
-   [ ] Business rule validation (e.g., budget ranges) added

---

## 🎓 Developer Guidelines

### When Adding a New Request DTO

1. **Create the Request class:**

    ```csharp
    // ExpertBridge.Core/Requests/NewFeature/NewFeatureRequest.cs
    namespace ExpertBridge.Core.Requests.NewFeature;

    public class NewFeatureRequest
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
    ```

2. **Create the Validator:**

    ```csharp
    // ExpertBridge.Core/Requests/NewFeature/NewFeatureRequestValidator.cs
    public class NewFeatureRequestValidator : AbstractValidator<NewFeatureRequest>
    {
        public NewFeatureRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Value)
                .GreaterThan(0);
        }
    }
    ```

3. **Validator Registration (Automatic):**

    - Already registered via `AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>()`
    - No additional registration needed

4. **Use in Controller:**

    ```csharp
    [HttpPost("new-feature")]
    public async Task<IActionResult> CreateFeature([FromBody] NewFeatureRequest request)
    {
        // Validation happens automatically via middleware
        var result = await _service.CreateAsync(request);
        return Ok(result);
    }
    ```

5. **Service Method (Optional Explicit Validation):**

    ```csharp
    private readonly IValidator<NewFeatureRequest> _validator;

    public async Task<FeatureResponse> CreateAsync(NewFeatureRequest request)
    {
        // Only needed if controller-level validation is not enabled
        // OR for complex business rules
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // Business logic...
    }
    ```

---

## ⚠️ Migration Notes

### Breaking Changes

**None** - This is purely additive:

-   Existing endpoints gain validation automatically
-   Existing manual validation can coexist with FluentValidation
-   Error response format changes are non-breaking (still 400 status code)

### Backward Compatibility

-   ✅ Controllers without validation continue working
-   ✅ Services with manual validation continue working
-   ✅ Gradual migration supported (Phase 1 first, Phase 2 optional)

### Rollback Plan

If issues arise in Phase 1:

1. Remove `.AddFluentValidationAutoValidation()` from `Program.cs`
2. Revert `GlobalExceptionMiddleware` changes
3. System returns to manual validation only

---

## 📊 Success Metrics

### Code Quality Metrics

| Metric                         | Current | Target (Phase 1)      | Target (Phase 2) |
| ------------------------------ | ------- | --------------------- | ---------------- |
| **Services with Validation**   | 6.25%   | 100% (via middleware) | 100% (explicit)  |
| **Validation Coverage**        | ~5%     | 100%                  | 100%             |
| **Manual Validation Lines**    | ~50     | ~50 (coexist)         | ~0               |
| **Consistent Error Responses** | ❌ No   | ✅ Yes                | ✅ Yes           |

### Security Metrics

| Vulnerability               | Current Risk | After Phase 1 | After Phase 3 |
| --------------------------- | ------------ | ------------- | ------------- |
| **XSS Injection**           | 🔴 High      | 🟡 Medium     | 🟢 Low        |
| **SQL Injection (via ORM)** | 🟢 Low       | 🟢 Low        | 🟢 Low        |
| **Invalid Data in DB**      | 🔴 High      | 🟡 Medium     | 🟢 Low        |
| **Resource Exhaustion**     | 🟡 Medium    | 🟡 Medium     | 🟢 Low        |

---

## 🔗 References

### FluentValidation Documentation

-   [FluentValidation ASP.NET Core Integration](https://docs.fluentvalidation.net/en/latest/aspnet.html)
-   [Built-in Validators](https://docs.fluentvalidation.net/en/latest/built-in-validators.html)
-   [Custom Validators](https://docs.fluentvalidation.net/en/latest/custom-validators.html)

### ExpertBridge Architecture

-   `ExpertBridge.Core/` - Domain entities, DTOs, validators
-   `ExpertBridge.Application/` - Business logic, domain services
-   `ExpertBridge.Api/` - Controllers, middleware, API configuration

### Related Documentation

-   `.github/copilot-instructions.md` - Coding standards
-   `Directory.Packages.props` - FluentValidation package version

---

## 🎯 Next Steps

1. **Review this plan** with the development team
2. **Assign Phase 1** to a developer (estimated 1-2 hours)
3. **Test Phase 1** thoroughly before proceeding to Phase 2
4. **Schedule Phase 2** based on priority and available resources
5. **Document learnings** and update this plan as implementation progresses

---

**Document Version:** 1.0  
**Last Updated:** October 26, 2025  
**Owner:** Development Team  
**Status:** 📋 **READY FOR IMPLEMENTATION**
