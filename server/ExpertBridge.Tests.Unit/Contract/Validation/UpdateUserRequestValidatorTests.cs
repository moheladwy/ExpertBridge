// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for UpdateUserRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: required ProviderId and Email, optional FirstName, LastName, PhoneNumber with E.164 format validation.
/// </remarks>
public sealed class UpdateUserRequestValidatorTests
{
    private readonly UpdateUserRequestValidator _validator;

    public UpdateUserRequestValidatorTests()
    {
        _validator = new UpdateUserRequestValidator();
    }

    #region Happy Path Tests

    [Fact]
    public async Task Should_Pass_When_Only_Required_Fields_Are_Provided()
    {
        // Arrange
        var request = new UpdateUserRequest { ProviderId = "firebase-uid-12345", Email = "john.doe@example.com" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_When_All_Fields_Are_Valid()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345",
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+14155552671"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_With_Valid_E164_Phone_Numbers()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345", Email = "test@example.com", PhoneNumber = "+12025551234"
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
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-67890",
            Email = "mary-jane@example.com",
            FirstName = "Mary-Jane",
            LastName = "O'Connor"
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
        var request = new UpdateUserRequest
        {
            ProviderId = new string('p', GlobalEntitiesConstraints.MaxIdLength),
            Email = new string('e', UserEntityConstraints.MaxEmailLength - 12) + "@example.com",
            FirstName = new string('F', UserEntityConstraints.MaxNameLength),
            LastName = new string('L', UserEntityConstraints.MaxNameLength),
            PhoneNumber = "+1234567890123" // 14 digits (within E.164 limit)
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
        var request = new UpdateUserRequest { ProviderId = null!, Email = "test@example.com" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProviderId)
            .WithErrorMessage("Provider ID cannot be null");
    }

    [Fact]
    public async Task Should_Fail_When_ProviderId_Is_Empty()
    {
        // Arrange
        var request = new UpdateUserRequest { ProviderId = string.Empty, Email = "test@example.com" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProviderId)
            .WithErrorMessage("Provider ID cannot be empty");
    }

    [Fact]
    public async Task Should_Fail_When_ProviderId_Exceeds_MaxLength()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            ProviderId = new string('p', GlobalEntitiesConstraints.MaxIdLength + 1), Email = "test@example.com"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProviderId)
            .WithErrorMessage($"Provider ID cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
    }

    #endregion

    #region Email Validation Tests

    [Fact]
    public async Task Should_Fail_When_Email_Is_Null()
    {
        // Arrange
        var request = new UpdateUserRequest { ProviderId = "firebase-uid-12345", Email = null! };

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
        var request = new UpdateUserRequest { ProviderId = "firebase-uid-12345", Email = string.Empty };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email cannot be empty");
    }

    [Fact]
    public async Task Should_Fail_When_Email_Exceeds_MaxLength()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345",
            Email = new string('e', UserEntityConstraints.MaxEmailLength + 1) + "@example.com"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage($"Email cannot be longer than {UserEntityConstraints.MaxEmailLength} characters");
    }

