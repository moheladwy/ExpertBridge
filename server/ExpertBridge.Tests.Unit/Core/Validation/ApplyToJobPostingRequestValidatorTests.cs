// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Core.Validation;

/// <summary>
///     Unit tests for ApplyToJobPostingRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: JobPostingId validation, OfferedCost validation with budget limits,
///     and optional CoverLetter validation with XSS prevention.
/// </remarks>
public sealed class ApplyToJobPostingRequestValidatorTests
{
  private readonly ApplyToJobPostingRequestValidator _validator;

  public ApplyToJobPostingRequestValidatorTests()
  {
    _validator = new ApplyToJobPostingRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_With_Valid_Request_Without_CoverLetter()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Valid_Request_With_CoverLetter()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "I am very interested in this position. I have 5 years of experience in web development."
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Zero_OfferedCost()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 0m,
      CoverLetter = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Maximum_OfferedCost()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 1000000m, // Exactly $1,000,000
      CoverLetter = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_CoverLetter_At_Max_Length()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = new string('a', GlobalEntitiesConstraints.MaxCoverLetterLength) // Exactly 1000 characters
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Long_Valid_JobPostingId()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = new string('a', GlobalEntitiesConstraints.MaxIdLength), // Exactly 450 characters
      OfferedCost = 500m,
      CoverLetter = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Decimal_OfferedCost()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 1234.56m,
      CoverLetter = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_CoverLetter_With_Safe_HTML()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "I have experience with <strong>Angular</strong> and <em>React</em>."
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region JobPostingId Validation Tests

  [Fact]
  public async Task Should_Fail_When_JobPostingId_Is_Null()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = null!,
      OfferedCost = 500m,
      CoverLetter = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.JobPostingId)
        .WithErrorMessage("JobPostingId cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_JobPostingId_Is_Empty()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = string.Empty,
      OfferedCost = 500m,
      CoverLetter = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.JobPostingId)
        .WithErrorMessage("JobPostingId cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_JobPostingId_Exceeds_MaxLength()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1), // 451 characters
      OfferedCost = 500m,
      CoverLetter = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.JobPostingId)
        .WithErrorMessage($"JobPostingId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
  }

  #endregion

  #region OfferedCost Validation Tests

