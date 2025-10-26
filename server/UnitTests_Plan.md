# 🧪 Unit Testing Plan for FluentValidation Validators

**Generated:** October 26, 2025  
**Last Updated:** January 22, 2025 - 24/25 Validators Tested (96%)  
**Project:** ExpertBridge.Tests.Unit  
**Target:** ExpertBridge.Core Request Validators  
**Framework:** xUnit + FluentValidation.TestHelper  
**Status:** 🟢 **IN PROGRESS** - Phase 3 Implementation Ongoing

---

## 📋 Executive Summary

### Objectives

1. ✅ **100% Validator Coverage** - Create unit tests for all 25 FluentValidation validators
2. ✅ **One Test Class Per Validator** - Each validator gets its own dedicated test class
3. ✅ **Comprehensive Test Cases** - Test happy paths, edge cases, and validation failures
4. ✅ **Maintainable Tests** - Follow AAA pattern (Arrange, Act, Assert)
5. ✅ **Fast Execution** - No database, no HTTP, pure unit tests

### Testing Stack

| Technology                      | Purpose                     | Version |
| ------------------------------- | --------------------------- | ------- |
| **xUnit**                       | Test framework              | 2.9.3   |
| **FluentValidation.TestHelper** | Validator testing utilities | 11.11.0 |
| **Bogus**                       | Test data generation        | 35.6.5  |
| **Shouldly** (optional)         | Fluent assertion syntax     | 4.2.1   |

### Test Structure

```
ExpertBridge.Tests.Unit/
└── Core/
    └── Validation/
        ├── ApplyToJobPostingRequestValidatorTests.cs
        ├── CreateCommentRequestValidatorTests.cs
        ├── CreateJobOfferRequestValidatorTests.cs
        ├── CreateJobPostingRequestValidatorTests.cs
        ├── CreateMessageRequestValidatorTests.cs
        ├── CreatePostRequestValidatorTests.cs
        ├── EditCommentRequestValidatorTests.cs
        ├── EditJobPostingRequestValidatorTests.cs
        ├── EditPostRequestValidatorTests.cs
        ├── GeneratePresignedUrlsRequestValidatorTests.cs
        ├── InitiateJobOfferRequestValidatorTests.cs
        ├── JobPostingsPaginationRequestValidatorTests.cs
        ├── MediaObjectRequestValidatorTests.cs
        ├── OnboardUserRequestValidatorTests.cs
        ├── PatchCommentRequestValidatorTests.cs
        ├── PostsCursorRequestValidatorTests.cs
        ├── RegisterUserRequestValidatorTests.cs
        ├── RespondToJobOfferRequestValidatorTests.cs
        ├── SearchJobPostsRequestValidatorTests.cs
        ├── SearchPostRequestValidatorTests.cs
        ├── SearchUserRequestValidatorTests.cs
        ├── UpdateJobStatusRequestValidatorTests.cs
        ├── UpdateProfileRequestValidatorTests.cs
        ├── UpdateProfileSkillsRequestValidatorTests.cs
        └── UpdateUserRequestValidatorTests.cs
```

---

## 📊 Validators Inventory

### Content Creation Validators (9 validators)

| #   | Validator                          | Request Type              | Priority  | Enhanced (Phase 3)         | Tests Status    |
| --- | ---------------------------------- | ------------------------- | --------- | -------------------------- | --------------- |
| 1   | `CreatePostRequestValidator`       | `CreatePostRequest`       | 🔴 High   | ✅ Yes (XSS, Media)        | ✅ **40 tests** |
| 2   | `EditPostRequestValidator`         | `EditPostRequest`         | 🔴 High   | ✅ Yes (XSS)               | ✅ **37 tests** |
| 3   | `CreateCommentRequestValidator`    | `CreateCommentRequest`    | 🔴 High   | ✅ Yes (XSS)               | ✅ **38 tests** |
| 4   | `EditCommentRequestValidator`      | `EditCommentRequest`      | 🔴 High   | ✅ Yes (XSS)               | ✅ **24 tests** |
| 5   | `PatchCommentRequestValidator`     | `PatchCommentRequest`     | 🟡 Medium | ✅ Yes (XSS)               | ✅ **36 tests** |
| 6   | `CreateJobPostingRequestValidator` | `CreateJobPostingRequest` | 🔴 High   | ✅ Yes (XSS, Budget)       | ✅ **48 tests** |
| 7   | `EditJobPostingRequestValidator`   | `EditJobPostingRequest`   | 🔴 High   | ✅ Yes (XSS, Budget)       | ✅ **57 tests** |
| 8   | `CreateMessageRequestValidator`    | `CreateMessageRequest`    | 🔴 High   | ✅ Yes (XSS)               | ✅ **38 tests** |
| 9   | `MediaObjectRequestValidator`      | `MediaObjectRequest`      | 🔴 High   | ✅ Yes (File Upload, Path) | ✅ **22 tests** |

### Search/Query Validators (3 validators)

| #   | Validator                        | Request Type            | Priority  | Enhanced (Phase 3)      | Tests Status    |
| --- | -------------------------------- | ----------------------- | --------- | ----------------------- | --------------- |
| 10  | `SearchPostRequestValidator`     | `SearchPostRequest`     | 🟡 Medium | ✅ Yes (Length Limits)  | ✅ **25 tests** |
| 11  | `SearchUserRequestValidator`     | `SearchUserRequest`     | 🟡 Medium | ✅ Yes (Length Limits)  | ✅ **26 tests** |
| 12  | `SearchJobPostsRequestValidator` | `SearchJobPostsRequest` | 🟡 Medium | ✅ Yes (Length, Budget) | ✅ **49 tests** |

### Job/Offer Management Validators (5 validators)

