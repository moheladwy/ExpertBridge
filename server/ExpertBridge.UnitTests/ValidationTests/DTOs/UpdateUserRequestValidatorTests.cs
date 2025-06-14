// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.UnitTests.ValidationTests.DTOs;

public class UpdateUserRequestValidatorTests
{
    private readonly UpdateUserRequestValidator _updateUserRequestValidator = new();

    private readonly UpdateUserRequest _validUpdateUserRequest = new()
    {
        ProviderId = Guid.NewGuid().ToString(),
        FirstName = "John",
        LastName = "Doe",
        Email = "john.doe@example.com",
        //Username = "johndoe",
        PhoneNumber = "+1234567890"
    };

    [Fact]
    public void Validate_ValidUpdateUserRequest_ShouldReturnNoErrors()
    {
        // Arrange
        // No need to arrange anything since the update user request is already valid

        // Act
        var validationResult = _updateUserRequestValidator.TestValidate(_validUpdateUserRequest);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenEmailIsInvalid_ShouldReturnErrors()
    {
        // Arrange
        var updateUserRequestWithNullEmail = _validUpdateUserRequest;
        updateUserRequestWithNullEmail.Email = null;

        var updateUserRequestWithEmptyEmail = _validUpdateUserRequest;
        updateUserRequestWithEmptyEmail.Email = string.Empty;

        var updateUserRequestWithInvalidEmail = _validUpdateUserRequest;
        updateUserRequestWithInvalidEmail.Email = "test";

        // Act
        var resultOfNullEmail = _updateUserRequestValidator.TestValidate(updateUserRequestWithNullEmail);
        var resultOfEmptyEmail = _updateUserRequestValidator.TestValidate(updateUserRequestWithEmptyEmail);
        var resultOfInvalidEmail = _updateUserRequestValidator.TestValidate(updateUserRequestWithInvalidEmail);

        // Assert
        resultOfNullEmail.ShouldHaveValidationErrorFor(x => x.Email);
        resultOfEmptyEmail.ShouldHaveValidationErrorFor(x => x.Email);
        resultOfInvalidEmail.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WhenPhoneNumberIsInvalid_ShouldReturnErrors()
    {
        // Arrange
        var updateUserRequestWithEmptyPhoneNumber = _validUpdateUserRequest;
        updateUserRequestWithEmptyPhoneNumber.PhoneNumber = string.Empty;

        var updateUserRequestWithInvalidPhoneNumber = _validUpdateUserRequest;
        updateUserRequestWithInvalidPhoneNumber.PhoneNumber = "test.test";

        var updateUserRequestWithLongPhoneNumber = _validUpdateUserRequest;
        updateUserRequestWithLongPhoneNumber.PhoneNumber =
            new string('1', UserEntityConstraints.MaxPhoneNumberLength + 1);

        // Act
        var resultOfEmptyPhoneNumber = _updateUserRequestValidator.TestValidate(updateUserRequestWithEmptyPhoneNumber);
        var resultOfInvalidPhoneNumber =
            _updateUserRequestValidator.TestValidate(updateUserRequestWithInvalidPhoneNumber);
        var resultOfLongPhoneNumber = _updateUserRequestValidator.TestValidate(updateUserRequestWithLongPhoneNumber);

        // Assert
        resultOfEmptyPhoneNumber.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        resultOfInvalidPhoneNumber.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        resultOfLongPhoneNumber.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Validate_WhenProviderIdIsInvalid_ShouldReturnErrors()
    {
        // Arrange
        var updateUserRequestWithNullProviderId = _validUpdateUserRequest;
        updateUserRequestWithNullProviderId.ProviderId = null;

        var updateUserRequestWithEmptyProviderId = _validUpdateUserRequest;
        updateUserRequestWithEmptyProviderId.ProviderId = string.Empty;

        var updateUserRequestWithInvalidProviderId = _validUpdateUserRequest;
        updateUserRequestWithInvalidProviderId.ProviderId = new string('n', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullProviderId = _updateUserRequestValidator.TestValidate(updateUserRequestWithNullProviderId);
        var resultOfEmptyProviderId = _updateUserRequestValidator.TestValidate(updateUserRequestWithEmptyProviderId);
        var resultOfInvalidProviderId =
            _updateUserRequestValidator.TestValidate(updateUserRequestWithInvalidProviderId);

        // Assert
        resultOfNullProviderId.ShouldHaveValidationErrorFor(x => x.ProviderId);
        resultOfEmptyProviderId.ShouldHaveValidationErrorFor(x => x.ProviderId);
        resultOfInvalidProviderId.ShouldHaveValidationErrorFor(x => x.ProviderId);
    }

    [Fact]
    public void Validate_WhenFirstNameIsInvalid_ShouldReturnErrors()
    {
        // Arrange
        var updateUserRequestWithNullFirstName = _validUpdateUserRequest;
        updateUserRequestWithNullFirstName.FirstName = null;

        var updateUserRequestWithEmptyFirstName = _validUpdateUserRequest;
        updateUserRequestWithEmptyFirstName.FirstName = string.Empty;

        var updateUserRequestWithInvalidFirstName = _validUpdateUserRequest;
        updateUserRequestWithInvalidFirstName.FirstName = new string('n', UserEntityConstraints.MaxNameLength + 1);

        // Act
        var resultOfNullFirstName = _updateUserRequestValidator.TestValidate(updateUserRequestWithNullFirstName);
        var resultOfEmptyFirstName = _updateUserRequestValidator.TestValidate(updateUserRequestWithEmptyFirstName);
        var resultOfInvalidFirstName = _updateUserRequestValidator.TestValidate(updateUserRequestWithInvalidFirstName);

        // Assert
        resultOfNullFirstName.ShouldHaveValidationErrorFor(x => x.FirstName);
        resultOfEmptyFirstName.ShouldHaveValidationErrorFor(x => x.FirstName);
        resultOfInvalidFirstName.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_WhenLastNameIsInvalid_ShouldReturnErrors()
    {
        // Arrange
        var updateUserRequestWithNullLastName = _validUpdateUserRequest;
        updateUserRequestWithNullLastName.LastName = null;

        var updateUserRequestWithEmptyLastName = _validUpdateUserRequest;
        updateUserRequestWithEmptyLastName.LastName = string.Empty;

        var updateUserRequestWithInvalidLastName = _validUpdateUserRequest;
        updateUserRequestWithInvalidLastName.LastName = new string('n', UserEntityConstraints.MaxNameLength + 1);

        // Act
        var resultOfNullLastName = _updateUserRequestValidator.TestValidate(updateUserRequestWithNullLastName);
        var resultOfEmptyLastName = _updateUserRequestValidator.TestValidate(updateUserRequestWithEmptyLastName);
        var resultOfInvalidLastName = _updateUserRequestValidator.TestValidate(updateUserRequestWithInvalidLastName);

        // Assert
        resultOfNullLastName.ShouldHaveValidationErrorFor(x => x.LastName);
        resultOfEmptyLastName.ShouldHaveValidationErrorFor(x => x.LastName);
        resultOfInvalidLastName.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    //[Fact]
    //public void Validate_WhenUsernameIsInvalid_ShouldReturnErrors()
    //{
    //    // Arrange
    //    var updateUserRequestWithNullUsername = _validUpdateUserRequest;
    //    updateUserRequestWithNullUsername.Username = null;

    //    var updateUserRequestWithEmptyUsername = _validUpdateUserRequest;
    //    updateUserRequestWithEmptyUsername.Username = string.Empty;

    //    var updateUserRequestWithInvalidUsername = _validUpdateUserRequest;
    //    updateUserRequestWithInvalidUsername.Username = new string('n', UserEntityConstraints.MaxUsernameLength + 1);

    //    // Act
    //    var resultOfNullUsername = _updateUserRequestValidator.TestValidate(updateUserRequestWithNullUsername);
    //    var resultOfEmptyUsername = _updateUserRequestValidator.TestValidate(updateUserRequestWithEmptyUsername);
    //    var resultOfInvalidUsername = _updateUserRequestValidator.TestValidate(updateUserRequestWithInvalidUsername);

    //    // Assert
    //    resultOfNullUsername.ShouldHaveValidationErrorFor(x => x.Username);
    //    resultOfEmptyUsername.ShouldHaveValidationErrorFor(x => x.Username);
    //    resultOfInvalidUsername.ShouldHaveValidationErrorFor(x => x.Username);
    //}
}
