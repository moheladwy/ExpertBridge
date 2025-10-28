// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for UpdateProfileRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: optional JobTitle, Bio, FirstName, LastName, Username, PhoneNumber, Skills with XSS prevention and conditional validation.
/// </remarks>
public sealed class UpdateProfileRequestValidatorTests
{
  private readonly UpdateProfileRequestValidator _validator;

  public UpdateProfileRequestValidatorTests()
  {
    _validator = new UpdateProfileRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_When_All_Fields_Are_Null()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      JobTitle = null,
      Bio = null,
      FirstName = null,
      LastName = null,
      Username = null,
      PhoneNumber = null,
      Skills = null!
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_All_Fields_Are_Valid()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      JobTitle = "Senior Software Engineer",
      Bio = "Experienced software developer with 10 years in the industry.",
      FirstName = "John",
      LastName = "Doe",
      Username = "johndoe123",
      PhoneNumber = "+14155552671",
      Skills = ["skill-id-1", "skill-id-2", "skill-id-3"]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Valid_JobTitle_And_Bio()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      JobTitle = "Data Scientist | ML Engineer",
      Bio = "Passionate about AI and machine learning. Love solving complex problems with data-driven solutions."
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Valid_Names_Containing_Hyphens_And_Apostrophes()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      FirstName = "Mary-Jane",
      LastName = "O'Connor"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Valid_Username_Patterns()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Username = "user.name_123-test"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Maximum_Length_Fields()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      JobTitle = new string('J', ProfileEntityConstraints.JobTitleMaxLength),
      Bio = new string('B', ProfileEntityConstraints.BioMaxLength),
      FirstName = new string('F', UserEntityConstraints.MaxNameLength),
      LastName = new string('L', UserEntityConstraints.MaxNameLength),
      Username = new string('u', UserEntityConstraints.MaxUsernameLength),
      PhoneNumber = "+1234567890123456", // 17 chars
      Skills = [new string('s', GlobalEntitiesConstraints.MaxIdLength)]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Empty_Skills_Collection()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Skills = []
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region JobTitle Validation Tests (Optional, XSS)

  [Fact]
  public async Task Should_Pass_When_JobTitle_Is_Null()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      JobTitle = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.JobTitle);
  }

  [Fact]
  public async Task Should_Fail_When_JobTitle_Is_Empty()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      JobTitle = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.JobTitle)
        .WithErrorMessage("Job title cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Fail_When_JobTitle_Exceeds_MaxLength()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      JobTitle = new string('J', ProfileEntityConstraints.JobTitleMaxLength + 1)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.JobTitle)
        .WithErrorMessage($"Job title cannot be longer than {ProfileEntityConstraints.JobTitleMaxLength} characters");
  }

  [Fact]
  public async Task Should_Fail_When_JobTitle_Contains_Script_Tags()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      JobTitle = "Software Engineer<script>alert('XSS')</script>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.JobTitle)
        .WithErrorMessage("Job title cannot contain script tags");
  }

  [Fact]
  public async Task Should_Fail_When_JobTitle_Contains_JavaScript_Protocol()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      JobTitle = "Click here: javascript:alert('XSS')"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.JobTitle)
        .WithErrorMessage("Job title contains potentially dangerous content");
  }

  [Fact]
  public async Task Should_Fail_When_JobTitle_Contains_Data_URI()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      JobTitle = "data:text/html,<script>alert('XSS')</script>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.JobTitle)
        .WithErrorMessage("Job title contains potentially dangerous content");
  }

  [Fact]
  public async Task Should_Fail_When_JobTitle_Contains_Event_Handler()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      JobTitle = "Engineer onclick=alert('XSS')"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.JobTitle)
        .WithErrorMessage("Job title contains potentially dangerous content");
  }

  #endregion

  #region Bio Validation Tests (Optional, XSS)

  [Fact]
  public async Task Should_Pass_When_Bio_Is_Null()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Bio = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Bio);
  }

  [Fact]
  public async Task Should_Fail_When_Bio_Is_Empty()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Bio = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Bio)
        .WithErrorMessage("Bio cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Fail_When_Bio_Exceeds_MaxLength()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Bio = new string('B', ProfileEntityConstraints.BioMaxLength + 1)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Bio)
        .WithErrorMessage($"Bio cannot be longer than {ProfileEntityConstraints.BioMaxLength} characters");
  }

  [Fact]
  public async Task Should_Fail_When_Bio_Contains_Script_Tags()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Bio = "I love coding <script>malicious()</script> and design."
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Bio)
        .WithErrorMessage("Bio cannot contain script tags");
  }

  [Fact]
  public async Task Should_Fail_When_Bio_Contains_JavaScript_Protocol()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Bio = "Visit my site: javascript:void(0)"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Bio)
        .WithErrorMessage("Bio contains potentially dangerous content");
  }

  [Fact]
  public async Task Should_Fail_When_Bio_Contains_Data_URI()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Bio = "Check this out: data:text/html,<h1>Test</h1>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Bio)
        .WithErrorMessage("Bio contains potentially dangerous content");
  }

  [Fact]
  public async Task Should_Fail_When_Bio_Contains_Event_Handler()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Bio = "My bio onload=alert('xss')"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Bio)
        .WithErrorMessage("Bio contains potentially dangerous content");
  }

  #endregion

  #region FirstName Validation Tests (Optional, Name Pattern)

  [Fact]
  public async Task Should_Pass_When_FirstName_Is_Null()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      FirstName = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
  }

  [Fact]
  public async Task Should_Fail_When_FirstName_Is_Empty()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      FirstName = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.FirstName)
        .WithErrorMessage("First name cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Fail_When_FirstName_Exceeds_MaxLength()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      FirstName = new string('F', UserEntityConstraints.MaxNameLength + 1)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.FirstName)
        .WithErrorMessage($"First name cannot be longer than {UserEntityConstraints.MaxNameLength} characters");
  }

  [Fact]
  public async Task Should_Fail_When_FirstName_Contains_Numbers()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      FirstName = "John123"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.FirstName)
        .WithErrorMessage("First name can only contain letters, spaces, hyphens, and apostrophes");
  }

  [Fact]
  public async Task Should_Fail_When_FirstName_Contains_Special_Characters()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      FirstName = "John@Doe"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.FirstName)
        .WithErrorMessage("First name can only contain letters, spaces, hyphens, and apostrophes");
  }

  #endregion

  #region LastName Validation Tests (Optional, Name Pattern)

  [Fact]
  public async Task Should_Pass_When_LastName_Is_Null()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      LastName = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.LastName);
  }

  [Fact]
  public async Task Should_Fail_When_LastName_Is_Empty()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      LastName = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.LastName)
        .WithErrorMessage("Last name cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Fail_When_LastName_Exceeds_MaxLength()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      LastName = new string('L', UserEntityConstraints.MaxNameLength + 1)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.LastName)
        .WithErrorMessage($"Last name cannot be longer than {UserEntityConstraints.MaxNameLength} characters");
  }

  [Fact]
  public async Task Should_Fail_When_LastName_Contains_Numbers()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      LastName = "Doe123"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.LastName)
        .WithErrorMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");
  }

  [Fact]
  public async Task Should_Fail_When_LastName_Contains_Special_Characters()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      LastName = "Doe$%^"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.LastName)
        .WithErrorMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");
  }

  #endregion

  #region Username Validation Tests (Optional, Regex Pattern)

  [Fact]
  public async Task Should_Pass_When_Username_Is_Null()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Username = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Username);
  }

  [Fact]
  public async Task Should_Fail_When_Username_Is_Empty()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Username = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Username)
        .WithErrorMessage("Username cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Fail_When_Username_Exceeds_MaxLength()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Username = new string('u', UserEntityConstraints.MaxUsernameLength + 1)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Username)
        .WithErrorMessage($"Username cannot be longer than {UserEntityConstraints.MaxUsernameLength} characters");
  }

  [Fact]
  public async Task Should_Fail_When_Username_Contains_Spaces()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Username = "user name"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Username)
        .WithErrorMessage("Username can only contain letters, numbers, periods, hyphens, and underscores");
  }

  [Fact]
  public async Task Should_Fail_When_Username_Contains_Special_Characters()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Username = "user@name!"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Username)
        .WithErrorMessage("Username can only contain letters, numbers, periods, hyphens, and underscores");
  }

  #endregion

  #region PhoneNumber Validation Tests (Optional, International Format)

  [Fact]
  public async Task Should_Pass_When_PhoneNumber_Is_Null()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      PhoneNumber = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
  }

  [Fact]
  public async Task Should_Fail_When_PhoneNumber_Is_Empty()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      PhoneNumber = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
        .WithErrorMessage("Phone number cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Fail_When_PhoneNumber_Exceeds_MaxLength()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      PhoneNumber = new string('1', UserEntityConstraints.MaxPhoneNumberLength + 1)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
        .WithErrorMessage($"Phone number cannot be longer than {UserEntityConstraints.MaxPhoneNumberLength} characters");
  }

  [Fact]
  public async Task Should_Fail_When_PhoneNumber_Is_Invalid_Format()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      PhoneNumber = "abc-def-ghij"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
        .WithErrorMessage("Phone number must be in a valid international format");
  }

  #endregion

  #region Skills Validation Tests (Optional Collection)

  [Fact]
  public async Task Should_Pass_When_Skills_Is_Null()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Skills = null!
    };    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Skills);
  }

  [Fact]
  public async Task Should_Pass_With_Valid_Skill_IDs()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Skills = ["skill-1", "skill-2", "skill-3"]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Skills);
  }

  [Fact]
  public async Task Should_Fail_When_Skill_ID_Is_Null()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Skills = [null!]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Skills)
        .WithErrorMessage("Skill ID cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_Skill_ID_Is_Empty()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Skills = [string.Empty]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Skills)
        .WithErrorMessage("Skill ID cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Skill_ID_Exceeds_MaxLength()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Skills = [new string('s', GlobalEntitiesConstraints.MaxIdLength + 1)]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Skills)
        .WithErrorMessage($"Each skill ID cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
  }

  [Fact]
  public async Task Should_Fail_When_Multiple_Skill_IDs_Are_Invalid()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Skills = [string.Empty, new string('s', GlobalEntitiesConstraints.MaxIdLength + 1)]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Skills);
  }

  #endregion

  #region Edge Cases and Parameterized Tests

  [Theory]
  [InlineData("Software Engineer", true)]
  [InlineData("Senior Developer | Team Lead", true)]
  [InlineData("Data Scientist & ML Engineer", true)]
  [InlineData("<script>alert('xss')</script>", false)]
  [InlineData("javascript:void(0)", false)]
  [InlineData("onclick=alert('xss')", false)]
  public async Task Should_Validate_JobTitle_XSS_Patterns(string jobTitle, bool shouldBeValid)
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      JobTitle = jobTitle
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    if (shouldBeValid)
    {
      result.ShouldNotHaveValidationErrorFor(x => x.JobTitle);
    }
    else
    {
      result.ShouldHaveValidationErrorFor(x => x.JobTitle);
    }
  }

  [Theory]
  [InlineData("johndoe", true)]
  [InlineData("john.doe", true)]
  [InlineData("john-doe", true)]
  [InlineData("john_doe", true)]
  [InlineData("johndoe123", true)]
  [InlineData("john doe", false)]
  [InlineData("john@doe", false)]
  [InlineData("john#doe", false)]
  public async Task Should_Validate_Username_Patterns(string username, bool shouldBeValid)
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      Username = username
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    if (shouldBeValid)
    {
      result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }
    else
    {
      result.ShouldHaveValidationErrorFor(x => x.Username);
    }
  }

  [Theory]
  [InlineData("+14155552671", true)]
  [InlineData("+442071234567", true)]
  [InlineData("14155552671", true)] // Without + is valid for this validator
  [InlineData("123-456-7890", false)] // Hyphens not allowed
  [InlineData("(123) 456-7890", false)] // Parentheses not allowed
  public async Task Should_Validate_PhoneNumber_Formats(string phoneNumber, bool shouldBeValid)
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      PhoneNumber = phoneNumber
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    if (shouldBeValid)
    {
      result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }
    else
    {
      result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }
  }

  [Fact]
  public async Task Should_Fail_With_Multiple_Validation_Errors_Across_All_Fields()
  {
    // Arrange
    var request = new UpdateProfileRequest
    {
      JobTitle = string.Empty,
      Bio = string.Empty,
      FirstName = "John123",
      LastName = string.Empty,
      Username = "user name",
      PhoneNumber = string.Empty,
      Skills = [string.Empty]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.JobTitle);
    result.ShouldHaveValidationErrorFor(x => x.Bio);
    result.ShouldHaveValidationErrorFor(x => x.FirstName);
    result.ShouldHaveValidationErrorFor(x => x.LastName);
    result.ShouldHaveValidationErrorFor(x => x.Username);
    result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    result.ShouldHaveValidationErrorFor(x => x.Skills);
  }

  #endregion
}