| #   | Validator                           | Request Type               | Priority  | Enhanced (Phase 3)         | Tests Status    |
| --- | ----------------------------------- | -------------------------- | --------- | -------------------------- | --------------- |
| 13  | `CreateJobOfferRequestValidator`    | `CreateJobOfferRequest`    | 🔴 High   | ✅ Yes (GUID, XSS, Budget) | ✅ **59 tests** |
| 14  | `InitiateJobOfferRequestValidator`  | `InitiateJobOfferRequest`  | 🟡 Medium | ✅ Yes (XSS, Budget)       | ✅ **42 tests** |
| 15  | `RespondToJobOfferRequestValidator` | `RespondToJobOfferRequest` | 🟡 Medium | ✅ N/A (boolean only)      | ✅ **5 tests**  |
| 16  | `UpdateJobStatusRequestValidator`   | `UpdateJobStatusRequest`   | 🟡 Medium | ✅ N/A (enum only)         | ✅ **14 tests** |
| 17  | `ApplyToJobPostingRequestValidator` | `ApplyToJobPostingRequest` | 🟡 Medium | ✅ Yes (XSS, Budget)       | ✅ **35 tests** |

### User/Profile Management Validators (5 validators)

| #   | Validator                             | Request Type                 | Priority  | Enhanced (Phase 3)     | Tests Status    |
| --- | ------------------------------------- | ---------------------------- | --------- | ---------------------- | --------------- |
| 18  | `RegisterUserRequestValidator`        | `RegisterUserRequest`        | 🔴 High   | ✅ Yes (Email, Names)  | ✅ **44 tests** |
| 19  | `UpdateUserRequestValidator`          | `UpdateUserRequest`          | 🔴 High   | ✅ Yes (E.164, Names)  | ✅ **51 tests** |
| 20  | `OnboardUserRequestValidator`         | `OnboardUserRequest`         | 🟡 Medium | ✅ Yes (Tag Limits)    | ✅ **22 tests** |
| 21  | `UpdateProfileRequestValidator`       | `UpdateProfileRequest`       | 🔴 High   | ✅ Yes (XSS, Names)    | ✅ **69 tests** |
| 22  | `UpdateProfileSkillsRequestValidator` | `UpdateProfileSkillsRequest` | 🟡 Medium | ✅ N/A (ID collection) | ✅ **21 tests** |

### Pagination/Utility Validators (3 validators)

| #   | Validator                               | Request Type                   | Priority  | Enhanced (Phase 3)           | Tests Status    |
| --- | --------------------------------------- | ------------------------------ | --------- | ---------------------------- | --------------- |
| 23  | `PostsCursorRequestValidator`           | `PostsCursorRequest`           | 🟢 Low    | ✅ N/A (pagination only)     | ✅ **48 tests** |
| 24  | `JobPostingsPaginationRequestValidator` | `JobPostingsPaginationRequest` | 🟢 Low    | ✅ N/A (pagination only)     | ✅ **48 tests** |
| 25  | `GeneratePresignedUrlsRequestValidator` | `GeneratePresignedUrlsRequest` | 🟡 Medium | ✅ Yes (File Upload, Limits) | ✅ **30 tests** |

---

## 🎯 Test Case Categories

### For Each Validator, Test:

#### 1. **Happy Path Tests** (Valid Data)

-   ✅ Valid request with all required fields
-   ✅ Valid request with all optional fields populated
-   ✅ Valid request with minimum acceptable values
-   ✅ Valid request with maximum acceptable values

#### 2. **Null/Empty Tests**

-   ❌ Null request object (if applicable)
-   ❌ Required fields set to null
-   ❌ Required fields set to empty string
-   ❌ Required collections set to null/empty

#### 3. **Length Validation Tests**

-   ❌ String fields exceeding max length
-   ❌ String fields below min length (if applicable)
-   ✅ String fields at exact max length boundary
-   ✅ String fields at exact min length boundary

#### 4. **Format Validation Tests** (for enhanced validators)

-   ❌ Invalid email format
-   ❌ Invalid phone number format (E.164)
-   ❌ Invalid GUID format
-   ❌ Script tags in content (XSS)
-   ❌ HTML tags in titles
-   ❌ Dangerous patterns (javascript:, data:, event handlers)

#### 5. **Range Validation Tests**

-   ❌ Numeric values below minimum
-   ❌ Numeric values above maximum
-   ✅ Numeric values at boundaries
-   ❌ Negative values where not allowed

#### 6. **Business Rule Tests**

-   ❌ Budget exceeding $1,000,000
-   ❌ More than 10 media attachments
-   ❌ More than 20 tags
-   ❌ Query length < 2 or > 200 characters
-   ❌ Invalid file types for media uploads
-   ❌ Path traversal patterns in file keys

#### 7. **Conditional Validation Tests** (When/Unless)

-   ✅ Optional fields validated only when provided
-   ❌ Optional fields with invalid data when provided
-   ✅ Either/Or validation (e.g., PostId OR JobPostingId)

#### 8. **Collection Validation Tests**

-   ❌ Invalid items in collection
-   ✅ Valid collection with multiple items
-   ❌ Collection exceeding count limits
-   ✅ Empty collection when allowed

---

## 📝 Test Class Template

### Standard Test Class Structure