    [Fact]
    public async Task Should_Fail_When_Email_Is_Invalid_Format()
    {
        // Arrange
        var request = new UpdateUserRequest { ProviderId = "firebase-uid-12345", Email = "invalid-email-format" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email must be a valid email address");
    }

    [Fact]
    public async Task Should_Fail_When_Email_Contains_Double_Dots()
    {
        // Arrange
        var request = new UpdateUserRequest { ProviderId = "firebase-uid-12345", Email = "test..user@example.com" };

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
        var request = new UpdateUserRequest { ProviderId = "firebase-uid-12345", Email = ".testuser@example.com" };

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
        var request = new UpdateUserRequest { ProviderId = "firebase123", Email = "test@example.com." };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email cannot end with a dot");
    }

    #endregion

    #region FirstName Validation Tests (Optional)

    [Fact]
    public async Task Should_Pass_When_FirstName_Is_Null()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345", Email = "test@example.com", FirstName = null
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
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345", Email = "test@example.com", FirstName = string.Empty
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
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345",
            Email = "test@example.com",
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
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345", Email = "test@example.com", FirstName = "John123"
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
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345", Email = "test@example.com", FirstName = "John@Doe"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name can only contain letters, spaces, hyphens, and apostrophes");
    }

    #endregion

    #region LastName Validation Tests (Optional)

    [Fact]
    public async Task Should_Pass_When_LastName_Is_Null()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345", Email = "test@example.com", LastName = null
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
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345", Email = "test@example.com", LastName = string.Empty
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
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345",
            Email = "test@example.com",
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
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345", Email = "test@example.com", LastName = "Doe123"
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
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345", Email = "test@example.com", LastName = "Doe$%^"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");
    }

    #endregion

    #region PhoneNumber Validation Tests (Optional E.164)

    [Fact]
    public async Task Should_Pass_When_PhoneNumber_Is_Null()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345", Email = "test@example.com", PhoneNumber = null
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
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345", Email = "test@example.com", PhoneNumber = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("PhoneNumber cannot be empty when provided");
    }

    [Fact]
    public async Task Should_Fail_When_PhoneNumber_Exceeds_MaxLength()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345",
            Email = "test@example.com",
            PhoneNumber = "+12345678901234567890" // Exceeds 20 chars
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage(
                $"PhoneNumber cannot be longer than {UserEntityConstraints.MaxPhoneNumberLength} characters");
    }

    [Fact]
    public async Task Should_Fail_When_PhoneNumber_Is_Not_E164_Format()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345",
            Email = "test@example.com",
            PhoneNumber = "123-456-7890" // Not E.164 format
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("PhoneNumber must be in E.164 format (+[country code][number], 10-15 digits)");
    }

    [Fact]
    public async Task Should_Fail_When_PhoneNumber_Missing_Plus_Sign()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345", Email = "test@example.com", PhoneNumber = "14155552671"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("PhoneNumber must be in E.164 format (+[country code][number], 10-15 digits)");
    }

    [Fact]
    public async Task Should_Fail_When_PhoneNumber_Too_Short()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345",
            Email = "test@example.com",
            PhoneNumber = "+1234567" // Only 7 digits (too short)
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("PhoneNumber must be in E.164 format (+[country code][number], 10-15 digits)");
    }

    [Fact]
    public async Task Should_Fail_When_PhoneNumber_Too_Long()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345",
            Email = "test@example.com",
            PhoneNumber = "+12345678901234567" // 17 digits (too long for E.164)
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("PhoneNumber must be in E.164 format (+[country code][number], 10-15 digits)");
    }

    #endregion

    #region Edge Cases and Parameterized Tests

    [Theory]
    [InlineData("+14155552671", true)] // US number
    [InlineData("+442071234567", true)] // UK number
    [InlineData("+861234567890", true)] // China number
    [InlineData("+61412345678", true)] // Australia number
    [InlineData("+49301234567", true)] // Germany number
    [InlineData("+1234567890", true)] // Minimum valid (10 digits)
    [InlineData("+123456789012345", true)] // Maximum valid (15 digits)
    [InlineData("14155552671", false)] // Missing +
    [InlineData("+0123456789", false)] // Starts with 0 (invalid country code)
    [InlineData("+1234567", false)] // Too short (7 digits)
    [InlineData("+1234567890123456", false)] // Too long (16 digits)
    [InlineData("+1-415-555-2671", false)] // Contains hyphens
    [InlineData("+1 415 555 2671", false)] // Contains spaces
    public async Task Should_Validate_PhoneNumber_E164_Formats(string phoneNumber, bool shouldBeValid)
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345", Email = "test@example.com", PhoneNumber = phoneNumber
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

    [Theory]
    [InlineData("John", true)]
    [InlineData("Mary-Jane", true)]
    [InlineData("O'Connor", true)]
    [InlineData("Maria Del Carmen", true)]
    [InlineData("Jean-Pierre", true)]
    [InlineData("John123", false)]
    [InlineData("Test@User", false)]
    [InlineData("User_Name", false)]
    public async Task Should_Validate_Name_Patterns(string name, bool shouldBeValid)
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            ProviderId = "firebase-uid-12345", Email = "test@example.com", FirstName = name, LastName = "TestLast"
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
        var request = new UpdateUserRequest
        {
            ProviderId = string.Empty,
            Email = "invalid-email",
            FirstName = "John123",
            LastName = string.Empty,
            PhoneNumber = "not-a-phone"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProviderId);
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    #endregion
}
