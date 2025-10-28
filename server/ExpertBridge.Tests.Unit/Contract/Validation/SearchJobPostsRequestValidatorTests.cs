// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for SearchJobPostsRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: Query validation (required, min/max length), Limit validation (optional, range),
///     budget filters (MinBudget, MaxBudget), budget range logic, and edge cases.
/// </remarks>
public sealed class SearchJobPostsRequestValidatorTests
{
  private readonly SearchJobPostsRequestValidator _validator;

  public SearchJobPostsRequestValidatorTests()
  {
    _validator = new SearchJobPostsRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_With_Valid_Query_Only()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "software developer"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_All_Fields_Valid()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "web developer",
      Limit = 50,
      MinBudget = 1000,
      MaxBudget = 5000
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Minimum_Valid_Query_Length()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "ab" // Exactly 2 characters
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Maximum_Valid_Query_Length()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = new string('a', 200) // Exactly 200 characters
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_MinBudget_Only()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "designer",
      MinBudget = 500
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_MaxBudget_Only()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "designer",
      MaxBudget = 10000
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Equal_MinBudget_And_MaxBudget()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "consultant",
      MinBudget = 5000,
      MaxBudget = 5000
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Zero_MinBudget()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "freelance",
      MinBudget = 0,
      MaxBudget = 1000
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Maximum_Budget_Values()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "enterprise",
      MinBudget = 999999,
      MaxBudget = 1000000
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region Query Validation Tests

  [Fact]
  public async Task Should_Fail_When_Query_Is_Null()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = null!
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Query)
        .WithErrorMessage("Query cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_Query_Is_Empty()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Query)
        .WithErrorMessage("Query cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Query_Is_Whitespace()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "   "
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Query)
        .WithErrorMessage("Query cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Query_Is_Too_Short()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "a" // Only 1 character
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Query)
        .WithErrorMessage("Query must be at least 2 characters long");
  }

  [Fact]
  public async Task Should_Fail_When_Query_Exceeds_Maximum_Length()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = new string('a', 201) // 201 characters
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Query)
        .WithErrorMessage("Query cannot exceed 200 characters");
  }

  #endregion

  #region Limit Validation Tests

  [Fact]
  public async Task Should_Pass_When_Limit_Is_Null()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      Limit = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Limit_At_Minimum_Boundary()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      Limit = 1
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Limit_At_Maximum_Boundary()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      Limit = 100
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Fail_When_Limit_Is_Zero()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      Limit = 0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Limit)
        .WithErrorMessage("Limit must be greater than 0");
  }

  [Fact]
  public async Task Should_Fail_When_Limit_Is_Negative()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      Limit = -10
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Limit)
        .WithErrorMessage("Limit must be greater than 0");
  }

  [Fact]
  public async Task Should_Fail_When_Limit_Exceeds_Maximum()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      Limit = 101
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Limit)
        .WithErrorMessage("Limit cannot exceed 100");
  }

  #endregion

  #region MinBudget Validation Tests

  [Fact]
  public async Task Should_Pass_When_MinBudget_Is_Null()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      MinBudget = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_MinBudget_At_Zero()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      MinBudget = 0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_MinBudget_At_Maximum()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      MinBudget = 1000000
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Fail_When_MinBudget_Is_Negative()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      MinBudget = -100
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.MinBudget)
        .WithErrorMessage("MinBudget must be greater than or equal to 0");
  }

  [Fact]
  public async Task Should_Fail_When_MinBudget_Exceeds_Maximum()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      MinBudget = 1000001
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.MinBudget)
        .WithErrorMessage("MinBudget cannot exceed 1,000,000");
  }

  #endregion

  #region MaxBudget Validation Tests

  [Fact]
  public async Task Should_Pass_When_MaxBudget_Is_Null()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      MaxBudget = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_MaxBudget_At_Zero()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      MaxBudget = 0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_MaxBudget_At_Maximum()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      MaxBudget = 1000000
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Fail_When_MaxBudget_Is_Negative()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      MaxBudget = -500
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.MaxBudget)
        .WithErrorMessage("MaxBudget must be greater than or equal to 0");
  }

  [Fact]
  public async Task Should_Fail_When_MaxBudget_Exceeds_Maximum()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      MaxBudget = 1500000
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.MaxBudget)
        .WithErrorMessage("MaxBudget cannot exceed 1,000,000");
  }

  #endregion

  #region Budget Range Validation Tests

  [Fact]
  public async Task Should_Fail_When_MinBudget_Greater_Than_MaxBudget()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      MinBudget = 10000,
      MaxBudget = 5000
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x)
        .WithErrorMessage("MinBudget cannot be greater than MaxBudget");
  }

  [Fact]
  public async Task Should_Pass_When_Only_MinBudget_Is_Provided()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      MinBudget = 5000,
      MaxBudget = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Only_MaxBudget_Is_Provided()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      MinBudget = null,
      MaxBudget = 5000
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Both_Budgets_Are_Null()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "developer",
      MinBudget = null,
      MaxBudget = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region Edge Cases

  [Fact]
  public async Task Should_Pass_With_Query_Containing_Job_Related_Keywords()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "full-time remote senior developer"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Query_Containing_Special_Characters()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "C# .NET developer"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Query_Containing_Unicode()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "développeur français"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Decimal_Budget_Values()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "freelancer",
      MinBudget = 500.50m,
      MaxBudget = 1500.99m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_All_Optional_Fields_Null()
  {
    // Arrange
    var request = new SearchJobPostsRequest
    {
      Query = "engineer",
      Limit = null,
      MinBudget = null,
      MaxBudget = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion
}