```csharp
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Requests.[RequestFolder];
using FluentValidation.TestHelper;

namespace ExpertBridge.Tests.Unit.Core.Validation;

/// <summary>
///     Unit tests for [ValidatorName] using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: required fields, length limits, format validation,
///     business rules, and edge cases.
/// </remarks>
public sealed class [ValidatorName]Tests
{
    private readonly [ValidatorName] _validator;

    public [ValidatorName]Tests()
    {
        _validator = new [ValidatorName]();
    }

    #region Happy Path Tests

    [Fact]
    public async Task Should_Pass_Validation_When_All_Required_Fields_Valid()
    {
        // Arrange
        var request = new [RequestType]
        {
            // Valid data
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region Required Field Tests

    [Fact]
    public async Task Should_Have_Error_When_[Field]_Is_Null()
    {
        // Arrange
        var request = new [RequestType]
        {
            [Field] = null
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.[Field])
            .WithErrorMessage("*cannot be null*");
    }

    [Fact]
    public async Task Should_Have_Error_When_[Field]_Is_Empty()
    {
        // Arrange
        var request = new [RequestType]
        {
            [Field] = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.[Field])
            .WithErrorMessage("*cannot be empty*");
    }

    #endregion

    #region Length Validation Tests

    [Fact]
    public async Task Should_Have_Error_When_[Field]_Exceeds_Max_Length()
    {
        // Arrange
        var request = new [RequestType]
        {
            [Field] = new string('a', [MaxLength + 1])
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.[Field])
            .WithErrorMessage("*cannot be longer than*");
    }

    [Fact]
    public async Task Should_Pass_When_[Field]_At_Max_Length()
    {
        // Arrange
        var request = new [RequestType]
        {
            [Field] = new string('a', [MaxLength])
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.[Field]);
    }

    #endregion

    #region Format Validation Tests (Phase 3 Enhanced Validators)

    [Fact]
    public async Task Should_Have_Error_When_[Field]_Contains_Script_Tags()
    {
        // Arrange
        var request = new [RequestType]
        {
            [Field] = "Valid content <script>alert('xss')</script> more content"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.[Field])
            .WithErrorMessage("*cannot contain script tags*");
    }

    #endregion

    #region Business Rule Tests

    [Fact]
    public async Task Should_Have_Error_When_Budget_Exceeds_Maximum()
    {
        // Arrange
        var request = new [RequestType]
        {
            Budget = 1_000_001m
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Budget)
            .WithErrorMessage("*cannot exceed 1,000,000*");
    }

    #endregion
}
```

---

## 🚀 Implementation Strategy

### Phase-by-Phase Approach

#### **Phase 1: Setup and Infrastructure** (30 minutes)

**Tasks:**

1. ✅ Add `FluentValidation.TestHelper` package to `Directory.Packages.props`
2. ✅ Add `FluentAssertions` package (optional, for better assertions)
3. ✅ Update `ExpertBridge.Tests.Unit.csproj` with package references
4. ✅ Create base test class structure (if needed)
5. ✅ Update `global.cs` with common usings

**Files to Modify:**

-   `/Directory.Packages.props`
-   `/ExpertBridge.Tests.Unit/ExpertBridge.Tests.Unit.csproj`
-   `/ExpertBridge.Tests.Unit/global.cs`

**Expected Output:**

-   Test project ready with all dependencies
-   Common imports available globally

---

#### **Phase 2: High-Priority Validators** (3-4 hours)

**Order of Implementation:**

1. `CreatePostRequestValidator` (Enhanced - XSS, Media)
2. `CreateCommentRequestValidator` (Enhanced - XSS)
3. `CreateJobOfferRequestValidator` (Enhanced - GUID, XSS, Budget)
4. `CreateJobPostingRequestValidator` (Enhanced - XSS, Budget)
5. `UpdateUserRequestValidator` (Enhanced - E.164, Names)
6. `RegisterUserRequestValidator`
7. `CreateMessageRequestValidator`
8. `MediaObjectRequestValidator` (Enhanced - File Upload)
9. `UpdateProfileRequestValidator`

**Estimated:** 20-30 minutes per validator

---

#### **Phase 3: Medium-Priority Validators** (2-3 hours)

**Order of Implementation:** 10. `SearchPostRequestValidator` (Enhanced - Length Limits) 11. `SearchUserRequestValidator` (Enhanced - Length Limits) 12. `SearchJobPostsRequestValidator` (Enhanced - Length, Budget) 13. `EditPostRequestValidator` 14. `EditCommentRequestValidator` (Enhanced - XSS) 15. `EditJobPostingRequestValidator` 16. `OnboardUserRequestValidator` (Enhanced - Tag Limits) 17. `InitiateJobOfferRequestValidator` 18. `RespondToJobOfferRequestValidator` 19. `UpdateJobStatusRequestValidator`

**Estimated:** 15-20 minutes per validator

---

#### **Phase 4: Low-Priority Validators** (1-2 hours)

**Order of Implementation:** 20. `ApplyToJobPostingRequestValidator` 21. `PatchCommentRequestValidator` 22. `UpdateProfileSkillsRequestValidator` 23. `PostsCursorRequestValidator` 24. `JobPostingsPaginationRequestValidator` 25. `GeneratePresignedUrlsRequestValidator`

**Estimated:** 10-15 minutes per validator

---

## 📋 Test Coverage Goals

### Coverage Metrics

| Metric                       | Target     | Measurement                      |
| ---------------------------- | ---------- | -------------------------------- |
| **Validator Coverage**       | 100%       | 25/25 validators tested          |
| **Code Coverage**            | ≥80%       | Lines of validator code executed |
| **Validation Rule Coverage** | 100%       | Every validation rule tested     |
| **Edge Case Coverage**       | ≥90%       | Boundary values, nulls, extremes |
| **Test Execution Time**      | <5 seconds | Fast feedback loop               |

### Per-Validator Test Count Estimates

| Validator Complexity         | Test Count Range | Examples                                |
| ---------------------------- | ---------------- | --------------------------------------- |
| **Simple** (3-5 rules)       | 8-12 tests       | Pagination validators                   |
| **Medium** (6-10 rules)      | 12-20 tests      | Search validators, basic CRUD           |
| **Complex** (11-15 rules)    | 20-30 tests      | Enhanced validators (XSS, etc.)         |
| **Very Complex** (16+ rules) | 30-40 tests      | CreatePostRequestValidator (media, XSS) |

**Estimated Total Tests:** 400-500 tests across all 25 validators

---

## 🔍 Example: CreatePostRequestValidator Tests

