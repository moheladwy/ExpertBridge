using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.Skills;
using FluentValidation.TestHelper;

namespace ExpertBridge.UnitTests.ValidationTests.Entities;

public class SkillValidatorTests
{
    private readonly SkillEntityValidator _skillEntityValidator = new();

    private readonly Skill _validSkill = new()
    {
        Id = Guid.NewGuid().ToString(), Name = "Test Skill", Description = "Test Description"
    };

    [Fact]
    public void ValidateSkill_WhenSkillIsValid_ShouldReturnTrue()
    {
        // No need to arrange anything since the skill is already valid
        // Act
        var result = _skillEntityValidator.TestValidate(_validSkill);
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidateSkill_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var skillWithNullId = _validSkill;
        skillWithNullId.Id = null;

        var skillWithEmptyId = _validSkill;
        skillWithEmptyId.Id = string.Empty;

        var skillWithLongId = _validSkill;
        skillWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _skillEntityValidator.TestValidate(skillWithNullId);
        var resultOfEmptyId = _skillEntityValidator.TestValidate(skillWithEmptyId);
        var resultOfLongId = _skillEntityValidator.TestValidate(skillWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidateSkill_WhenNameIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var skillWithNullName = _validSkill;
        skillWithNullName.Name = null;

        var skillWithEmptyName = _validSkill;
        skillWithEmptyName.Name = string.Empty;

        var skillWithLongName = _validSkill;
        skillWithLongName.Name = new string('a', SkillEntityConstraints.MaxNameLength + 1);

        // Act
        var resultOfNullName = _skillEntityValidator.TestValidate(skillWithNullName);
        var resultOfEmptyName = _skillEntityValidator.TestValidate(skillWithEmptyName);
        var resultOfLongName = _skillEntityValidator.TestValidate(skillWithLongName);

        // Assert
        resultOfNullName.ShouldHaveValidationErrorFor(x => x.Name);
        resultOfEmptyName.ShouldHaveValidationErrorFor(x => x.Name);
        resultOfLongName.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ValidateSkill_WhenDescriptionIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var skillWithNullDescription = _validSkill;
        skillWithNullDescription.Description = null;

        var skillWithEmptyDescription = _validSkill;
        skillWithEmptyDescription.Description = string.Empty;

        var skillWithLongDescription = _validSkill;
        skillWithLongDescription.Description = new string('a', SkillEntityConstraints.MaxDescriptionLength + 1);

        // Act
        var resultOfNullDescription = _skillEntityValidator.TestValidate(skillWithNullDescription);
        var resultOfEmptyDescription = _skillEntityValidator.TestValidate(skillWithEmptyDescription);
        var resultOfLongDescription = _skillEntityValidator.TestValidate(skillWithLongDescription);

        // Assert
        resultOfNullDescription.ShouldHaveValidationErrorFor(x => x.Description);
        resultOfEmptyDescription.ShouldHaveValidationErrorFor(x => x.Description);
        resultOfLongDescription.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
