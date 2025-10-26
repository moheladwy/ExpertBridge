// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentValidation.TestHelper;

namespace ExpertBridge.Tests.Unit.Core.Validation;

/// <summary>
///     Unit tests for CreateJobOfferRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: Title validation with XSS/HTML prevention, Description validation with XSS/dangerous patterns,
///     Area GUID validation, Budget range validation (0 to 1,000,000), WorkerId GUID validation, and edge cases.
/// </remarks>
public sealed class CreateJobOfferRequestValidatorTests
{
  private readonly CreateJobOfferRequestValidator _validator;

  public CreateJobOfferRequestValidatorTests()
  {
    _validator = new CreateJobOfferRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_With_Valid_Request()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Senior Backend Developer",
      Description = "Looking for experienced developer for long-term project",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 75000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
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
    var request = new CreateJobOfferRequest
    {
      Title = "Quick Task",
      Description = "Simple quick task",
      Area = "770e8400-e29b-41d4-a716-446655440000",
      Budget = 0m,
      WorkerId = "880e8400-e29b-41d4-a716-446655440001"
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
    var request = new CreateJobOfferRequest
    {
      Title = "Enterprise Project",
      Description = "Large scale enterprise transformation project",
      Area = "990e8400-e29b-41d4-a716-446655440000",
      Budget = 1000000m,
      WorkerId = "aa0e8400-e29b-41d4-a716-446655440001"
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
    var request = new CreateJobOfferRequest
    {
      Title = new string('A', GlobalEntitiesConstraints.MaxTitleLength),
      Description = "Description",
      Area = "bb0e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "cc0e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Description_At_Max_Length()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = new string('B', GlobalEntitiesConstraints.MaxDescriptionLength),
      Area = "dd0e8400-e29b-41d4-a716-446655440000",
      Budget = 10000m,
      WorkerId = "ee0e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region Title Validation Tests

  [Fact]
  public async Task Should_Fail_When_Title_Is_Null()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = null!,
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
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
    var request = new CreateJobOfferRequest
    {
      Title = string.Empty,
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Title_Is_Whitespace()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "   ",
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
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
    var request = new CreateJobOfferRequest
    {
      Title = new string('X', GlobalEntitiesConstraints.MaxTitleLength + 1),
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage($"Title cannot be longer than {GlobalEntitiesConstraints.MaxTitleLength} characters");
  }

  [Theory]
  [InlineData("<script>alert('xss')</script>")]
  [InlineData("Job Title<script>malicious()</script>")]
  [InlineData("<SCRIPT>XSS</SCRIPT>")]
  public async Task Should_Fail_When_Title_Contains_Script_Tags(string titleWithScript)
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = titleWithScript,
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
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
  public async Task Should_Fail_When_Title_Contains_HTML_Tags(string titleWithHtml)
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = titleWithHtml,
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title cannot contain HTML tags");
  }

  #endregion

  #region Description Validation Tests

  [Fact]
  public async Task Should_Fail_When_Description_Is_Null()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = null!,
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
        .WithErrorMessage("Description cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_Description_Is_Empty()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = string.Empty,
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
        .WithErrorMessage("Description cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Description_Is_Whitespace()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "   ",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
        .WithErrorMessage("Description cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Description_Exceeds_Max_Length()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = new string('X', GlobalEntitiesConstraints.MaxDescriptionLength + 1),
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
        .WithErrorMessage($"Description cannot be longer than {GlobalEntitiesConstraints.MaxDescriptionLength} characters");
  }

  [Theory]
  [InlineData("<script>alert('xss')</script>")]
  [InlineData("Description<script>bad()</script>")]
  [InlineData("<SCRIPT SRC='evil.js'></SCRIPT>")]
  public async Task Should_Fail_When_Description_Contains_Script_Tags(string descWithScript)
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = descWithScript,
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
        .WithErrorMessage("Description cannot contain script tags");
  }

  [Theory]
  [InlineData("Click here: javascript:alert('xss')")]
  [InlineData("JAVASCRIPT:malicious()")]
  public async Task Should_Fail_When_Description_Contains_Javascript_Protocol(string descWithJs)
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = descWithJs,
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
        .WithErrorMessage("Description contains potentially dangerous patterns");
  }

  [Theory]
  [InlineData("Data: data:text/html,<script>alert('xss')</script>")]
  [InlineData("DATA:TEXT/HTML,malicious")]
  public async Task Should_Fail_When_Description_Contains_Data_Protocol(string descWithData)
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = descWithData,
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
        .WithErrorMessage("Description contains potentially dangerous patterns");
  }

  [Theory]
  [InlineData("<div onclick='malicious()'>Click</div>")]
  [InlineData("<img onload='evil()'>")]
  public async Task Should_Fail_When_Description_Contains_Event_Handlers(string descWithEvent)
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = descWithEvent,
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
        .WithErrorMessage("Description contains potentially dangerous patterns");
  }

  #endregion

  #region Area Validation Tests

  [Fact]
  public async Task Should_Fail_When_Area_Is_Null()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = null!,
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
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
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = string.Empty,
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Area)
        .WithErrorMessage("Area cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Area_Is_Whitespace()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = "   ",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
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
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = new string('X', GlobalEntitiesConstraints.MaxIdLength + 1),
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Area)
        .WithErrorMessage($"Area cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
  }

  [Theory]
  [InlineData("not-a-guid")]
  [InlineData("123456")]
  [InlineData("550e8400-e29b-41d4-a716")]
  [InlineData("550e8400-e29b-41d4-a716-446655440000-extra")]
  [InlineData("550e8400-e29b-41d4-a716-44665544000g")]
  public async Task Should_Fail_When_Area_Is_Not_Valid_GUID(string invalidGuid)
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = invalidGuid,
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Area)
        .WithErrorMessage("Area must be a valid GUID format");
  }

  [Fact]
  public async Task Should_Pass_When_Area_Is_Valid_GUID_Uppercase()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = "550E8400-E29B-41D4-A716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Area);
  }

  #endregion

  #region Budget Validation Tests

  [Fact]
  public async Task Should_Fail_When_Budget_Is_Negative()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = -100m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Budget)
        .WithErrorMessage("Budget must be greater than or equal to 0");
  }

  [Fact]
  public async Task Should_Fail_When_Budget_Exceeds_Maximum()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 1000001m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Budget)
        .WithErrorMessage("Budget cannot exceed 1,000,000");
  }

  [Fact]
  public async Task Should_Pass_With_Budget_At_Zero()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 0m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Budget);
  }

  [Fact]
  public async Task Should_Pass_With_Budget_At_Maximum()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 1000000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Budget);
  }

  #endregion

  #region WorkerId Validation Tests

  [Fact]
  public async Task Should_Fail_When_WorkerId_Is_Null()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = null!
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.WorkerId)
        .WithErrorMessage("WorkerId cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_WorkerId_Is_Empty()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.WorkerId)
        .WithErrorMessage("WorkerId cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_WorkerId_Is_Whitespace()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "   "
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.WorkerId)
        .WithErrorMessage("WorkerId cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_WorkerId_Exceeds_Max_Length()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = new string('X', GlobalEntitiesConstraints.MaxIdLength + 1)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.WorkerId)
        .WithErrorMessage($"WorkerId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
  }

  [Theory]
  [InlineData("not-a-guid")]
  [InlineData("12345")]
  [InlineData("660e8400-e29b-41d4-a716")]
  [InlineData("660e8400-e29b-41d4-a716-446655440001-extra")]
  [InlineData("660e8400-e29b-41d4-a716-44665544000z")]
  public async Task Should_Fail_When_WorkerId_Is_Not_Valid_GUID(string invalidGuid)
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = invalidGuid
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.WorkerId)
        .WithErrorMessage("WorkerId must be a valid GUID format");
  }

  [Fact]
  public async Task Should_Pass_When_WorkerId_Is_Valid_GUID_Uppercase()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660E8400-E29B-41D4-A716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.WorkerId);
  }

  #endregion

  #region Edge Cases

  [Fact]
  public async Task Should_Pass_With_Unicode_In_Title()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Senior Developer 高级开发者 🔧",
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Unicode_In_Description()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Job description with Unicode: 你好世界 🚀 مرحبا",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
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
    var request = new CreateJobOfferRequest
    {
      Title = string.Empty,
      Description = "<script>xss</script>",
      Area = "not-a-guid",
      Budget = -500m,
      WorkerId = "invalid"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title);
    result.ShouldHaveValidationErrorFor(x => x.Description);
    result.ShouldHaveValidationErrorFor(x => x.Area);
    result.ShouldHaveValidationErrorFor(x => x.Budget);
    result.ShouldHaveValidationErrorFor(x => x.WorkerId);
  }

  [Fact]
  public async Task Should_Pass_With_Decimal_Budget()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 12345.67m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_HTML_Entities_In_Description()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Looking for developer with &gt; 5 years experience &amp; strong skills",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Detect_Script_Tags_Case_Insensitively()
  {
    // Arrange
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description with <ScRiPt>alert('xss')</sCrIpT>",
      Area = "550e8400-e29b-41d4-a716-446655440000",
      Budget = 5000m,
      WorkerId = "660e8400-e29b-41d4-a716-446655440001"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
        .WithErrorMessage("Description cannot contain script tags");
  }

  [Fact]
  public async Task Should_Pass_With_Same_Area_And_WorkerId()
  {
    // Arrange - This is allowed since they represent different things
    var sameGuid = "550e8400-e29b-41d4-a716-446655440000";
    var request = new CreateJobOfferRequest
    {
      Title = "Developer",
      Description = "Description",
      Area = sameGuid,
      Budget = 5000m,
      WorkerId = sameGuid
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion
}