### Complete Test Suite Example

```csharp
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Requests.CreatePost;
using FluentValidation.TestHelper;

namespace ExpertBridge.Tests.Unit.Core.Validation;

/// <summary>
///     Unit tests for CreatePostRequestValidator covering XSS prevention,
///     media validation, and standard field validation.
/// </summary>
public sealed class CreatePostRequestValidatorTests
{
    private readonly CreatePostRequestValidator _validator;

    public CreatePostRequestValidatorTests()
    {
        _validator = new CreatePostRequestValidator();
    }

    #region Happy Path Tests

    [Fact]
    public async Task Should_Pass_When_All_Required_Fields_Valid()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = "Valid Post Title",
            Content = "This is valid post content without any script tags.",
            Media = null
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_When_Valid_With_Media()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = "Post with Media",
            Content = "Content here",
            Media = new List<MediaObjectRequest>
            {
                new() { MediaType = "image/jpeg", FileKey = "valid-file-key.jpg" }
            }
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region Title Validation Tests

    [Fact]
    public async Task Should_Have_Error_When_Title_Is_Null()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = null,
            Content = "Valid content"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("*cannot be null*");
    }

    [Fact]
    public async Task Should_Have_Error_When_Title_Is_Empty()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = string.Empty,
            Content = "Valid content"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("*cannot be empty*");
    }

    [Fact]
    public async Task Should_Have_Error_When_Title_Exceeds_Max_Length()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = new string('a', 201), // Assuming max is 200
            Content = "Valid content"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("*cannot be longer than*");
    }

    [Fact]
    public async Task Should_Have_Error_When_Title_Contains_Script_Tags()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = "Bad Title <script>alert('xss')</script>",
            Content = "Valid content"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("*cannot contain script tags*");
    }

    [Fact]
    public async Task Should_Have_Error_When_Title_Contains_HTML_Tags()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = "Title with <b>HTML</b> tags",
            Content = "Valid content"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("*cannot contain HTML tags*");
    }

    #endregion

    #region Content Validation Tests

    [Fact]
    public async Task Should_Have_Error_When_Content_Is_Null()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = "Valid Title",
            Content = null
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public async Task Should_Have_Error_When_Content_Contains_Script_Tags()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = "Valid Title",
            Content = "Content with <script>malicious code</script>"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("*cannot contain script tags*");
    }

    [Fact]
    public async Task Should_Have_Error_When_Content_Contains_Dangerous_Patterns()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = "Valid Title",
            Content = "Content with javascript:alert('xss') link"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("*dangerous patterns*");
    }

    [Theory]
    [InlineData("data:text/html,<script>alert('xss')</script>")]
    [InlineData("<div onclick='alert()'>Click me</div>")]
    [InlineData("<img onload='alert()' src='x'>")]
    public async Task Should_Have_Error_When_Content_Contains_Event_Handlers(string dangerousContent)
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = "Valid Title",
            Content = dangerousContent
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    #endregion

    #region Media Validation Tests

    [Fact]
    public async Task Should_Have_Error_When_Media_Count_Exceeds_10()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = "Valid Title",
            Content = "Valid content",
            Media = Enumerable.Range(1, 11)
                .Select(i => new MediaObjectRequest
                {
                    MediaType = "image/jpeg",
                    FileKey = $"file{i}.jpg"
                })
                .ToList()
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Media)
            .WithErrorMessage("*cannot exceed 10*");
    }

    [Fact]
    public async Task Should_Pass_When_Media_Count_Is_10()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = "Valid Title",
            Content = "Valid content",
            Media = Enumerable.Range(1, 10)
                .Select(i => new MediaObjectRequest
                {
                    MediaType = "image/jpeg",
                    FileKey = $"file{i}.jpg"
                })
                .ToList()
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Media);
    }

    [Fact]
    public async Task Should_Have_Error_When_Media_Has_Invalid_Type()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = "Valid Title",
            Content = "Valid content",
            Media = new List<MediaObjectRequest>
            {
                new() { MediaType = "application/exe", FileKey = "virus.exe" }
            }
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Media[0].MediaType");
    }

    [Fact]
    public async Task Should_Have_Error_When_Media_Key_Contains_Path_Traversal()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = "Valid Title",
            Content = "Valid content",
            Media = new List<MediaObjectRequest>
            {
                new() { MediaType = "image/jpeg", FileKey = "../../../etc/passwd" }
            }
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Media[0].FileKey");
    }

    [Theory]
    [InlineData("file\\path\\traversal.jpg")]
    [InlineData("/absolute/path/file.jpg")]
    [InlineData("..\\windows\\system32\\file.jpg")]
    public async Task Should_Have_Error_When_Media_Key_Has_Unsafe_Characters(string unsafeKey)
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = "Valid Title",
            Content = "Valid content",
            Media = new List<MediaObjectRequest>
            {
                new() { MediaType = "image/jpeg", FileKey = unsafeKey }
            }
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Media[0].FileKey");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task Should_Pass_When_Media_Is_Null()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = "Valid Title",
            Content = "Valid content",
            Media = null
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Media);
    }

    [Fact]
    public async Task Should_Pass_When_Media_Is_Empty_List()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            Title = "Valid Title",
            Content = "Valid content",
            Media = new List<MediaObjectRequest>()
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Media);
    }

    #endregion
}
```

---

## 📦 Package Dependencies

### Required Packages

Add to `Directory.Packages.props`:

```xml
<PackageVersion Include="FluentValidation.TestHelper" Version="11.11.0"/>
<PackageVersion Include="Shouldly" Version="4.2.1"/>
```

Add to `ExpertBridge.Tests.Unit.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="FluentValidation.TestHelper" />
  <PackageReference Include="Shouldly" />
  <PackageReference Include="Bogus" />
</ItemGroup>
```

Update `global.cs`:

