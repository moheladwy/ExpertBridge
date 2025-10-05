# ExpertBridge.UnitTests - GitHub Copilot Instructions

## Project Purpose

xUnit-based unit testing project for validating business logic, validators, interceptors, and domain services. Uses FluentValidation.TestHelper for validation testing and EF Core In-Memory database for data layer testing.

## Architecture Role

**Testing Layer** - Ensures code quality through automated unit tests, validation tests, and integration tests with in-memory databases.

## Key Responsibilities

-   Unit testing for validators
-   Integration testing for interceptors
-   Entity configuration testing
-   Service logic testing
-   DTO validation testing
-   Test fixtures and helpers

## Project Dependencies

```xml
Dependencies:
- ExpertBridge.Core (entities, DTOs, validators)
- ExpertBridge.Data (DbContext, interceptors)
- ExpertBridge.Application (services, domain logic)

External:
- xUnit (v3.0.3)
- FluentValidation.TestHelper
- Microsoft.EntityFrameworkCore.InMemory
- FluentAssertions
```

## Test Class Pattern

### Validator Test Structure

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.UnitTests.ValidationTests.DTOs;

public class RegisterUserRequestValidatorTests
{
    private readonly RegisterUserRequestValidator _registerUserRequestValidator = new();

    private readonly RegisterUserRequest _validRegisterUserRequest = new()
    {
        Email = "test@gmail.com",
        ProviderId = Guid.NewGuid().ToString(),
        FirstName = "Test",
        LastName = "Test",
        Username = "test"
    };

