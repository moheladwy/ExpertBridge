// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobPostings;
using FluentValidation.TestHelper;

namespace ExpertBridge.Tests.Unit.Core.Validation;

/// <summary>
///     Unit tests for EditJobPostingRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: Optional Title validation with XSS/HTML prevention,
///     Optional Content validation with XSS/dangerous patterns,
///     Optional Budget range validation (0 to 1,000,000),
///     Optional Area validation, conditional validation, and edge cases.
/// </remarks>
public sealed class EditJobPostingRequestValidatorTests
{
  private readonly EditJobPostingRequestValidator _validator;

  public EditJobPostingRequestValidatorTests()
  {
    _validator = new EditJobPostingRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_With_All_Fields_Null()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Title = null,
      Content = null,
      Budget = null,
      Area = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Valid_Title()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Title = "Senior Full Stack Developer"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Valid_Content()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Content = "We are looking for an experienced developer..."
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Valid_Budget()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Budget = 50000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Valid_Area()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Area = "Software Development"
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
    var request = new EditJobPostingRequest
    {
      Title = "Senior Developer",
      Content = "Detailed job description",
      Budget = 75000m,
      Area = "Technology"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Minimum_Budget()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Budget = 0m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Maximum_Budget()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Budget = 1000000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Title_At_Max_Length()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Title = new string('A', JobPostingEntityConstraints.MaxTitleLength)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Content_At_Max_Length()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Content = new string('B', JobPostingEntityConstraints.MaxContentLength)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Area_At_Max_Length()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Area = new string('C', GlobalEntitiesConstraints.MaxIdLength)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region Title Validation Tests

  [Fact]
  public async Task Should_Fail_When_Title_Is_Empty()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Title = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Fail_When_Title_Is_Whitespace()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Title = "   "
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Fail_When_Title_Exceeds_Max_Length()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Title = new string('X', JobPostingEntityConstraints.MaxTitleLength + 1)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage($"Title cannot be longer than {JobPostingEntityConstraints.MaxTitleLength} characters");
  }

  [Theory]
  [InlineData("<script>alert('xss')</script>")]
  [InlineData("Job Title<script>malicious()</script>")]
  [InlineData("<SCRIPT>XSS</SCRIPT>")]
  [InlineData("<script src='evil.js'></script>")]
  public async Task Should_Fail_When_Title_Contains_Script_Tags(string titleWithScript)
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Title = titleWithScript
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title cannot contain script tags");
  }