```csharp
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using FluentValidation.TestHelper;
global using Shouldly;
global using Xunit;
```

---

## 📊 Time Estimates

### Total Time Breakdown

| Phase                | Validators | Avg Time | Total Time  |
| -------------------- | ---------- | -------- | ----------- |
| **Setup**            | -          | -        | 30 min      |
| **High Priority**    | 9          | 25 min   | 3.75 hours  |
| **Medium Priority**  | 10         | 18 min   | 3 hours     |
| **Low Priority**     | 6          | 12 min   | 1.25 hours  |
| **Review & Cleanup** | -          | -        | 30 min      |
| **TOTAL**            | **25**     | -        | **9 hours** |

### Per-Session Breakdown

**Session 1** (2-3 hours):

-   Setup + Phase 2 (validators 1-5)

**Session 2** (2-3 hours):

-   Phase 2 continued (validators 6-9)

**Session 3** (2-3 hours):

-   Phase 3 (validators 10-19)

**Session 4** (1-2 hours):

-   Phase 4 (validators 20-25) + Review

---

## ✅ Acceptance Criteria

### Per Test Class

-   ✅ One test class per validator
-   ✅ All validation rules have corresponding tests
-   ✅ Both positive and negative test cases
-   ✅ Edge cases covered (boundaries, nulls, empty)
-   ✅ Clear test names following pattern: `Should_[Expected]_When_[Condition]`
-   ✅ AAA pattern (Arrange, Act, Assert)
-   ✅ No test dependencies (each test isolated)
-   ✅ Fast execution (<100ms per test)

### Overall Project

-   ✅ 100% validator coverage (25/25)
-   ✅ All tests pass on first run
-   ✅ No compiler warnings
-   ✅ Consistent code style
-   ✅ XML documentation on test classes
-   ✅ Tests organized in logical regions

---

## 🎯 Next Steps

### After Plan Approval

1. **Setup Phase:**

    - Add packages to Directory.Packages.props
    - Update test project file
    - Update global.cs with common imports

2. **Implementation:**

    - Start with CreatePostRequestValidatorTests (most complex)
    - Use it as template for other validators
    - Iterate through phases 2-4

3. **Review:**

    - Run all tests
    - Check code coverage
    - Review test quality
    - Document any edge cases discovered

4. **Documentation:**
    - Update ValidationIntegrationPlan.md with Phase 4 completion
    - Add testing section to README
    - Create test execution guide

---

## 📝 Notes

-   **FluentValidation.TestHelper** provides `TestValidateAsync()` and `ShouldHaveValidationErrorFor()` methods
-   **Bogus** can generate realistic test data if needed (names, emails, etc.)
-   **Shouldly** provides better assertion syntax with natural language (optional but recommended)
-   Tests should be **deterministic** - same input always produces same result
-   No mocking needed - validators are pure functions
-   Each test class is **completely independent** - can be run in any order

---

## 🚀 Phase 4 Implementation Progress

### Current Status: IN PROGRESS ⚙️

**Started:** October 26, 2025  
**Completion:** 17/25 validators (68%)  
**Total Tests:** ~627 tests passing

### Completed Test Classes ✅

#### 1. CreatePostRequestValidator ✅ (40 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/CreatePostRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (40/40)  
**Coverage:** Title, Content, Media validation with XSS prevention, dangerous patterns, path traversal, business rules

**Test Breakdown:**

-   **Happy Path (4 tests):** Valid scenarios with/without media
-   **Title Validation (8 tests):** Null, empty, max length (256), XSS prevention, HTML tag detection
-   **Content Validation (7 tests):** Null, empty, max length (5000), script tags, dangerous patterns, event handlers
-   **Media Validation (7 tests):** Count limits (≤10), file types, path traversal, unsafe characters
-   **Edge Cases (5 tests):** Whitespace, Unicode, case sensitivity, script tag attributes
-   **Parameterized Tests:** 9 test cases via `[Theory]` with `[InlineData]`

**Key Validations Tested:**

-   ✅ XSS prevention (script tags, HTML tags in title)
-   ✅ Dangerous patterns (javascript:, data:, event handlers)
-   ✅ Path traversal protection (../, ..\, absolute paths)
-   ✅ Business rules (max 10 media attachments)
-   ✅ Length limits (Title: 256, Content: 5000)
-   ✅ Nested validator integration (MediaObjectRequestValidator)

**Test Execution:**

```bash
dotnet test ExpertBridge.Tests.Unit/ExpertBridge.Tests.Unit.csproj
# Result: 40 passed, 0 failed, 1.3s execution time
```

---

#### 2. EditPostRequestValidator ✅ (37 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/EditPostRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (37/37)  
**Coverage:** Optional Title, Content, Media validation with conditional logic, XSS prevention

---

#### 3. CreateCommentRequestValidator ✅ (38 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/CreateCommentRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (38/38)  
**Coverage:** PostId, Content, Media validation with XSS prevention, dangerous patterns

---

#### 4. EditCommentRequestValidator ✅ (24 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/EditCommentRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (24/24)  
**Coverage:** Optional Content validation with XSS prevention, dangerous patterns

---

#### 5. PatchCommentRequestValidator ✅ (36 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/PatchCommentRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (36/36)  
**Coverage:** Partial update validation with optional Content, IsDeleted, conditional validation

---

#### 6. GeneratePresignedUrlsRequestValidator ✅ (30 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/GeneratePresignedUrlsRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (30/30)  
**Coverage:** FileMetadata collection validation with limits, nested validators, path traversal protection

---

#### 7. ApplyToJobPostingRequestValidator ✅ (35 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/ApplyToJobPostingRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (35/35)  
**Coverage:** JobPostingId, CoverLetter, ProposedBudget validation with XSS prevention, budget limits

---

#### 8. CreateJobPostingRequestValidator ✅ (48 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/CreateJobPostingRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (48/48)  
**Coverage:** Area, Title, Content, Budget validation with XSS prevention, HTML tag detection, budget limits (0-1,000,000)

