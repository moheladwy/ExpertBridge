using Core.Entities;
using Core.Entities.Users;
using FluentValidation.TestHelper;

namespace UnitTests.ValidationTests.Entities;

public class UserValidatorTests
{
    private readonly UserEntityValidator _validator = new();

    private readonly User _validUser = new()
    {
        Id = Guid.NewGuid().ToString(),
        ProviderId = Guid.NewGuid().ToString(),
        Email = "user@example.com",
        Username = "johndoe",
        FirstName = "John",
        LastName = "Doe",
        PhoneNumber = "1234567890",
        IsBanned = false,
        IsDeleted = false,
        IsEmailVerified = false,
        IsOnboarded = false,
        CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public void ValidateUser_WhenUserIsValid_ShouldReturnTrue()
    {
        // No need to arrange anything since the user is already valid
        // Act
        var result = _validator.TestValidate(_validUser);
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidateUser_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var UserWithNullId = _validUser;
        UserWithNullId.Id = null;

        var UserWithEmptyId = _validUser;
        UserWithEmptyId.Id = string.Empty;

        var UserWithLongId = _validUser;
        UserWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _validator.TestValidate(UserWithNullId);
        var resultOfEmptyId = _validator.TestValidate(UserWithEmptyId);
        var resultOfLongId = _validator.TestValidate(UserWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidateUser_WhenEmailIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var UserWithNullEmail = _validUser;
        UserWithNullEmail.Email = null;

        var UserWithEmptyEmail = _validUser;
        UserWithEmptyEmail.Email = string.Empty;

        var UserWithInvalidEmail = _validUser;
        UserWithInvalidEmail.Email = "invalidemail";

        var UserWithLongEmail = _validUser;
        UserWithLongEmail.Email = new string('a', UserEntityConstraints.MaxEmailLength + 1) + "@example.com";

        // Act
        var resultOfNullEmail = _validator.TestValidate(UserWithNullEmail);
        var resultOfEmptyEmail = _validator.TestValidate(UserWithEmptyEmail);
        var resultOfInvalidEmail = _validator.TestValidate(UserWithInvalidEmail);
        var resultOfLongEmail = _validator.TestValidate(UserWithLongEmail);

        // Assert
        resultOfNullEmail.ShouldHaveValidationErrorFor(x => x.Email);
        resultOfEmptyEmail.ShouldHaveValidationErrorFor(x => x.Email);
        resultOfInvalidEmail.ShouldHaveValidationErrorFor(x => x.Email);
        resultOfLongEmail.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void ValidateUser_WhenUsernameIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var UserWithNullUsername = _validUser;
        UserWithNullUsername.Username = null;

        var UserWithEmptyUsername = _validUser;
        UserWithEmptyUsername.Username = string.Empty;

        var UserWithLongUsername = _validUser;
        UserWithLongUsername.Username = new string('a', UserEntityConstraints.MaxUsernameLength + 1);

        // Act
        var resultOfNullUsername = _validator.TestValidate(UserWithNullUsername);
        var resultOfEmptyUsername = _validator.TestValidate(UserWithEmptyUsername);
        var resultOfLongUsername = _validator.TestValidate(UserWithLongUsername);

        // Assert
        resultOfNullUsername.ShouldHaveValidationErrorFor(x => x.Username);
        resultOfEmptyUsername.ShouldHaveValidationErrorFor(x => x.Username);
        resultOfLongUsername.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void ValidateUser_WhenFirstNameIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var UserWithNullName = _validUser;
        UserWithNullName.FirstName = null;

        var UserWithEmptyName = _validUser;
        UserWithEmptyName.FirstName = string.Empty;

        var UserWithLongName = _validUser;
        UserWithLongName.FirstName = new string('a', UserEntityConstraints.MaxNameLength + 1);

        // Act
        var resultOfNullName = _validator.TestValidate(UserWithNullName);
        var resultOfEmptyName = _validator.TestValidate(UserWithEmptyName);
        var resultOfLongName = _validator.TestValidate(UserWithLongName);

        // Assert
        resultOfNullName.ShouldHaveValidationErrorFor(x => x.FirstName);
        resultOfEmptyName.ShouldHaveValidationErrorFor(x => x.FirstName);
        resultOfLongName.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void ValidateUser_WhenLastNameIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var UserWithNullName = _validUser;
        UserWithNullName.LastName = null;

        var UserWithEmptyName = _validUser;
        UserWithEmptyName.LastName = string.Empty;

        var UserWithLongName = _validUser;
        UserWithLongName.LastName = new string('a', UserEntityConstraints.MaxNameLength + 1);

        // Act
        var resultOfNullName = _validator.TestValidate(UserWithNullName);
        var resultOfEmptyName = _validator.TestValidate(UserWithEmptyName);
        var resultOfLongName = _validator.TestValidate(UserWithLongName);

        // Assert
        resultOfNullName.ShouldHaveValidationErrorFor(x => x.LastName);
        resultOfEmptyName.ShouldHaveValidationErrorFor(x => x.LastName);
        resultOfLongName.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void ValidateUser_WhenFirebaseIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var UserWithNullFirebaseId = _validUser;
        UserWithNullFirebaseId.ProviderId = null;

        var UserWithEmptyFirebaseId = _validUser;
        UserWithEmptyFirebaseId.ProviderId = string.Empty;

        var UserWithLongFirebaseId = _validUser;
        UserWithLongFirebaseId.ProviderId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullFirebaseId = _validator.TestValidate(UserWithNullFirebaseId);
        var resultOfEmptyFirebaseId = _validator.TestValidate(UserWithEmptyFirebaseId);
        var resultOfLongFirebaseId = _validator.TestValidate(UserWithLongFirebaseId);

        // Assert
        resultOfNullFirebaseId.ShouldHaveValidationErrorFor(x => x.ProviderId);
        resultOfEmptyFirebaseId.ShouldHaveValidationErrorFor(x => x.ProviderId);
        resultOfLongFirebaseId.ShouldHaveValidationErrorFor(x => x.ProviderId);
    }

    [Fact]
    public void ValidateUser_WhenPhoneNumberIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var UserWithNullPhoneNumber = _validUser;
        UserWithNullPhoneNumber.PhoneNumber = null;

        var UserWithEmptyPhoneNumber = _validUser;
        UserWithEmptyPhoneNumber.PhoneNumber = string.Empty;

        var UserWithInvalidPhoneNumber = _validUser;
        UserWithInvalidPhoneNumber.PhoneNumber = "invalidphonenumber";

        var UserWithLongPhoneNumber = _validUser;
        UserWithLongPhoneNumber.PhoneNumber = new string('1', UserEntityConstraints.MaxPhoneNumberLength + 1);

        // Act
        var resultOfNullPhoneNumber = _validator.TestValidate(UserWithNullPhoneNumber);
        var resultOfEmptyPhoneNumber = _validator.TestValidate(UserWithEmptyPhoneNumber);
        var resultOfInvalidPhoneNumber = _validator.TestValidate(UserWithInvalidPhoneNumber);
        var resultOfLongPhoneNumber = _validator.TestValidate(UserWithLongPhoneNumber);

        // Assert
        resultOfNullPhoneNumber.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        resultOfEmptyPhoneNumber.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        resultOfInvalidPhoneNumber.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        resultOfLongPhoneNumber.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }
}
