using Core.Entities;
using Core.Entities.Users;
using Core.Requests.RegisterUser;
using FluentValidation.TestHelper;

namespace UnitTests.ValidationTests.DTOs;

public class RegisterUserRequestValidatorTests
{
    private readonly RegisterUserRequestValidator _registerUserRequestValidator = new();

    private readonly RegisterUserRequest _validRegisterUserRequest = new()
    {
        Email = "test@gmail.com",
        ProviderId = Guid.NewGuid().ToString(),
        FirstName = "Test",
        LastName = "Test",
        Username = "test"
    };

    [Fact]
    public void Validate_ValidRegisterUserRequest_ShouldReturnNoErrors()
    {
        // Arrange
        // No need to arrange anything since the register user request is already valid

        // Act
        var validationResult = _registerUserRequestValidator.TestValidate(_validRegisterUserRequest);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenEmailIsInvalid_ShouldReturnErrors()
    {
        // Arrange
        var registerUserRequestWithNullEmail = _validRegisterUserRequest;
        registerUserRequestWithNullEmail.Email = null;

        var registerUserRequestWithEmptyEmail = _validRegisterUserRequest;
        registerUserRequestWithEmptyEmail.Email = string.Empty;

        var registerUserRequestWithInvalidEmail = _validRegisterUserRequest;
        registerUserRequestWithInvalidEmail.Email = "test";

        // Act
        var resultOfNullEmail = _registerUserRequestValidator.TestValidate(registerUserRequestWithNullEmail);
        var resultOfEmptyEmail = _registerUserRequestValidator.TestValidate(registerUserRequestWithEmptyEmail);
        var resultOfInvalidEmail = _registerUserRequestValidator.TestValidate(registerUserRequestWithInvalidEmail);

        // Assert
        resultOfNullEmail.ShouldHaveValidationErrorFor(x => x.Email);
        resultOfEmptyEmail.ShouldHaveValidationErrorFor(x => x.Email);
        resultOfInvalidEmail.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WhenFirebaseIdIsInvalid_ShouldReturnErrors()
    {
        // Arrange
        var registerUserRequestWithNullFirebaseId = _validRegisterUserRequest;
        registerUserRequestWithNullFirebaseId.ProviderId = null;

        var registerUserRequestWithEmptyFirebaseId = _validRegisterUserRequest;
        registerUserRequestWithEmptyFirebaseId.ProviderId = string.Empty;

        var registerUserRequestWithInvalidFirebaseId = _validRegisterUserRequest;
        registerUserRequestWithInvalidFirebaseId.ProviderId =
            new string('n', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullFirebaseId = _registerUserRequestValidator.TestValidate(registerUserRequestWithNullFirebaseId);
        var resultOfEmptyFirebaseId =
            _registerUserRequestValidator.TestValidate(registerUserRequestWithEmptyFirebaseId);
        var resultOfInvalidFirebaseId =
            _registerUserRequestValidator.TestValidate(registerUserRequestWithInvalidFirebaseId);

        // Assert
        resultOfNullFirebaseId.ShouldHaveValidationErrorFor(x => x.ProviderId);
        resultOfEmptyFirebaseId.ShouldHaveValidationErrorFor(x => x.ProviderId);
        resultOfInvalidFirebaseId.ShouldHaveValidationErrorFor(x => x.ProviderId);
    }

    [Fact]
    public void Validate_WhenFirstNameIsInvalid_ShouldReturnErrors()
    {
        // Arrange
        var registerUserRequestWithNullFirstName = _validRegisterUserRequest;
        registerUserRequestWithNullFirstName.FirstName = null;

        var registerUserRequestWithEmptyFirstName = _validRegisterUserRequest;
        registerUserRequestWithEmptyFirstName.FirstName = string.Empty;

        var registerUserRequestWithInvalidFirstName = _validRegisterUserRequest;
        registerUserRequestWithInvalidFirstName.FirstName = new string('n', UserEntityConstraints.MaxNameLength + 1);

        // Act
        var resultOfNullFirstName = _registerUserRequestValidator.TestValidate(registerUserRequestWithNullFirstName);
        var resultOfEmptyFirstName = _registerUserRequestValidator.TestValidate(registerUserRequestWithEmptyFirstName);
        var resultOfInvalidFirstName =
            _registerUserRequestValidator.TestValidate(registerUserRequestWithInvalidFirstName);

        // Assert
        resultOfNullFirstName.ShouldHaveValidationErrorFor(x => x.FirstName);
        resultOfEmptyFirstName.ShouldHaveValidationErrorFor(x => x.FirstName);
        resultOfInvalidFirstName.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_WhenLastNameIsInvalid_ShouldReturnErrors()
    {
        // Arrange
        var registerUserRequestWithNullLastName = _validRegisterUserRequest;
        registerUserRequestWithNullLastName.LastName = null;

        var registerUserRequestWithEmptyLastName = _validRegisterUserRequest;
        registerUserRequestWithEmptyLastName.LastName = string.Empty;

        var registerUserRequestWithInvalidLastName = _validRegisterUserRequest;
        registerUserRequestWithInvalidLastName.LastName = new string('n', UserEntityConstraints.MaxNameLength + 1);

        // Act
        var resultOfNullLastName = _registerUserRequestValidator.TestValidate(registerUserRequestWithNullLastName);
        var resultOfEmptyLastName = _registerUserRequestValidator.TestValidate(registerUserRequestWithEmptyLastName);
        var resultOfInvalidLastName =
            _registerUserRequestValidator.TestValidate(registerUserRequestWithInvalidLastName);

        // Assert
        resultOfNullLastName.ShouldHaveValidationErrorFor(x => x.LastName);
        resultOfEmptyLastName.ShouldHaveValidationErrorFor(x => x.LastName);
        resultOfInvalidLastName.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_WhenUsernameIsInvalid_ShouldReturnErrors()
    {
        // Arrange
        var registerUserRequestWithNullUsername = _validRegisterUserRequest;
        registerUserRequestWithNullUsername.Username = null;

        var registerUserRequestWithEmptyUsername = _validRegisterUserRequest;
        registerUserRequestWithEmptyUsername.Username = string.Empty;

        var registerUserRequestWithInvalidUsername = _validRegisterUserRequest;
        registerUserRequestWithInvalidUsername.Username = new string('n', UserEntityConstraints.MaxUsernameLength + 1);

        // Act
        var resultOfNullUsername = _registerUserRequestValidator.TestValidate(registerUserRequestWithNullUsername);
        var resultOfEmptyUsername = _registerUserRequestValidator.TestValidate(registerUserRequestWithEmptyUsername);
        var resultOfInvalidUsername =
            _registerUserRequestValidator.TestValidate(registerUserRequestWithInvalidUsername);

        // Assert
        resultOfNullUsername.ShouldHaveValidationErrorFor(x => x.Username);
        resultOfEmptyUsername.ShouldHaveValidationErrorFor(x => x.Username);
        resultOfInvalidUsername.ShouldHaveValidationErrorFor(x => x.Username);
    }
}
