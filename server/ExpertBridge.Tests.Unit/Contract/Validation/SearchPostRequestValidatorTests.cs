// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for SearchPostRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: Query validation (required, min/max length), Limit validation (optional, range),
///     and edge cases for post search functionality.
/// </remarks>
public sealed class SearchPostRequestValidatorTests
{
  private readonly SearchPostRequestValidator _validator;

  public SearchPostRequestValidatorTests()
  {
    _validator = new SearchPostRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_With_Valid_Query_Only()
  {
    // Arrange
    var request = new SearchPostRequest
    {
      Query = "test query"
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
    var request = new SearchPostRequest
    {
      Query = "technology",
      Limit = 50
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
    var request = new SearchPostRequest
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
    var request = new SearchPostRequest
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
    var request = new SearchPostRequest
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
    var request = new SearchPostRequest
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
    var request = new SearchPostRequest
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
    var request = new SearchPostRequest
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
    var request = new SearchPostRequest
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
    var request = new SearchPostRequest
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
    var request = new SearchPostRequest
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
    var request = new SearchPostRequest
    {
      Query = "search",
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
    var request = new SearchPostRequest
    {
      Query = "search",
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
    var request = new SearchPostRequest
    {
      Query = "search",
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
    var request = new SearchPostRequest
    {
      Query = "search",
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
  public async Task Should_Pass_With_Query_Containing_Special_Characters()
  {
    // Arrange
    var request = new SearchPostRequest
    {
      Query = "C# .NET & Azure!"
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
    var request = new SearchPostRequest
    {
      Query = "技術 technology"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Query_Containing_Emojis()
  {
    // Arrange
    var request = new SearchPostRequest
    {
      Query = "tech 🚀 innovation"
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
    var request = new SearchPostRequest
    {
      Query = "dotnet9 2025"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Multiple_Word_Query()
  {
    // Arrange
    var request = new SearchPostRequest
    {
      Query = "software engineering best practices"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion
}
