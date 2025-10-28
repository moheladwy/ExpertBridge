// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for SearchUserRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: Query validation (required, min/max length), Limit validation (optional, range),
///     and edge cases for user search functionality.
/// </remarks>
public sealed class SearchUserRequestValidatorTests
{
  private readonly SearchUserRequestValidator _validator;

  public SearchUserRequestValidatorTests()
  {
    _validator = new SearchUserRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_With_Valid_Query_Only()
  {
    // Arrange
    var request = new SearchUserRequest
    {
      Query = "john doe"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Valid_Query_And_Limit()
  {
    // Arrange
    var request = new SearchUserRequest
    {
      Query = "developer",
      Limit = 25
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
    var request = new SearchUserRequest
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
    var request = new SearchUserRequest
    {
      Query = new string('a', 200) // Exactly 200 characters
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
    var request = new SearchUserRequest
    {
      Query = "search",
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
    var request = new SearchUserRequest
    {
      Query = "search",
      Limit = 100
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
    var request = new SearchUserRequest
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
    var request = new SearchUserRequest
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
    var request = new SearchUserRequest
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
    var request = new SearchUserRequest
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
    var request = new SearchUserRequest
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
    var request = new SearchUserRequest
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
  public async Task Should_Fail_When_Limit_Is_Zero()
  {
    // Arrange
    var request = new SearchUserRequest
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
    var request = new SearchUserRequest
    {
      Query = "developer",
      Limit = -5
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
    var request = new SearchUserRequest
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

  #region Edge Cases

  [Fact]
  public async Task Should_Pass_With_Username_Query()
  {
    // Arrange
    var request = new SearchUserRequest
    {
      Query = "@username123"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Email_Query()
  {
    // Arrange
    var request = new SearchUserRequest
    {
      Query = "user@example.com"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Full_Name_Query()
  {
    // Arrange
    var request = new SearchUserRequest
    {
      Query = "John Michael Doe"
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
    var request = new SearchUserRequest
    {
      Query = "José García"
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
    var request = new SearchUserRequest
    {
      Query = "O'Brien-Smith"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Query_Containing_Numbers()
  {
    // Arrange
    var request = new SearchUserRequest
    {
      Query = "user2025"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion
}