  [Theory]
  [InlineData("<div>Developer</div>")]
  [InlineData("<b>Senior</b> Developer")]
  [InlineData("<img src='x'>")]
  [InlineData("<a href='test'>Link</a>")]
  public async Task Should_Fail_When_Title_Contains_HTML_Tags(string titleWithHtml)
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Title = titleWithHtml
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title cannot contain HTML tags");
  }

  #endregion

  #region Content Validation Tests

  [Fact]
  public async Task Should_Fail_When_Content_Is_Empty()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Content = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Is_Whitespace()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Content = "   "
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Exceeds_Max_Length()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Content = new string('X', JobPostingEntityConstraints.MaxContentLength + 1)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage($"Content cannot be longer than {JobPostingEntityConstraints.MaxContentLength} characters");
  }

  [Theory]
  [InlineData("<script>alert('xss')</script>")]
  [InlineData("Good content<script>bad()</script>more content")]
  [InlineData("<SCRIPT SRC='evil.js'></SCRIPT>")]
  [InlineData("Text before<script type='text/javascript'>code()</script>text after")]
  public async Task Should_Fail_When_Content_Contains_Script_Tags(string contentWithScript)
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Content = contentWithScript
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot contain script tags");
  }

  [Theory]
  [InlineData("Click here: javascript:alert('xss')")]
  [InlineData("Link with javascript:void(0)")]
  [InlineData("JAVASCRIPT:malicious()")]
  public async Task Should_Fail_When_Content_Contains_Javascript_Protocol(string contentWithJavascript)
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Content = contentWithJavascript
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns");
  }

  [Theory]
  [InlineData("Data URI: data:text/html,<script>alert('xss')</script>")]
  [InlineData("DATA:TEXT/HTML,malicious")]
  public async Task Should_Fail_When_Content_Contains_Data_Protocol(string contentWithData)
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Content = contentWithData
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns");
  }

  [Theory]
  [InlineData("<div onclick='malicious()'>Click</div>")]
  [InlineData("<img onload='evil()'>")]
  [InlineData("<body onmouseover='attack()'>")]
  public async Task Should_Fail_When_Content_Contains_Event_Handlers(string contentWithEvent)
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Content = contentWithEvent
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns");
  }

  #endregion

  #region Budget Validation Tests

  [Fact]
  public async Task Should_Fail_When_Budget_Is_Negative()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Budget = -100m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Budget)
        .WithErrorMessage($"Budget must be greater than or equal to {JobPostingEntityConstraints.MinBudget}");
  }

  [Fact]
  public async Task Should_Fail_When_Budget_Exceeds_Maximum()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Budget = 1000001m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Budget)
        .WithErrorMessage("Budget cannot exceed 1,000,000");
  }

  [Fact]
  public async Task Should_Pass_With_Budget_At_Minimum_Boundary()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Budget = 0m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Budget);
  }

  [Fact]
  public async Task Should_Pass_With_Budget_At_Maximum_Boundary()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Budget = 1000000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Budget);
  }

  #endregion

  #region Area Validation Tests

  [Fact]
  public async Task Should_Fail_When_Area_Is_Empty()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Area = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Area)
        .WithErrorMessage("Area cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Fail_When_Area_Is_Whitespace()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Area = "   "
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Area)
        .WithErrorMessage("Area cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Fail_When_Area_Exceeds_Max_Length()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Area = new string('X', GlobalEntitiesConstraints.MaxIdLength + 1)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Area)
        .WithErrorMessage($"Area cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
  }

  #endregion

  #region Conditional Validation Tests

  [Fact]
  public async Task Should_Not_Validate_Title_When_Null()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Title = null,
      Content = "Valid content",
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Title);
  }

  [Fact]
  public async Task Should_Not_Validate_Content_When_Null()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Title = "Valid title",
      Content = null,
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Content);
  }

  [Fact]
  public async Task Should_Not_Validate_Budget_When_Null()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Title = "Valid title",
      Content = "Valid content",
      Budget = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Budget);
  }

  [Fact]
  public async Task Should_Not_Validate_Area_When_Null()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Title = "Valid title",
      Area = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Area);
  }

  #endregion

  #region Edge Cases

  [Fact]
  public async Task Should_Pass_With_Unicode_Characters_In_Content()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Content = "Job description with Unicode: 你好世界 🚀 Ω"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Unicode_Characters_In_Title()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Title = "Senior Developer 高级开发者 🔧"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Fail_With_Multiple_Validation_Errors()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Title = new string('X', JobPostingEntityConstraints.MaxTitleLength + 1),
      Content = "<script>alert('xss')</script>",
      Budget = -500m,
      Area = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title);
    result.ShouldHaveValidationErrorFor(x => x.Content);
    result.ShouldHaveValidationErrorFor(x => x.Budget);
    result.ShouldHaveValidationErrorFor(x => x.Area);
  }

  [Fact]
  public async Task Should_Detect_Script_Tags_Case_Insensitively_In_Content()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Content = "Content with <ScRiPt>alert('xss')</sCrIpT>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot contain script tags");
  }

  [Fact]
  public async Task Should_Detect_Script_Tags_Case_Insensitively_In_Title()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Title = "Job<ScRiPt>xss</ScRiPt>Title"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title cannot contain script tags");
  }

  [Fact]
  public async Task Should_Pass_With_Decimal_Budget_Values()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Budget = 12345.67m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Valid_HTML_Entities_In_Content()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Content = "Looking for developer with &gt; 5 years experience &amp; strong skills."
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Only_One_Field_Is_Updated()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Budget = 85000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Zero_Budget()
  {
    // Arrange
    var request = new EditJobPostingRequest
    {
      Budget = 0m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion
}
