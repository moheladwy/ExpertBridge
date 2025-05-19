namespace ExpertBridge.UnitTests.ValidationTests.Entities;

public class ProfileExperienceValidatorTests
{
    private readonly ProfileExperienceValidator _profileExperienceEntityValidator = new();

    private readonly ProfileExperience _validProfileExperience = new()
    {
        Id = Guid.NewGuid().ToString(),
        ProfileId = Guid.NewGuid().ToString(),
        Description = "Test Description",
        Title = "Software Engineer",
        Company = "Test Company",
        Location = "Test Location",
        StartDate = DateTime.UtcNow.AddYears(-1),
        EndDate = DateTime.UtcNow.AddYears(1)
    };

    [Fact]
    public void ValidateProfileExperience_WhenProfileExperienceIsValid_ShouldReturnTrue()
    {
        // No need to arrange anything since the profile experience is already valid
        // Act
        var result = _profileExperienceEntityValidator.TestValidate(_validProfileExperience);
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidateProfileExperience_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var profileExperienceWithNullId = _validProfileExperience;
        profileExperienceWithNullId.Id = null;

        var profileExperienceWithEmptyId = _validProfileExperience;
        profileExperienceWithEmptyId.Id = string.Empty;

        var profileExperienceWithLongId = _validProfileExperience;
        profileExperienceWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _profileExperienceEntityValidator.TestValidate(profileExperienceWithNullId);
        var resultOfEmptyId = _profileExperienceEntityValidator.TestValidate(profileExperienceWithEmptyId);
        var resultOfLongId = _profileExperienceEntityValidator.TestValidate(profileExperienceWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidateProfileExperience_WhenProfileIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var profileExperienceWithNullProfileId = _validProfileExperience;
        profileExperienceWithNullProfileId.ProfileId = null;

        var profileExperienceWithEmptyProfileId = _validProfileExperience;
        profileExperienceWithEmptyProfileId.ProfileId = string.Empty;

        var profileExperienceWithLongProfileId = _validProfileExperience;
        profileExperienceWithLongProfileId.ProfileId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullProfileId = _profileExperienceEntityValidator.TestValidate(profileExperienceWithNullProfileId);
        var resultOfEmptyProfileId =
            _profileExperienceEntityValidator.TestValidate(profileExperienceWithEmptyProfileId);
        var resultOfLongProfileId = _profileExperienceEntityValidator.TestValidate(profileExperienceWithLongProfileId);

        // Assert
        resultOfNullProfileId.ShouldHaveValidationErrorFor(x => x.ProfileId);
        resultOfEmptyProfileId.ShouldHaveValidationErrorFor(x => x.ProfileId);
        resultOfLongProfileId.ShouldHaveValidationErrorFor(x => x.ProfileId);
    }

    [Fact]
    public void ValidateProfileExperience_WhenTitleIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var profileExperienceWithNullTitle = _validProfileExperience;
        profileExperienceWithNullTitle.Title = null;

        var profileExperienceWithEmptyTitle = _validProfileExperience;
        profileExperienceWithEmptyTitle.Title = string.Empty;

        var profileExperienceWithLongTitle = _validProfileExperience;
        profileExperienceWithLongTitle.Title = new string('a', ProfileExperienceConstraints.MaxTitleLength + 1);

        // Act
        var resultOfNullTitle = _profileExperienceEntityValidator.TestValidate(profileExperienceWithNullTitle);
        var resultOfEmptyTitle = _profileExperienceEntityValidator.TestValidate(profileExperienceWithEmptyTitle);
        var resultOfLongTitle = _profileExperienceEntityValidator.TestValidate(profileExperienceWithLongTitle);

        // Assert
        resultOfNullTitle.ShouldHaveValidationErrorFor(x => x.Title);
        resultOfEmptyTitle.ShouldHaveValidationErrorFor(x => x.Title);
        resultOfLongTitle.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void ValidateProfileExperience_WhenCompanyIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var profileExperienceWithNullCompany = _validProfileExperience;
        profileExperienceWithNullCompany.Company = null;

        var profileExperienceWithEmptyCompany = _validProfileExperience;
        profileExperienceWithEmptyCompany.Company = string.Empty;

        var profileExperienceWithLongCompany = _validProfileExperience;
        profileExperienceWithLongCompany.Company = new string('a', ProfileExperienceConstraints.MaxCompanyLength + 1);

        // Act
        var resultOfNullCompany = _profileExperienceEntityValidator.TestValidate(profileExperienceWithNullCompany);
        var resultOfEmptyCompany = _profileExperienceEntityValidator.TestValidate(profileExperienceWithEmptyCompany);
        var resultOfLongCompany = _profileExperienceEntityValidator.TestValidate(profileExperienceWithLongCompany);

        // Assert
        resultOfNullCompany.ShouldHaveValidationErrorFor(x => x.Company);
        resultOfEmptyCompany.ShouldHaveValidationErrorFor(x => x.Company);
        resultOfLongCompany.ShouldHaveValidationErrorFor(x => x.Company);
    }

    [Fact]
    public void ValidateProfileExperience_WhenStartDateIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var profileExperienceWithInvalidStartDate = _validProfileExperience;
        profileExperienceWithInvalidStartDate.StartDate = DateTime.UtcNow.AddYears(1);

        // Act
        var resultOfInvalidStartDate =
            _profileExperienceEntityValidator.TestValidate(profileExperienceWithInvalidStartDate);

        // Assert
        resultOfInvalidStartDate.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void ValidateProfileExperience_WhenEndDateIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var profileExperienceWithInvalidEndDate = _validProfileExperience;
        profileExperienceWithInvalidEndDate.EndDate = _validProfileExperience.StartDate.AddYears(-1);

        // Act
        var resultOfInvalidEndDate =
            _profileExperienceEntityValidator.TestValidate(profileExperienceWithInvalidEndDate);

        // Assert
        resultOfInvalidEndDate.ShouldHaveValidationErrorFor(x => x.EndDate);
    }
}