  [Fact]
  public async Task Should_Fail_When_OfferedCost_Is_Negative()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = -1m,
      CoverLetter = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.OfferedCost)
        .WithErrorMessage("OfferedCost must be greater than or equal to 0");
  }

  [Fact]
  public async Task Should_Fail_When_OfferedCost_Exceeds_Maximum()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 1000000.01m, // Just over $1,000,000
      CoverLetter = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.OfferedCost)
        .WithErrorMessage("OfferedCost cannot exceed $1,000,000");
  }

  [Fact]
  public async Task Should_Fail_When_OfferedCost_Is_Very_Large()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 999999999m,
      CoverLetter = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.OfferedCost)
        .WithErrorMessage("OfferedCost cannot exceed $1,000,000");
  }

  #endregion

  #region CoverLetter Validation Tests - Length

  [Fact]
  public async Task Should_Pass_When_CoverLetter_Is_Null()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.CoverLetter);
  }

  [Fact]
  public async Task Should_Fail_When_CoverLetter_Exceeds_MaxLength()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = new string('a', GlobalEntitiesConstraints.MaxCoverLetterLength + 1) // 1001 characters
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CoverLetter)
        .WithErrorMessage($"CoverLetter cannot be longer than {GlobalEntitiesConstraints.MaxCoverLetterLength} characters");
  }

  #endregion

  #region CoverLetter Validation Tests - XSS Prevention

  [Fact]
  public async Task Should_Fail_When_CoverLetter_Contains_Script_Tag()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "Hello <script>alert('xss')</script> world"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CoverLetter)
        .WithErrorMessage("CoverLetter cannot contain script tags");
  }

  [Fact]
  public async Task Should_Fail_When_CoverLetter_Contains_Script_Tag_With_Uppercase()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "Hello <SCRIPT>alert('xss')</SCRIPT> world"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CoverLetter)
        .WithErrorMessage("CoverLetter cannot contain script tags");
  }

  [Fact]
  public async Task Should_Fail_When_CoverLetter_Contains_Script_Tag_With_Attributes()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "Hello <script type='text/javascript'>alert('xss')</script> world"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CoverLetter)
        .WithErrorMessage("CoverLetter cannot contain script tags");
  }

  [Fact]
  public async Task Should_Fail_When_CoverLetter_Contains_Javascript_Protocol()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "Click here: javascript:alert('xss')"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CoverLetter)
        .WithErrorMessage("CoverLetter contains potentially dangerous content");
  }

  [Fact]
  public async Task Should_Fail_When_CoverLetter_Contains_Data_Protocol()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "Link: data:text/html,<script>alert('xss')</script>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CoverLetter)
        .WithErrorMessage("CoverLetter contains potentially dangerous content");
  }

  [Fact]
  public async Task Should_Fail_When_CoverLetter_Contains_Onclick_Handler()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "<div onclick='alert(1)'>Click me</div>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CoverLetter)
        .WithErrorMessage("CoverLetter contains potentially dangerous content");
  }

  [Fact]
  public async Task Should_Fail_When_CoverLetter_Contains_Onload_Handler()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "<img onload='alert(1)' src='x'>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CoverLetter)
        .WithErrorMessage("CoverLetter contains potentially dangerous content");
  }

  [Fact]
  public async Task Should_Fail_When_CoverLetter_Contains_Onerror_Handler()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "<img onerror='alert(1)' src='invalid'>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CoverLetter)
        .WithErrorMessage("CoverLetter contains potentially dangerous content");
  }

  [Fact]
  public async Task Should_Fail_When_CoverLetter_Contains_Onmouseover_Handler()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "<div onmouseover='alert(1)'>Hover</div>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CoverLetter)
        .WithErrorMessage("CoverLetter contains potentially dangerous content");
  }

  #endregion

  #region Conditional Validation Tests

  [Fact]
  public async Task Should_Not_Validate_CoverLetter_When_Null()
  {
    // Arrange - CoverLetter is null, so XSS validation should not run
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.CoverLetter);
  }

  #endregion

  #region Edge Cases

  [Fact]
  public async Task Should_Pass_With_Empty_String_CoverLetter()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Whitespace_CoverLetter()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "   "
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_CoverLetter_Containing_Newlines()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "Dear Hiring Manager,\n\nI am interested in this position.\n\nBest regards,\nJohn Doe"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_CoverLetter_Containing_Special_Characters()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "I have 5+ years of experience! My skills include: C#, .NET, & SQL. Contact me at user@example.com."
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_CoverLetter_Containing_Unicode_Characters()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "Hello! 你好! Привет! مرحبا"
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
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = string.Empty, // Invalid
      OfferedCost = -100m, // Invalid
      CoverLetter = "<script>alert('xss')</script>" // Invalid
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.JobPostingId);
    result.ShouldHaveValidationErrorFor(x => x.OfferedCost);
    result.ShouldHaveValidationErrorFor(x => x.CoverLetter);
  }

  [Fact]
  public async Task Should_Pass_With_Very_Small_OfferedCost()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 0.01m,
      CoverLetter = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_GUID_JobPostingId()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = Guid.NewGuid().ToString(),
      OfferedCost = 500m,
      CoverLetter = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Fail_When_CoverLetter_Contains_Multiple_XSS_Patterns()
  {
    // Arrange
    var request = new ApplyToJobPostingRequest
    {
      JobPostingId = "job123",
      OfferedCost = 500m,
      CoverLetter = "<script>alert('xss')</script><img onclick='alert(1)' src='x'>javascript:void(0)"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CoverLetter);
  }

  #endregion
}
