// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobPostings;

namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for CreateJobPostingRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: Area validation, Title validation with XSS/HTML prevention,
///     Content validation with XSS/dangerous patterns, Budget range validation (0 to 1,000,000),
///     and edge cases.
/// </remarks>
public sealed class CreateJobPostingRequestValidatorTests
{
  private readonly CreateJobPostingRequestValidator _validator;

  public CreateJobPostingRequestValidatorTests()
  {
    _validator = new CreateJobPostingRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_With_Valid_Request_All_Fields()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "Software Development",
      Title = "Senior Full Stack Developer",
      Content = "We are looking for an experienced full stack developer...",
      Budget = 50000m
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
    var request = new CreateJobPostingRequest
    {
      Area = "Consulting",
      Title = "Business Consultant",
      Content = "Looking for business strategy advice.",
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
    var request = new CreateJobPostingRequest
    {
      Area = "Enterprise",
      Title = "Chief Technology Officer",
      Content = "Seeking experienced CTO for large enterprise project.",
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
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = new string('A', JobPostingEntityConstraints.MaxTitleLength),
      Content = "Job description",
      Budget = 1000m
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
    var request = new CreateJobPostingRequest
    {
      Area = "Technology",
      Title = "Developer",
      Content = new string('B', JobPostingEntityConstraints.MaxContentLength),
      Budget = 5000m
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
    var request = new CreateJobPostingRequest
    {
      Area = new string('C', GlobalEntitiesConstraints.MaxIdLength),
      Title = "Job Title",
      Content = "Job content",
      Budget = 2000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region Area Validation Tests

  [Fact]
  public async Task Should_Fail_When_Area_Is_Null()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = null!,
      Title = "Developer",
      Content = "Description",
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Area)
        .WithErrorMessage("Area cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_Area_Is_Empty()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = string.Empty,
      Title = "Developer",
      Content = "Description",
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Area)
        .WithErrorMessage("Area cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Area_Exceeds_Max_Length()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = new string('X', GlobalEntitiesConstraints.MaxIdLength + 1),
      Title = "Developer",
      Content = "Description",
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Area)
        .WithErrorMessage($"Area cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
  }

  #endregion

  #region Title Validation Tests

  [Fact]
  public async Task Should_Fail_When_Title_Is_Null()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = null!,
      Content = "Description",
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_Title_Is_Empty()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = string.Empty,
      Content = "Description",
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Title_Exceeds_Max_Length()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = new string('A', JobPostingEntityConstraints.MaxTitleLength + 1),
      Content = "Description",
      Budget = 1000m
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
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = titleWithScript,
      Content = "Description",
      Budget = 1000m
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
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = titleWithHtml,
      Content = "Description",
      Budget = 1000m
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
  public async Task Should_Fail_When_Content_Is_Null()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = null!,
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Is_Empty()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = string.Empty,
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Exceeds_Max_Length()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = new string('X', JobPostingEntityConstraints.MaxContentLength + 1),
      Budget = 1000m
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
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = contentWithScript,
      Budget = 1000m
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
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = contentWithJavascript,
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");
  }

  [Theory]
  [InlineData("Data URI: data:text/html,<script>alert('xss')</script>")]
  [InlineData("DATA:TEXT/HTML,malicious")]
  public async Task Should_Fail_When_Content_Contains_Data_Protocol(string contentWithData)
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = contentWithData,
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");
  }

  [Theory]
  [InlineData("<div onclick='malicious()'>Click</div>")]
  [InlineData("<img onload='evil()'>")]
  [InlineData("<body onmouseover='attack()'>")]
  public async Task Should_Fail_When_Content_Contains_Event_Handlers(string contentWithEvent)
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = contentWithEvent,
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");
  }

  #endregion

  #region Budget Validation Tests

  [Fact]
  public async Task Should_Fail_When_Budget_Is_Negative()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = "Description",
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
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = "Description",
      Budget = 1000001m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Budget)
        .WithErrorMessage("Budget must be less than or equal to 1,000,000");
  }

  [Fact]
  public async Task Should_Pass_With_Budget_At_Minimum_Boundary()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = "Description",
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
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = "Description",
      Budget = 1000000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Budget);
  }

  #endregion

  #region Edge Cases

  [Fact]
  public async Task Should_Fail_When_Title_Is_Whitespace()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "   ",
      Content = "Description",
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Is_Whitespace()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = "   ",
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Area_Is_Whitespace()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "   ",
      Title = "Developer",
      Content = "Description",
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Area)
        .WithErrorMessage("Area cannot be empty");
  }

  [Fact]
  public async Task Should_Pass_With_Unicode_Characters_In_Content()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = "Job description with Unicode: 你好世界 🚀 Ω",
      Budget = 1000m
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
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Senior Developer 高级开发者 🔧",
      Content = "Description",
      Budget = 1000m
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
    var request = new CreateJobPostingRequest
    {
      Area = string.Empty,
      Title = new string('X', JobPostingEntityConstraints.MaxTitleLength + 1),
      Content = "<script>alert('xss')</script>",
      Budget = -500m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Area);
    result.ShouldHaveValidationErrorFor(x => x.Title);
    result.ShouldHaveValidationErrorFor(x => x.Content);
    result.ShouldHaveValidationErrorFor(x => x.Budget);
  }

  [Fact]
  public async Task Should_Detect_Script_Tags_Case_Insensitively()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = "Content with <ScRiPt>alert('xss')</sCrIpT>",
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot contain script tags");
  }

  [Fact]
  public async Task Should_Pass_With_Decimal_Budget_Values()
  {
    // Arrange
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = "Description",
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
    var request = new CreateJobPostingRequest
    {
      Area = "IT",
      Title = "Developer",
      Content = "Looking for developer with &gt; 5 years experience &amp; strong skills.",
      Budget = 1000m
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion
}
