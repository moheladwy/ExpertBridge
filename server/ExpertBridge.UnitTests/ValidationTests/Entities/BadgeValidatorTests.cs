namespace ExpertBridge.UnitTests.ValidationTests.Entities;

public class BadgeValidatorTests
{
    private readonly BadgeEntityValidator _badgeEntityValidator = new();

    private readonly Badge _validBadge = new()
    {
        Id = Guid.NewGuid().ToString(), Name = "Badge Name", Description = "Badge Description"
    };

    [Fact]
    public void ValidateBadge_WhenBadgeIsValid_ShouldReturnTrue()
    {
        // No need to arrange anything since the badge is already valid
        // Act
        var result = _badgeEntityValidator.TestValidate(_validBadge);
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidateBadge_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var badgeWithNullId = _validBadge;
        badgeWithNullId.Id = null;

        var badgeWithEmptyId = _validBadge;
        badgeWithEmptyId.Id = string.Empty;

        var badgeWithLongId = _validBadge;
        badgeWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _badgeEntityValidator.TestValidate(badgeWithNullId);
        var resultOfEmptyId = _badgeEntityValidator.TestValidate(badgeWithEmptyId);
        var resultOfLongId = _badgeEntityValidator.TestValidate(badgeWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidateBadge_WhenNameIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var badgeWithNullName = _validBadge;
        badgeWithNullName.Name = null;

        var badgeWithEmptyName = _validBadge;
        badgeWithEmptyName.Name = string.Empty;

        var badgeWithLongName = _validBadge;
        badgeWithLongName.Name = new string('a', BadgeEntityConstraints.MaxNameLength + 1);

        // Act
        var resultOfNullName = _badgeEntityValidator.TestValidate(badgeWithNullName);
        var resultOfEmptyName = _badgeEntityValidator.TestValidate(badgeWithEmptyName);
        var resultOfLongName = _badgeEntityValidator.TestValidate(badgeWithLongName);

        // Assert
        resultOfNullName.ShouldHaveValidationErrorFor(x => x.Name);
        resultOfEmptyName.ShouldHaveValidationErrorFor(x => x.Name);
        resultOfLongName.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ValidateBadge_WhenDescriptionIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var badgeWithNullDescription = _validBadge;
        badgeWithNullDescription.Description = null;

        var badgeWithEmptyDescription = _validBadge;
        badgeWithEmptyDescription.Description = string.Empty;

        var badgeWithLongDescription = _validBadge;
        badgeWithLongDescription.Description = new string('a', BadgeEntityConstraints.MaxDescriptionLength + 1);

        // Act
        var resultOfNullDescription = _badgeEntityValidator.TestValidate(badgeWithNullDescription);
        var resultOfEmptyDescription = _badgeEntityValidator.TestValidate(badgeWithEmptyDescription);
        var resultOfLongDescription = _badgeEntityValidator.TestValidate(badgeWithLongDescription);

        // Assert
        resultOfNullDescription.ShouldHaveValidationErrorFor(x => x.Description);
        resultOfEmptyDescription.ShouldHaveValidationErrorFor(x => x.Description);
        resultOfLongDescription.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