    [Fact]
    public void Validate_ValidRegisterUserRequest_ShouldReturnNoErrors()
    {
        // Arrange
        // No need to arrange anything since the register user request is already valid

        // Act
        var validationResult = _registerUserRequestValidator.TestValidate(_validRegisterUserRequest);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenEmailIsInvalid_ShouldReturnErrors()
    {
        // Arrange
        var registerUserRequestWithNullEmail = _validRegisterUserRequest;
        registerUserRequestWithNullEmail.Email = null;

        var registerUserRequestWithEmptyEmail = _validRegisterUserRequest;
        registerUserRequestWithEmptyEmail.Email = string.Empty;

        var registerUserRequestWithInvalidEmail = _validRegisterUserRequest;
        registerUserRequestWithInvalidEmail.Email = "test";

        // Act
        var resultOfNullEmail = _registerUserRequestValidator.TestValidate(registerUserRequestWithNullEmail);
        var resultOfEmptyEmail = _registerUserRequestValidator.TestValidate(registerUserRequestWithEmptyEmail);
        var resultOfInvalidEmail = _registerUserRequestValidator.TestValidate(registerUserRequestWithInvalidEmail);

        // Assert
        resultOfNullEmail.ShouldHaveValidationErrorFor(x => x.Email);
        resultOfEmptyEmail.ShouldHaveValidationErrorFor(x => x.Email);
        resultOfInvalidEmail.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WhenFirstNameIsInvalid_ShouldReturnErrors()
    {
        // Arrange
        var registerUserRequestWithNullFirstName = _validRegisterUserRequest;
        registerUserRequestWithNullFirstName.FirstName = null;

        var registerUserRequestWithEmptyFirstName = _validRegisterUserRequest;
        registerUserRequestWithEmptyFirstName.FirstName = string.Empty;

        var registerUserRequestWithTooLongFirstName = _validRegisterUserRequest;
        registerUserRequestWithTooLongFirstName.FirstName =
            new string('a', UserConstraints.MaxFirstNameLength + 1);

        // Act
        var resultOfNullFirstName = _registerUserRequestValidator.TestValidate(registerUserRequestWithNullFirstName);
        var resultOfEmptyFirstName = _registerUserRequestValidator.TestValidate(registerUserRequestWithEmptyFirstName);
        var resultOfTooLongFirstName =
            _registerUserRequestValidator.TestValidate(registerUserRequestWithTooLongFirstName);

        // Assert
        resultOfNullFirstName.ShouldHaveValidationErrorFor(x => x.FirstName);
        resultOfEmptyFirstName.ShouldHaveValidationErrorFor(x => x.FirstName);
        resultOfTooLongFirstName.ShouldHaveValidationErrorFor(x => x.FirstName);
    }
}
```

## Interceptor Testing

### SoftDeleteInterceptor Test

```csharp
namespace ExpertBridge.UnitTests;

public class SoftDeleteInterceptorTests
{
    [Fact]
    public async Task SavingChanges_WhenEntityIsDeleted_ShouldSoftDelete()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .AddInterceptors(new SoftDeleteInterceptor())
            .Options;

        using var context = new TestDbContext(options);
        var entity = new TestEntity { Id = 1 };

        context.TestEntities.Add(entity);
        await context.SaveChangesAsync();

        // Act
        context.TestEntities.Remove(entity);
        await context.SaveChangesAsync();

        // Assert
        var softDeletedEntity = await context.TestEntities.FirstOrDefaultAsync();
        Assert.NotNull(softDeletedEntity);
        Assert.True(softDeletedEntity.IsDeleted);
        Assert.NotNull(softDeletedEntity.DeletedAt);
        Assert.True(softDeletedEntity.DeletedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void SavingChanges_WhenEntityIsDeleted_ShouldSoftDelete_Sync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .AddInterceptors(new SoftDeleteInterceptor())
            .Options;

        using var context = new TestDbContext(options);
        var entity = new TestEntity { Id = 1 };

        context.TestEntities.Add(entity);
        context.SaveChanges();

        // Act
        context.TestEntities.Remove(entity);
        context.SaveChanges();

        // Assert
        var softDeletedEntity = context.TestEntities.FirstOrDefault();
        Assert.NotNull(softDeletedEntity);
        Assert.True(softDeletedEntity.IsDeleted);
        Assert.NotNull(softDeletedEntity.DeletedAt);
    }
}

// Test DbContext
public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    public DbSet<TestEntity> TestEntities { get; set; }
}

// Test Entity
public class TestEntity : ISoftDeletable
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
```

## FluentValidation TestHelper

### Common Validation Patterns

```csharp
// Should not have any errors
validationResult.ShouldNotHaveAnyValidationErrors();

// Should have error for specific property
validationResult.ShouldHaveValidationErrorFor(x => x.Email);

// Should not have error for specific property
validationResult.ShouldNotHaveValidationErrorFor(x => x.Username);

// Check specific error message
validationResult.ShouldHaveValidationErrorFor(x => x.Email)
    .WithErrorMessage("Email is required");

// Check error code
validationResult.ShouldHaveValidationErrorFor(x => x.Email)
    .WithErrorCode("NotEmpty");
```

## In-Memory Database Testing

### DbContext Setup Pattern

```csharp
[Fact]
public async Task MyTest_ShouldWork()
{
    // Arrange - Create unique database per test
    var options = new DbContextOptionsBuilder<ExpertBridgeDbContext>()
        .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
        .AddInterceptors(new SoftDeleteInterceptor())
        .Options;

    using var context = new ExpertBridgeDbContext(options);

    var entity = new MyEntity { Id = "test-id", Name = "Test" };
    context.MyEntities.Add(entity);
    await context.SaveChangesAsync();

    // Act
    var result = await context.MyEntities.FirstOrDefaultAsync(e => e.Id == "test-id");

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Test", result.Name);
}
```

## Test Organization

### Folder Structure

```
ExpertBridge.UnitTests/
├── GlobalUsings.cs                              # Global using directives
├── ValidationTests/
│   ├── DTOs/
│   │   ├── RegisterUserRequestValidatorTests.cs
│   │   ├── CreatePostRequestValidatorTests.cs
│   │   ├── CreateCommentRequestValidatorTests.cs
│   │   └── ...
│   └── Entities/
│       ├── UserEntityValidationTests.cs
│       └── ...
├── InterceptorTests/
│   └── SoftDeleteInterceptorTests.cs
├── ServiceTests/
│   ├── CommentServiceTests.cs
│   ├── PostServiceTests.cs
│   └── ...
└── RepositoryTests/
    └── ...
```

## GlobalUsings.cs

### Common Global Usings

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

global using Xunit;
global using FluentValidation;
global using FluentValidation.TestHelper;
global using Microsoft.EntityFrameworkCore;
global using ExpertBridge.Core.Entities;
global using ExpertBridge.Core.Requests;
global using ExpertBridge.Core.Validators;
global using ExpertBridge.Core.EntityConfiguration.EntitiesConstraints;
global using ExpertBridge.Data.DatabaseContexts;
global using ExpertBridge.Data.Interceptors;
```

## xUnit Patterns

### Fact Tests (Single Test)

```csharp
[Fact]
public void MyTest_ShouldWork()
{
    // Arrange
    var input = "test";

    // Act
    var result = MyMethod(input);

    // Assert
    Assert.Equal("expected", result);
}
```

### Theory Tests (Parameterized)

```csharp
[Theory]
[InlineData("test@example.com", true)]
[InlineData("invalid-email", false)]
[InlineData("", false)]
[InlineData(null, false)]
public void ValidateEmail_WithVariousInputs_ShouldReturnExpectedResult(string email, bool expectedValid)
{
    // Arrange
    var request = new RegisterUserRequest { Email = email };

    // Act
    var result = _validator.TestValidate(request);

    // Assert
    if (expectedValid)
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    else
        result.ShouldHaveValidationErrorFor(x => x.Email);
}
```

### MemberData Tests (Complex Data)

```csharp
public static IEnumerable<object[]> GetTestData()
{
    yield return new object[] { "test1", true };
    yield return new object[] { "test2", false };
}

[Theory]
[MemberData(nameof(GetTestData))]
public void MyTest_WithMemberData(string input, bool expected)
{
    // Test implementation
}
```

## Async Testing

```csharp
[Fact]
public async Task MyAsyncTest_ShouldWork()
{
    // Arrange
    var dbContext = CreateInMemoryDbContext();

    // Act
    var result = await dbContext.Users.FirstOrDefaultAsync();

    // Assert
    Assert.NotNull(result);
}
```

## Test Fixtures

### Shared Setup Across Tests

```csharp
public class DatabaseFixture : IDisposable
{
    public ExpertBridgeDbContext Context { get; private set; }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<ExpertBridgeDbContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .Options;

        Context = new ExpertBridgeDbContext(options);
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        // Add seed data
        Context.Users.Add(new User { Id = "test-1", Email = "test@example.com" });
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}

public class MyTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public MyTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_UsingFixture()
    {
        var user = _fixture.Context.Users.First();
        Assert.NotNull(user);
    }
}
```

## Best Practices

1. **Use Arrange-Act-Assert pattern** - Clear test structure
2. **One assertion concept per test** - Focused tests
3. **Use descriptive test names** - `MethodName_Scenario_ExpectedResult`
4. **Use unique database names** - `Guid.NewGuid()` per test
5. **Test both async and sync methods** - If both exist
6. **Use FluentValidation.TestHelper** - Simplifies validator testing
7. **Test edge cases** - Null, empty, max length, min length
8. **Mock external dependencies** - Use Moq for services
9. **Use Theory for multiple scenarios** - Reduce test duplication
10. **Clean up resources** - Dispose DbContext, use `using` statements

## Validation Test Patterns

### Required Field Validation

```csharp
[Fact]
public void Validate_WhenFieldIsNull_ShouldHaveError()
{
    var request = _validRequest with { FieldName = null };
    var result = _validator.TestValidate(request);
    result.ShouldHaveValidationErrorFor(x => x.FieldName);
}
```

### Length Validation

```csharp
[Fact]
public void Validate_WhenFieldTooLong_ShouldHaveError()
{
    var request = _validRequest with
    {
        FieldName = new string('a', MaxLength + 1)
    };
    var result = _validator.TestValidate(request);
    result.ShouldHaveValidationErrorFor(x => x.FieldName);
}
```

### Email Validation

```csharp
[Theory]
[InlineData("test@example.com")]
[InlineData("user.name@example.co.uk")]
public void Validate_WhenEmailIsValid_ShouldNotHaveError(string email)
{
    var request = _validRequest with { Email = email };
    var result = _validator.TestValidate(request);
    result.ShouldNotHaveValidationErrorFor(x => x.Email);
}
```

## Anti-Patterns to Avoid

-   ❌ Don't share DbContext between tests (use unique databases)
-   ❌ Don't use static data in tests (creates test coupling)
-   ❌ Don't test multiple concerns in one test
-   ❌ Don't use Thread.Sleep for async operations
-   ❌ Don't forget to dispose resources
-   ❌ Don't rely on test execution order
-   ❌ Don't use production database for tests
-   ❌ Don't skip testing error paths
-   ❌ Don't have tests with no assertions
-   ❌ Don't use hard-coded timestamps (use DateTime.UtcNow comparisons)

## Running Tests

### CLI Commands

```bash
# Run all tests
dotnet test

# Run specific test
dotnet test --filter "FullyQualifiedName~RegisterUserRequestValidatorTests"

# Run tests with verbosity
dotnet test --verbosity detailed

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Test Naming Conventions

```csharp
// Pattern: MethodName_Scenario_ExpectedBehavior

Validate_ValidRegisterUserRequest_ShouldReturnNoErrors
Validate_WhenEmailIsInvalid_ShouldReturnErrors
SavingChanges_WhenEntityIsDeleted_ShouldSoftDelete
CreateComment_WithValidData_ShouldSucceed
GetUserById_WhenUserNotFound_ShouldThrowException
```

## Mock Setup (if using Moq)

```csharp
[Fact]
public async Task MyService_CallsDependency_Correctly()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
        .ReturnsAsync(new User { Id = "test-1" });

    var service = new UserService(mockRepo.Object);

    // Act
    var result = await service.GetUserAsync("test-1");

    // Assert
    Assert.NotNull(result);
    mockRepo.Verify(r => r.GetByIdAsync("test-1"), Times.Once);
}
```

## FluentAssertions Examples

```csharp
// Use FluentAssertions for more readable assertions
result.Should().NotBeNull();
result.Id.Should().Be("expected-id");
result.Users.Should().HaveCount(5);
result.Email.Should().Contain("@example.com");
action.Should().Throw<ArgumentNullException>();
```
