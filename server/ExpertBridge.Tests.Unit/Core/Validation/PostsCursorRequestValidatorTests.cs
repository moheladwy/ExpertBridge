// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Requests.PostsCursor;
using FluentValidation.TestHelper;

namespace ExpertBridge.Tests.Unit.Core.Validation;

/// <summary>
///     Unit tests for PostsCursorRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: PageSize (1-100), Page (>0), optional After (0-1), optional LastIdCursor validation.
/// </remarks>
public sealed class PostsCursorRequestValidatorTests
{
  private readonly PostsCursorRequestValidator _validator;

  public PostsCursorRequestValidatorTests()
  {
    _validator = new PostsCursorRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_When_Request_Has_Valid_PageSize_And_Page()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 10,
      Page = 1
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Request_Has_Valid_After_Cursor()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 20,
      Page = 2,
      After = 0.75
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Request_Has_Valid_LastIdCursor()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 50,
      Page = 3,
      LastIdCursor = "post-id-12345"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Request_Has_Both_Cursors()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 25,
      Page = 1,
      After = 0.5,
      LastIdCursor = "post-abc"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_After_Is_Zero()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 10,
      Page = 1,
      After = 0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_After_Is_One()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 10,
      Page = 1,
      After = 1
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_PageSize_Is_One()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 1,
      Page = 1
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_PageSize_Is_100()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 100,
      Page = 1
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_LastIdCursor_Has_Max_Length()
  {
    // Arrange
    var maxLengthCursor = new string('a', GlobalEntitiesConstraints.MaxIdLength);
    var request = new PostsCursorRequest
    {
      PageSize = 10,
      Page = 1,
      LastIdCursor = maxLengthCursor
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region PageSize Validation Tests

  [Fact]
  public async Task Should_Fail_When_PageSize_Is_Zero()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 0,
      Page = 1
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.PageSize)
        .WithErrorMessage("PageSize must be greater than 0");
  }

  [Fact]
  public async Task Should_Fail_When_PageSize_Is_Negative()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = -1,
      Page = 1
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.PageSize)
        .WithErrorMessage("PageSize must be greater than 0");
  }

  [Fact]
  public async Task Should_Fail_When_PageSize_Exceeds_100()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 101,
      Page = 1
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.PageSize)
        .WithErrorMessage("PageSize cannot exceed 100");
  }

  #endregion

  #region Page Validation Tests

  [Fact]
  public async Task Should_Fail_When_Page_Is_Zero()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 10,
      Page = 0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Page)
        .WithErrorMessage("Page must be greater than 0");
  }

  [Fact]
  public async Task Should_Fail_When_Page_Is_Negative()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 10,
      Page = -1
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Page)
        .WithErrorMessage("Page must be greater than 0");
  }

  #endregion

  #region After Cursor Validation Tests

  [Fact]
  public async Task Should_Fail_When_After_Is_Negative()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 10,
      Page = 1,
      After = -0.1
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.After)
        .WithErrorMessage("After must be greater than or equal to 0");
  }

  [Fact]
  public async Task Should_Fail_When_After_Exceeds_One()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 10,
      Page = 1,
      After = 1.1
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.After)
        .WithErrorMessage("After must be less than or equal to 1");
  }

  [Fact]
  public async Task Should_Pass_When_After_Is_Null()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 10,
      Page = 1,
      After = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region LastIdCursor Validation Tests

  [Fact]
  public async Task Should_Fail_When_LastIdCursor_Is_Empty()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 10,
      Page = 1,
      LastIdCursor = ""
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.LastIdCursor)
        .WithErrorMessage("LastIdCursor cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Fail_When_LastIdCursor_Exceeds_Max_Length()
  {
    // Arrange
    var tooLongCursor = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);
    var request = new PostsCursorRequest
    {
      PageSize = 10,
      Page = 1,
      LastIdCursor = tooLongCursor
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.LastIdCursor)
        .WithErrorMessage($"LastIdCursor cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
  }

  [Fact]
  public async Task Should_Pass_When_LastIdCursor_Is_Null()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 10,
      Page = 1,
      LastIdCursor = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region Edge Case Tests

  [Fact]
  public async Task Should_Fail_When_Both_PageSize_And_Page_Are_Invalid()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 0,
      Page = 0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.PageSize);
    result.ShouldHaveValidationErrorFor(x => x.Page);
  }

  [Fact]
  public async Task Should_Fail_When_All_Fields_Are_Invalid()
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 0,
      Page = -1,
      After = 2.0,
      LastIdCursor = ""
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.PageSize);
    result.ShouldHaveValidationErrorFor(x => x.Page);
    result.ShouldHaveValidationErrorFor(x => x.After);
    result.ShouldHaveValidationErrorFor(x => x.LastIdCursor);
  }

  #endregion

  #region Parameterized Tests

  [Theory]
  [InlineData(1)]
  [InlineData(10)]
  [InlineData(25)]
  [InlineData(50)]
  [InlineData(100)]
  public async Task Should_Pass_When_PageSize_Is_Valid(int pageSize)
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = pageSize,
      Page = 1
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  [InlineData(-10)]
  [InlineData(-100)]
  public async Task Should_Fail_When_PageSize_Is_Zero_Or_Negative(int pageSize)
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = pageSize,
      Page = 1
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.PageSize);
  }

  [Theory]
  [InlineData(101)]
  [InlineData(200)]
  [InlineData(1000)]
  public async Task Should_Fail_When_PageSize_Exceeds_Maximum(int pageSize)
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = pageSize,
      Page = 1
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.PageSize);
  }

  [Theory]
  [InlineData(1)]
  [InlineData(5)]
  [InlineData(10)]
  [InlineData(100)]
  [InlineData(1000)]
  public async Task Should_Pass_When_Page_Is_Positive(int page)
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 10,
      Page = page
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Theory]
  [InlineData(0.0)]
  [InlineData(0.1)]
  [InlineData(0.5)]
  [InlineData(0.9)]
  [InlineData(1.0)]
  public async Task Should_Pass_When_After_Is_Between_Zero_And_One(double after)
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 10,
      Page = 1,
      After = after
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Theory]
  [InlineData(-0.1)]
  [InlineData(-1.0)]
  [InlineData(1.1)]
  [InlineData(2.0)]
  public async Task Should_Fail_When_After_Is_Outside_Valid_Range(double after)
  {
    // Arrange
    var request = new PostsCursorRequest
    {
      PageSize = 10,
      Page = 1,
      After = after
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.After);
  }

  #endregion
}
