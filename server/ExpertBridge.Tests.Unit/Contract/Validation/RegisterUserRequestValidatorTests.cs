// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for RegisterUserRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: ProviderId, Email, Username, FirstName, LastName validation with email format and name pattern validation.
/// </remarks>
public sealed class RegisterUserRequestValidatorTests
{
  private readonly RegisterUserRequestValidator _validator;

  public RegisterUserRequestValidatorTests()
  {
    _validator = new RegisterUserRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_When_All_Fields_Are_Valid()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "john.doe@example.com",
      Username = "johndoe",
      FirstName = "John",
      LastName = "Doe"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Names_Contain_Hyphens_And_Apostrophes()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-67890",
      Email = "mary-jane@example.com",
      Username = "maryjane",
      FirstName = "Mary-Jane",
      LastName = "O'Connor"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Names_Contain_Spaces()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-11111",
      Email = "maria@example.com",
      Username = "mariadelcarmen",
      FirstName = "Maria Del Carmen",
      LastName = "Garcia Lopez"
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
    var request = new RegisterUserRequest
    {
      ProviderId = new string('a', GlobalEntitiesConstraints.MaxIdLength),
      Email = new string('e', UserEntityConstraints.MaxEmailLength - 12) + "@example.com",
      Username = new string('u', UserEntityConstraints.MaxUsernameLength),
      FirstName = new string('F', UserEntityConstraints.MaxNameLength),
      LastName = new string('L', UserEntityConstraints.MaxNameLength)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region ProviderId Validation Tests

  [Fact]
  public async Task Should_Fail_When_ProviderId_Is_Null()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = null!,
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ProviderId)
        .WithErrorMessage("FirebaseId cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_ProviderId_Is_Empty()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = string.Empty,
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ProviderId)
        .WithErrorMessage("FirebaseId cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_ProviderId_Exceeds_MaxLength()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1),
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ProviderId)
        .WithErrorMessage($"FirebaseId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
  }

  #endregion

  #region Email Validation Tests

  [Fact]
  public async Task Should_Fail_When_Email_Is_Null()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = null!,
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Email)
        .WithErrorMessage("Email cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_Email_Is_Empty()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = string.Empty,
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Email)
        .WithErrorMessage("Email cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Email_Is_Invalid_Format()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "invalid-email-format",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Email)
        .WithErrorMessage("Email must be a valid email address");
  }