---

#### 9. EditJobPostingRequestValidator ✅ (57 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/EditJobPostingRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (57/57)  
**Coverage:** Optional Title, Content, Budget, Area validation with conditional logic, XSS prevention, budget limits

---

#### 10. CreateMessageRequestValidator ✅ (38 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/CreateMessageRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (38/38)  
**Coverage:** ChatId, Content validation with XSS prevention, dangerous patterns for chat messages

---

#### 11. MediaObjectRequestValidator ✅ (25 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/MediaObjectRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (25/25)  
**Coverage:** S3 media metadata with Key and Type validation for images (jpeg, png, gif, webp), videos (mp4, webm), and documents (pdf only)

**Test Breakdown:**

-   **Happy Path (5 tests):** Valid requests for images, videos, documents with proper S3 keys
-   **Key Validation (3 tests):** Null, empty, whitespace handling with correct error message
-   **Type Validation (3 tests):** Null, empty, whitespace handling with correct error message
-   **Allowed Media Types (3 tests):** Images, videos, PDF documents
-   **Unsupported Media Types (5 tests):** Audio (mp3), archives (zip), Word documents (docx), text files (txt), unsupported video formats (mov/quicktime)
-   **Edge Cases (11 tests):** S3 paths with prefixes, GUIDs, timestamps, special characters, nested directories, long keys, short keys, multiple errors
-   **Parameterized Tests:** Various image and video types tested

**Key Validations Tested:**

-   ✅ Media type restriction: Only allows image/jpeg, image/png, image/gif, image/webp, video/mp4, video/webm, application/pdf
-   ✅ Unsupported types properly rejected: audio files, archives, Word docs, text files, QuickTime videos
-   ✅ S3 key validation: Max length 500, safe filename patterns
-   ✅ Path traversal protection: Prevents "..", "\\", and absolute paths
-   ✅ Error messages match validator: "Media key is required", "Media type is required"

**Test Execution:**

```bash
dotnet test --filter "FullyQualifiedName~MediaObjectRequestValidatorTests"
# Result: 25 passed, 0 failed, 1.6s execution time
```

---

#### 12. CreateJobOfferRequestValidator ✅ (59 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/CreateJobOfferRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (59/59)  
**Coverage:** Title, Description, Area, Budget, WorkerId validation with GUID format, XSS prevention, HTML tag detection, budget limits (0-1,000,000)

**Test Breakdown:**

-   **Happy Path (5 tests):** Valid requests with various budget values and field combinations
-   **Title Validation (6 tests):** Null, empty, max length (500), script tags, HTML tags
-   **Description Validation (8 tests):** Null, empty, max length (1000), script tags, dangerous patterns (javascript:, data:, event handlers)
-   **Area Validation (6 tests):** Null, empty, max length (450), GUID format validation
-   **Budget Validation (4 tests):** Negative values, exceeds max (1,000,000), boundaries
-   **WorkerId Validation (6 tests):** Null, empty, max length (450), GUID format validation
-   **Edge Cases (7 tests):** Unicode, multiple errors, decimal budgets, HTML entities, case sensitivity, same Area and WorkerId
-   **Parameterized Tests:** 16 test cases via `[Theory]` with `[InlineData]`

**Key Validations Tested:**

-   ✅ GUID format validation for Area and WorkerId (regex: `^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$`)
-   ✅ XSS prevention for Title and Description (script tags, HTML tags)
-   ✅ HTML tag detection specifically for Title
-   ✅ Dangerous pattern detection for Description (javascript:, data:, event handlers)
-   ✅ Budget range validation (0 to 1,000,000)
-   ✅ Length limits (Title: 500, Description: 1000, Area/WorkerId: 450)

**Test Execution:**

```bash
dotnet test --filter "FullyQualifiedName~CreateJobOfferRequestValidatorTests"
# Result: 59 passed, 0 failed, 1.6s execution time
```

---

#### 13. SearchPostRequestValidator ✅ (25 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/SearchPostRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (25/25)  
**Coverage:** Query validation (required, length limits), Limit validation (optional, range), edge cases for post search

**Test Breakdown:**

-   **Happy Path (6 tests):** Valid queries, with/without Limit, boundary values (2-200 chars, Limit 1-100)
-   **Query Validation (5 tests):** Null, empty, whitespace, too short (<2 chars), too long (>200 chars)
-   **Limit Validation (4 tests):** Null (allowed), zero (rejected), negative (rejected), exceeds max (>100)
-   **Edge Cases (10 tests):** Special characters, Unicode, emojis, numbers, multi-word queries

**Key Validations Tested:**

-   ✅ Query required with min length 2, max length 200 characters
-   ✅ Limit optional with range 1-100
-   ✅ Supports special characters, Unicode, emojis in search queries
-   ✅ Error messages: "Query cannot be null/empty", "Query must be at least 2 characters long", "Query cannot exceed 200 characters"

**Test Execution:**

```bash
dotnet test --filter "FullyQualifiedName~SearchPostRequestValidatorTests"
# Result: 25 passed, 0 failed, 0.5s execution time
```

---

#### 14. SearchUserRequestValidator ✅ (26 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/SearchUserRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (26/26)  
**Coverage:** Query validation (required, length limits), Limit validation (optional, range), edge cases for user search

**Test Breakdown:**

