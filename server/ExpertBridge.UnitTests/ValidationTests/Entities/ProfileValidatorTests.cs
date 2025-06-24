// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.UnitTests.ValidationTests.Entities;

public class ProfileValidatorTests
{
    private readonly ProfileEntityValidator _profileEntityValidator = new();

    private readonly Profile _validProfile = new()
    {
        Id = Guid.NewGuid()
            .ToString(),
        UserId = Guid.NewGuid()
            .ToString(),
        JobTitle = "Software Engineer",
        Bio = "I am a software engineer",
        ProfilePictureUrl = "https://example.com/profile.jpg",
        Rating = 5,
        RatingCount = 10,
        Email = "software.engineer@gmail.com",
        Username = "software_engineer"
    };

    [Fact]
    public void ValidateProfile_WhenProfileIsValid_ShouldReturnTrue()
    {
        // No need to arrange anything since the profile is already valid
        // Act
        var result = _profileEntityValidator.TestValidate(_validProfile);
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidateProfile_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var profileWithNullId = _validProfile;
        profileWithNullId.Id = null;

        var profileWithEmptyId = _validProfile;
        profileWithEmptyId.Id = string.Empty;

        var profileWithLongId = _validProfile;
        profileWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _profileEntityValidator.TestValidate(profileWithNullId);
        var resultOfEmptyId = _profileEntityValidator.TestValidate(profileWithEmptyId);
        var resultOfLongId = _profileEntityValidator.TestValidate(profileWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidateProfile_WhenUserIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var profileWithNullUserId = _validProfile;
        profileWithNullUserId.UserId = null;

        var profileWithEmptyUserId = _validProfile;
        profileWithEmptyUserId.UserId = string.Empty;

        var profileWithLongUserId = _validProfile;
        profileWithLongUserId.UserId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullUserId = _profileEntityValidator.TestValidate(profileWithNullUserId);
        var resultOfEmptyUserId = _profileEntityValidator.TestValidate(profileWithEmptyUserId);
        var resultOfLongUserId = _profileEntityValidator.TestValidate(profileWithLongUserId);

        // Assert
        resultOfNullUserId.ShouldHaveValidationErrorFor(x => x.UserId);
        resultOfEmptyUserId.ShouldHaveValidationErrorFor(x => x.UserId);
        resultOfLongUserId.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void ValidateProfile_WhenJobTitleIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var profileWithNullJobTitle = _validProfile;
        profileWithNullJobTitle.JobTitle = null;

        var profileWithEmptyJobTitle = _validProfile;
        profileWithEmptyJobTitle.JobTitle = string.Empty;

        var profileWithLongJobTitle = _validProfile;
        profileWithLongJobTitle.JobTitle = new string('a', ProfileEntityConstraints.JobTitleMaxLength + 1);

        // Act
        var resultOfNullJobTitle = _profileEntityValidator.TestValidate(profileWithNullJobTitle);
        var resultOfEmptyJobTitle = _profileEntityValidator.TestValidate(profileWithEmptyJobTitle);
        var resultOfLongJobTitle = _profileEntityValidator.TestValidate(profileWithLongJobTitle);

        // Assert
        resultOfNullJobTitle.ShouldHaveValidationErrorFor(x => x.JobTitle);
        resultOfEmptyJobTitle.ShouldHaveValidationErrorFor(x => x.JobTitle);
        resultOfLongJobTitle.ShouldHaveValidationErrorFor(x => x.JobTitle);
    }

    [Fact]
    public void ValidateProfile_WhenBioIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var profileWithNullBio = _validProfile;
        profileWithNullBio.Bio = null;

        var profileWithEmptyBio = _validProfile;
        profileWithEmptyBio.Bio = string.Empty;

        var profileWithLongBio = _validProfile;
        profileWithLongBio.Bio = new string('a', ProfileEntityConstraints.BioMaxLength + 1);

        // Act
        var resultOfNullBio = _profileEntityValidator.TestValidate(profileWithNullBio);
        var resultOfEmptyBio = _profileEntityValidator.TestValidate(profileWithEmptyBio);
        var resultOfLongBio = _profileEntityValidator.TestValidate(profileWithLongBio);

        // Assert
        resultOfNullBio.ShouldHaveValidationErrorFor(x => x.Bio);
        resultOfEmptyBio.ShouldHaveValidationErrorFor(x => x.Bio);
        resultOfLongBio.ShouldHaveValidationErrorFor(x => x.Bio);
    }

    [Fact]
    public void ValidateProfile_WhenRatingIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var profileWithNegativeRating = _validProfile;
        profileWithNegativeRating.Rating = ProfileEntityConstraints.RatingMinValue - 1;

        var profileWithRatingGreaterThanMaxValue = _validProfile;
        profileWithRatingGreaterThanMaxValue.Rating = ProfileEntityConstraints.RatingMaxValue + 1;

        // Act
        var resultOfNegativeRating = _profileEntityValidator.TestValidate(profileWithNegativeRating);
        var resultOfRatingGreaterThanMaxValue =
            _profileEntityValidator.TestValidate(profileWithRatingGreaterThanMaxValue);

        // Assert
        resultOfNegativeRating.ShouldHaveValidationErrorFor(x => x.Rating);
        resultOfRatingGreaterThanMaxValue.ShouldHaveValidationErrorFor(x => x.Rating);
    }

    [Fact]
    public void ValidateProfile_WhenRatingCountIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var profileWithNegativeRatingCount = _validProfile;
        profileWithNegativeRatingCount.RatingCount = ProfileEntityConstraints.RatingCountMinValue - 1;

        // Act
        var resultOfNegativeRatingCount = _profileEntityValidator.TestValidate(profileWithNegativeRatingCount);

        // Assert
        resultOfNegativeRatingCount.ShouldHaveValidationErrorFor(x => x.RatingCount);
    }

    [Fact]
    public void ValidateProfile_WhenProfilePictureUrlIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var profileWithNullProfilePictureUrl = _validProfile;
        profileWithNullProfilePictureUrl.ProfilePictureUrl = null;

        var profileWithEmptyProfilePictureUrl = _validProfile;
        profileWithEmptyProfilePictureUrl.ProfilePictureUrl = string.Empty;

        var profileWithLongProfilePictureUrl = _validProfile;
        profileWithLongProfilePictureUrl.ProfilePictureUrl =
            new string('a', MediaEntityConstraints.MaxMediaUrlLength + 1);

        // Act
        var resultOfNullProfilePictureUrl = _profileEntityValidator.TestValidate(profileWithNullProfilePictureUrl);
        var resultOfEmptyProfilePictureUrl = _profileEntityValidator.TestValidate(profileWithEmptyProfilePictureUrl);
        var resultOfLongProfilePictureUrl = _profileEntityValidator.TestValidate(profileWithLongProfilePictureUrl);

        // Assert
        resultOfNullProfilePictureUrl.ShouldHaveValidationErrorFor(x => x.ProfilePictureUrl);
        resultOfEmptyProfilePictureUrl.ShouldHaveValidationErrorFor(x => x.ProfilePictureUrl);
        resultOfLongProfilePictureUrl.ShouldHaveValidationErrorFor(x => x.ProfilePictureUrl);
    }
}