  [Fact]
  public async Task Should_Fail_When_Email_Exceeds_MaxLength()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = new string('e', UserEntityConstraints.MaxEmailLength + 1) + "@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Email)
        .WithErrorMessage($"Email cannot be longer than {UserEntityConstraints.MaxEmailLength} characters");
  }

  [Fact]
  public async Task Should_Fail_When_Email_Contains_Double_Dots()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "test..user@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Email)
        .WithErrorMessage("Email cannot contain consecutive dots");
  }

  [Fact]
  public async Task Should_Fail_When_Email_Starts_With_Dot()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = ".testuser@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Email)
        .WithErrorMessage("Email cannot start with a dot");
  }

  [Fact]
  public async Task Should_Fail_When_Email_Ends_With_Dot()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase123",
      Email = "test@example.com.",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Email)
        .WithErrorMessage("Email cannot end with a dot");
  }

  #endregion

  #region Username Validation Tests

  [Fact]
  public async Task Should_Fail_When_Username_Is_Null()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "test@example.com",
      Username = null!,
      FirstName = "Test",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Username)
        .WithErrorMessage("Username cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_Username_Is_Empty()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "test@example.com",
      Username = string.Empty,
      FirstName = "Test",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Username)
        .WithErrorMessage("Username cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Username_Exceeds_MaxLength()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "test@example.com",
      Username = new string('u', UserEntityConstraints.MaxUsernameLength + 1),
      FirstName = "Test",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Username)
        .WithErrorMessage($"Username cannot be longer than {UserEntityConstraints.MaxUsernameLength} characters");
  }

  #endregion

  #region FirstName Validation Tests

  [Fact]
  public async Task Should_Fail_When_FirstName_Is_Null()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = null!,
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.FirstName)
        .WithErrorMessage("FirstName cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_FirstName_Is_Empty()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = string.Empty,
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.FirstName)
        .WithErrorMessage("FirstName cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_FirstName_Exceeds_MaxLength()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = new string('F', UserEntityConstraints.MaxNameLength + 1),
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.FirstName)
        .WithErrorMessage($"FirstName cannot be longer than {UserEntityConstraints.MaxNameLength} characters");
  }

  [Fact]
  public async Task Should_Fail_When_FirstName_Contains_Numbers()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "John123",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.FirstName)
        .WithErrorMessage("FirstName can only contain letters, spaces, hyphens, and apostrophes");
  }

  [Fact]
  public async Task Should_Fail_When_FirstName_Contains_Special_Characters()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "John@Doe",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.FirstName)
        .WithErrorMessage("FirstName can only contain letters, spaces, hyphens, and apostrophes");
  }

  #endregion

  #region LastName Validation Tests

  [Fact]
  public async Task Should_Fail_When_LastName_Is_Null()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = null!
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.LastName)
        .WithErrorMessage("LastName cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_LastName_Is_Empty()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.LastName)
        .WithErrorMessage("LastName cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_LastName_Exceeds_MaxLength()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = new string('L', UserEntityConstraints.MaxNameLength + 1)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.LastName)
        .WithErrorMessage($"LastName cannot be longer than {UserEntityConstraints.MaxNameLength} characters");
  }

  [Fact]
  public async Task Should_Fail_When_LastName_Contains_Numbers()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User123"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.LastName)
        .WithErrorMessage("LastName can only contain letters, spaces, hyphens, and apostrophes");
  }

  [Fact]
  public async Task Should_Fail_When_LastName_Contains_Special_Characters()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User$%^"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.LastName)
        .WithErrorMessage("LastName can only contain letters, spaces, hyphens, and apostrophes");
  }

  #endregion

  #region Edge Cases and Parameterized Tests

  [Theory]
  [InlineData("test@example.com", true)]
  [InlineData("user.name@example.co.uk", true)]
  [InlineData("user+tag@example.com", true)]
  [InlineData("user_name@example.com", true)]
  [InlineData("123@example.com", true)]
  [InlineData("@example.com", false)]
  [InlineData("user@", false)]
  [InlineData("user@@example.com", false)]
  public async Task Should_Validate_Email_Formats(string email, bool shouldBeValid)
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = email,
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    if (shouldBeValid)
    {
      result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }
    else
    {
      result.ShouldHaveValidationErrorFor(x => x.Email);
    }
  }

  [Theory]
  [InlineData("John", true)]
  [InlineData("Mary-Jane", true)]
  [InlineData("O'Connor", true)]
  [InlineData("Maria Del Carmen", true)]
  [InlineData("Jean-Pierre", true)]
  [InlineData("John123", false)]
  [InlineData("Test@User", false)]
  [InlineData("User_Name", false)]
  public async Task Should_Validate_FirstName_Patterns(string firstName, bool shouldBeValid)
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = "firebase-uid-12345",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = firstName,
      LastName = "TestLast"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    if (shouldBeValid)
    {
      result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
    }
    else
    {
      result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }
  }

  [Fact]
  public async Task Should_Fail_With_Multiple_Validation_Errors()
  {
    // Arrange
    var request = new RegisterUserRequest
    {
      ProviderId = string.Empty,
      Email = "invalid-email",
      Username = string.Empty,
      FirstName = "John123",
      LastName = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ProviderId);
    result.ShouldHaveValidationErrorFor(x => x.Email);
    result.ShouldHaveValidationErrorFor(x => x.Username);
    result.ShouldHaveValidationErrorFor(x => x.FirstName);
    result.ShouldHaveValidationErrorFor(x => x.LastName);
  }

  #endregion
}