-   **Happy Path (6 tests):** Valid queries (names, usernames), with/without Limit, boundary values (2-200 chars, Limit 1-100)
-   **Query Validation (5 tests):** Null, empty, whitespace, too short (<2 chars), too long (>200 chars)
-   **Limit Validation (4 tests):** Null (allowed), zero (rejected), negative (rejected), exceeds max (>100)
-   **Edge Cases (11 tests):** Usernames (@username), email addresses, full names, Unicode, special characters (O'Brien-Smith), numbers

**Key Validations Tested:**

-   ✅ Query required with min length 2, max length 200 characters
-   ✅ Limit optional with range 1-100
-   ✅ Supports various user search patterns: usernames, emails, full names, Unicode characters
-   ✅ Error messages: "Query cannot be null/empty", "Query must be at least 2 characters long", "Query cannot exceed 200 characters"

**Test Execution:**

```bash
dotnet test --filter "FullyQualifiedName~SearchUserRequestValidatorTests"
# Result: 26 passed, 0 failed, 0.5s execution time
```

---

#### 15. SearchJobPostsRequestValidator ✅ (49 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/SearchJobPostsRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (49/49)  
**Coverage:** Query validation (required, length limits), Limit validation (optional, range), budget filters (MinBudget, MaxBudget), budget range logic, edge cases

**Test Breakdown:**

-   **Happy Path (9 tests):** Valid queries, all combinations of optional fields (Limit, MinBudget, MaxBudget), equal budgets, zero MinBudget, max budget values
-   **Query Validation (5 tests):** Null, empty, whitespace, too short (<2 chars), too long (>200 chars)
-   **Limit Validation (6 tests):** Null (allowed), boundaries (1-100), zero (rejected), negative (rejected), exceeds max (>100)
-   **MinBudget Validation (5 tests):** Null (allowed), zero (allowed), max (1,000,000), negative (rejected), exceeds max
-   **MaxBudget Validation (5 tests):** Null (allowed), zero (allowed), max (1,000,000), negative (rejected), exceeds max
-   **Budget Range Validation (4 tests):** MinBudget > MaxBudget (rejected), only MinBudget, only MaxBudget, both null
-   **Edge Cases (15 tests):** Job-related keywords, special characters (C# .NET), Unicode, decimal budgets, all optional fields null

**Key Validations Tested:**

-   ✅ Query required with min length 2, max length 200 characters
-   ✅ Limit optional with range 1-100
-   ✅ MinBudget and MaxBudget optional with range 0-1,000,000
-   ✅ Budget range logic: MinBudget ≤ MaxBudget (when both provided)
-   ✅ Supports decimal budget values
-   ✅ Error messages: "Query cannot be null/empty", "MinBudget cannot be greater than MaxBudget", "MinBudget/MaxBudget cannot exceed 1,000,000"

**Test Execution:**

```bash
dotnet test --filter "FullyQualifiedName~SearchJobPostsRequestValidatorTests"
# Result: 49 passed, 0 failed, 0.6s execution time
```

---

#### 16. InitiateJobOfferRequestValidator ✅ (42 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/InitiateJobOfferRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (42/42)  
**Coverage:** ContractorProfileId, Title, Description, ProposedRate, JobPostingId validation with XSS prevention, dangerous patterns, budget limits

**Test Breakdown:**

-   **Happy Path (6 tests):** Valid requests with/without optional JobPostingId, boundary ProposedRate values
-   **ContractorProfileId Validation (3 tests):** Null, empty, max length (450)
-   **Title Validation (5 tests):** Null, empty, max length (500), script tags, HTML tags
-   **Description Validation (8 tests):** Null, empty, max length (1000), script tags, dangerous patterns (javascript:, data:, event handlers)
-   **ProposedRate Validation (3 tests):** Negative values, exceeds max (1,000,000), boundaries
-   **JobPostingId Validation (3 tests):** Optional null (allowed), max length when provided, empty string
-   **Edge Cases (5 tests):** Unicode in Title/Description, multiple errors, decimal rates, all fields valid
-   **Parameterized Tests:** 9 test cases via `[Theory]` with `[InlineData]`

**Key Validations Tested:**

-   ✅ XSS prevention for Title (script tags, HTML tags)
-   ✅ Dangerous pattern detection for Description (javascript:, data:, event handlers)
-   ✅ ProposedRate range validation (0.01 to 1,000,000)
-   ✅ Optional JobPostingId with max length 450
-   ✅ Length limits (ContractorProfileId: 450, Title: 500, Description: 1000)
-   ✅ **CRITICAL FIX:** Conditional regex validation using `When(x => x.Field != null)` to prevent ArgumentNullException

**Test Execution:**

```bash
dotnet test --filter "FullyQualifiedName~InitiateJobOfferRequestValidatorTests"
# Result: 42 passed, 0 failed (after fixing null handling in validator)
```

**Validator Bug Fixed:**  
Original validator threw `ArgumentNullException` when testing null Title/Description with regex patterns. Fixed by wrapping regex validations in `When(x => x.Title != null)` and `When(x => x.Description != null)` blocks.

---

#### 17. RespondToJobOfferRequestValidator ✅ (5 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/RespondToJobOfferRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (5/5)  
**Coverage:** Simple boolean Accept field validation (not null check)

**Test Breakdown:**

-   **Happy Path (2 tests):** Valid Accept=true and Accept=false scenarios
-   **Accept Validation (1 test):** Null check (rejected)
-   **Edge Cases (2 tests):** Valid true/false values

**Key Validations Tested:**

-   ✅ Accept field must not be null (simple boolean validation)
-   ✅ Error message: "Accept field is required"

**Test Execution:**

```bash
dotnet test --filter "FullyQualifiedName~RespondToJobOfferRequestValidatorTests"
# Result: 5 passed, 0 failed
```

**Notes:** Simplest validator in the project - only validates that the Accept boolean is not null.

---

#### 18. UpdateJobStatusRequestValidator ✅ (14 tests)

**File:** `ExpertBridge.Tests.Unit/Core/Validation/UpdateJobStatusRequestValidatorTests.cs`  
**Status:** ✅ All tests passing (14/14)  
**Coverage:** Status string/enum validation with length limits, null/empty checks

**Test Breakdown:**

-   **Happy Path (4 tests):** Valid status values in various formats (PascalCase, UPPERCASE, lowercase, with spaces)
-   **Status Validation (4 tests):** Null, empty, whitespace, exceeds max length (128)
-   **Edge Cases (5 tests):** Single character, exactly 128 chars, numbers in status, mixed case
-   **Parameterized Tests:** 1 test case with multiple status format variations

**Key Validations Tested:**

-   ✅ Status required (not null, not empty, not whitespace)
-   ✅ Max length 128 characters (GlobalEntitiesConstraints.MaxEnumsLength)
-   ✅ Supports any string format (PascalCase, UPPERCASE, lowercase, spaces, numbers)
-   ✅ Error messages: "Status is required", "Status must not exceed 128 characters"

**Test Execution:**

```bash
dotnet test --filter "FullyQualifiedName~UpdateJobStatusRequestValidatorTests"
# Result: 14 passed, 0 failed
```

**Notes:** Simple enum-style validator with max length constraint, no XSS validation needed.

---

### Remaining Test Classes ⏳

**High Priority (5 remaining - User/Profile Management):**

-   ⏳ RegisterUserRequestValidator
-   ⏳ UpdateUserRequestValidator
-   ⏳ OnboardUserRequestValidator
-   ⏳ UpdateProfileRequestValidator
-   ⏳ UpdateProfileSkillsRequestValidator

**Low Priority (2 remaining - Pagination/Utility):**

-   ⏳ PostsCursorRequestValidator
-   ⏳ JobPostingsPaginationRequestValidator

**Low Priority (1 remaining):**

-   ⏳ UpdateProfileSkillsRequestValidator
-   ⏳ PostsCursorRequestValidator
-   ⏳ JobPostingsPaginationRequestValidator

---

## 🎯 Phase 3 Completion Status

### All Validators Enhanced! ✅

**Status as of October 26, 2025:**

| Category                 | Enhanced  | Not Applicable | Total  |
| ------------------------ | --------- | -------------- | ------ |
| **Content Creation**     | 9/9       | 0              | 9      |
| **Search/Query**         | 3/3       | 0              | 3      |
| **Job/Offer Management** | 3/5       | 2              | 5      |
| **User/Profile**         | 3/5       | 2              | 5      |
| **Pagination/Utility**   | 1/3       | 2              | 3      |
| **TOTAL**                | **19/25** | **6/25**       | **25** |

**Legend:**

-   ✅ **Enhanced (19):** Validators with Phase 3 security enhancements (XSS, path traversal, business rules)
-   ✅ **Not Applicable (6):** Validators that don't require content sanitization (booleans, enums, pagination, IDs only)
-   **Total Coverage:** 25/25 validators (100%)

### Security Enhancements Summary

**Applied Enhancements:**

-   ✅ **XSS Prevention:** 14 validators with script tag and dangerous pattern detection
-   ✅ **Path Traversal Protection:** 2 validators (MediaObject, GeneratePresignedUrls)
-   ✅ **Budget Limits:** 6 validators capped at $1,000,000
-   ✅ **Query Length Limits:** 3 validators (2-200 characters)
-   ✅ **File Upload Limits:** 2 validators (count, size, type validation)
-   ✅ **Name Pattern Validation:** 3 validators (letters, spaces, hyphens, apostrophes only)
-   ✅ **Email Validation:** 2 validators (enhanced format checks)
-   ✅ **Resource Limits:** Tag limits, media limits, file count limits

**Performance Optimizations:**

-   ✅ All regex patterns use C# 11 `[GeneratedRegex]` for compile-time optimization
-   ✅ 1000ms timeout protection on all regex patterns

### Implementation Started! 🚀

**All prerequisites complete:**

1. ✅ Phase 1: Global API validation infrastructure
2. ✅ Phase 2: Service-level validation
3. ✅ Phase 3: Enhanced security rules (100% coverage)
4. 🚀 Phase 4: Unit tests (IN PROGRESS - 1/25 completed)

**Progress:** CreatePostRequestValidator completed with 40 comprehensive tests (all passing).

**Next Steps:**

1. Continue with remaining high-priority validators (8 validators)
2. Implement medium-priority validators (10 validators)
3. Complete low-priority validators (6 validators)
4. Run full test suite and collect code coverage metrics
5. Update documentation with final test coverage statistics

**Estimated Time Remaining:** 6-7 hours for all remaining validators

---

**Implementation in progress. Systematic development of remaining 5 validator test classes following the established patterns.**

**Progress Summary:**

-   ✅ Content Creation: 9/9 validators completed (100%)
-   ✅ Search/Query: 3/3 validators completed (100%)
-   ✅ Job/Offer Management: 5/5 validators completed (100%)
-   ✅ User/Profile: 5/5 validators completed (100%)
-   ✅ Pagination/Utility: 3/3 validators completed (100%)

**Completed (303 tests added):**

**Session 1 (164 tests):**

1. ✅ RegisterUserRequestValidator (High Priority, Email + Names validation) - **44 tests**
2. ✅ UpdateUserRequestValidator (High Priority, E.164 + Names validation) - **51 tests**
3. ✅ UpdateProfileRequestValidator (High Priority, XSS + Names + Skills validation) - **69 tests**

**Session 2 (139 tests):**

4. ✅ OnboardUserRequestValidator (Medium Priority, Tag collection validation) - **22 tests**
5. ✅ UpdateProfileSkillsRequestValidator (Medium Priority, Skills collection validation) - **21 tests**
6. ✅ PostsCursorRequestValidator (Low Priority, Pagination validation) - **48 tests**
7. ✅ JobPostingsPaginationRequestValidator (Low Priority, Pagination validation) - **48 tests**

**Next Validators:**

Only 1 validator remaining: GeneratePresignedUrlsRequestValidator (already has 30 tests according to plan)

**Estimated Time Remaining:** 0 hours - All validators complete! 🎉
