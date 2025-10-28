// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for InitiateJobOfferRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: ContractorProfileId, Title, Description, ProposedRate validation,
///     optional JobPostingId, XSS prevention, HTML tags, dangerous patterns, and rate limits.
/// </remarks>
public sealed class InitiateJobOfferRequestValidatorTests
{
  private readonly InitiateJobOfferRequestValidator _validator;

  public InitiateJobOfferRequestValidatorTests()
  {
    _validator = new InitiateJobOfferRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_With_All_Required_Fields_Valid()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-profile-123",
      Title = "Senior Software Developer",
      Description = "We need an experienced developer for our project.",
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Optional_JobPostingId()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-profile-123",
      Title = "Developer Position",
      Description = "Great opportunity for talented developers.",
      ProposedRate = 3000.0,
      JobPostingId = "job-posting-456"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Minimum_ProposedRate()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Entry Level Position",
      Description = "Starting position for new developers.",
      ProposedRate = 0.01
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Maximum_ProposedRate()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Executive Position",
      Description = "High-level executive role.",
      ProposedRate = 1000000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Maximum_Length_Title()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = new string('a', 500), // Max title length
      Description = "Job description",
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Maximum_Length_Description()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = new string('a', 1000), // Max description length
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region ContractorProfileId Validation Tests

  [Fact]
  public async Task Should_Fail_When_ContractorProfileId_Is_Null()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = null!,
      Title = "Developer",
      Description = "Job description",
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ContractorProfileId)
        .WithErrorMessage("ContractorProfileId cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_ContractorProfileId_Is_Empty()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = string.Empty,
      Title = "Developer",
      Description = "Job description",
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ContractorProfileId)
        .WithErrorMessage("ContractorProfileId cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_ContractorProfileId_Exceeds_MaxLength()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = new string('a', 451), // Max is 450
      Title = "Developer",
      Description = "Job description",
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ContractorProfileId)
        .WithErrorMessage("ContractorProfileId cannot be longer than 450 characters");
  }

  #endregion

  #region Title Validation Tests

  [Fact]
  public async Task Should_Fail_When_Title_Is_Null()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = null!,
      Description = "Job description",
      ProposedRate = 5000.0
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
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = string.Empty,
      Description = "Job description",
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Title_Exceeds_MaxLength()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = new string('a', 501), // Max is 500
      Description = "Job description",
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title cannot be longer than 500 characters");
  }

  [Fact]
  public async Task Should_Fail_When_Title_Contains_Script_Tags()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Job Title <script>alert('xss')</script>",
      Description = "Job description",
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title cannot contain script tags");
  }

  [Fact]
  public async Task Should_Fail_When_Title_Contains_HTML_Tags()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Job Title <div>test</div>",
      Description = "Job description",
      ProposedRate = 5000.0
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
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = null!,
      ProposedRate = 5000.0
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
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = string.Empty,
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
        .WithErrorMessage("Description cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Description_Exceeds_MaxLength()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = new string('a', 1001), // Max is 1000
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
        .WithErrorMessage("Description cannot be longer than 1000 characters");
  }

  [Fact]
  public async Task Should_Fail_When_Description_Contains_Script_Tags()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = "Great opportunity <script>alert('xss')</script> for you",
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
        .WithErrorMessage("Description cannot contain script tags");
  }

  [Fact]
  public async Task Should_Fail_When_Description_Contains_Javascript_Protocol()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = "Click here: javascript:alert('xss')",
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
        .WithErrorMessage("Description contains potentially dangerous content");
  }

  [Fact]
  public async Task Should_Fail_When_Description_Contains_Data_Protocol()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = "Link: data:text/html,<script>alert('xss')</script>",
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
        .WithErrorMessage("Description contains potentially dangerous content");
  }

  [Fact]
  public async Task Should_Fail_When_Description_Contains_Event_Handlers()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = "Image: <img onerror='alert(1)' src='x'>",
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Description)
        .WithErrorMessage("Description contains potentially dangerous content");
  }

  #endregion

  #region ProposedRate Validation Tests

  [Fact]
  public async Task Should_Fail_When_ProposedRate_Is_Zero()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = "Job description",
      ProposedRate = 0.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ProposedRate)
        .WithErrorMessage("ProposedRate must be greater than 0");
  }

  [Fact]
  public async Task Should_Fail_When_ProposedRate_Is_Negative()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = "Job description",
      ProposedRate = -100.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ProposedRate)
        .WithErrorMessage("ProposedRate must be greater than 0");
  }

  [Fact]
  public async Task Should_Fail_When_ProposedRate_Exceeds_Maximum()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = "Job description",
      ProposedRate = 1000001.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ProposedRate)
        .WithErrorMessage("ProposedRate cannot exceed $1,000,000");
  }

  #endregion

  #region JobPostingId Validation Tests

  [Fact]
  public async Task Should_Pass_When_JobPostingId_Is_Null()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = "Job description",
      ProposedRate = 5000.0,
      JobPostingId = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Fail_When_JobPostingId_Is_Empty()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = "Job description",
      ProposedRate = 5000.0,
      JobPostingId = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.JobPostingId)
        .WithErrorMessage("JobPostingId cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Fail_When_JobPostingId_Exceeds_MaxLength()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = "Job description",
      ProposedRate = 5000.0,
      JobPostingId = new string('a', 451) // Max is 450
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.JobPostingId)
        .WithErrorMessage("JobPostingId cannot be longer than 450 characters");
  }

  #endregion

  #region Edge Cases

  [Fact]
  public async Task Should_Pass_With_Unicode_In_Title()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Développeur Senior 高级开发人员",
      Description = "Job description",
      ProposedRate = 5000.0
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
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = "Nous recherchons un développeur expérimenté. 我们正在寻找有经验的开发人员。",
      ProposedRate = 5000.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Decimal_ProposedRate()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "Developer",
      Description = "Job description",
      ProposedRate = 5250.75
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Special_Characters_In_Title()
  {
    // Arrange
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = "contractor-123",
      Title = "C# .NET Developer (Remote)",
      Description = "Job description",
      ProposedRate = 5000.0
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
    var request = new InitiateJobOfferRequest
    {
      ContractorProfileId = string.Empty,
      Title = "<script>alert('xss')</script>",
      Description = "javascript:alert('xss')",
      ProposedRate = -100.0
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ContractorProfileId);
    result.ShouldHaveValidationErrorFor(x => x.Title);
    result.ShouldHaveValidationErrorFor(x => x.Description);
    result.ShouldHaveValidationErrorFor(x => x.ProposedRate);
  }

  #endregion
}
