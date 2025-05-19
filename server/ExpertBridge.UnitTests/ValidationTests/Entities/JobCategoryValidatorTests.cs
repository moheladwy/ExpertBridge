namespace ExpertBridge.UnitTests.ValidationTests.Entities;

public class JobCategoryValidatorTests
{
    private readonly JobCategoryEntityValidator _jobCategoryEntityValidator = new();

    private readonly JobCategory _validJobCategory = new()
    {
        Id = Guid.NewGuid().ToString(), Name = "Job Category Name", Description = "Job Category Description"
    };

    [Fact]
    public void ValidateJobCategory_WhenJobCategoryIsValid_ShouldReturnTrue()
    {
        // No need to arrange anything since the job category is already valid
        // Act
        var result = _jobCategoryEntityValidator.TestValidate(_validJobCategory);
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidateJobCategory_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobCategoryWithNullId = _validJobCategory;
        jobCategoryWithNullId.Id = null;

        var jobCategoryWithEmptyId = _validJobCategory;
        jobCategoryWithEmptyId.Id = string.Empty;

        var jobCategoryWithLongId = _validJobCategory;
        jobCategoryWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _jobCategoryEntityValidator.TestValidate(jobCategoryWithNullId);
        var resultOfEmptyId = _jobCategoryEntityValidator.TestValidate(jobCategoryWithEmptyId);
        var resultOfLongId = _jobCategoryEntityValidator.TestValidate(jobCategoryWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidateJobCategory_WhenNameIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobCategoryWithNullName = _validJobCategory;
        jobCategoryWithNullName.Name = null;

        var jobCategoryWithEmptyName = _validJobCategory;
        jobCategoryWithEmptyName.Name = string.Empty;

        var jobCategoryWithLongName = _validJobCategory;
        jobCategoryWithLongName.Name = new string('a', JobCategoryEntityConstraints.MaxNameLength + 1);

        // Act
        var resultOfNullName = _jobCategoryEntityValidator.TestValidate(jobCategoryWithNullName);
        var resultOfEmptyName = _jobCategoryEntityValidator.TestValidate(jobCategoryWithEmptyName);
        var resultOfLongName = _jobCategoryEntityValidator.TestValidate(jobCategoryWithLongName);

        // Assert
        resultOfNullName.ShouldHaveValidationErrorFor(x => x.Name);
        resultOfEmptyName.ShouldHaveValidationErrorFor(x => x.Name);
        resultOfLongName.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ValidateJobCategory_WhenDescriptionIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobCategoryWithNullDescription = _validJobCategory;
        jobCategoryWithNullDescription.Description = null;

        var jobCategoryWithEmptyDescription = _validJobCategory;
        jobCategoryWithEmptyDescription.Description = string.Empty;

        var jobCategoryWithLongDescription = _validJobCategory;
        jobCategoryWithLongDescription.Description =
            new string('a', JobCategoryEntityConstraints.MaxDescriptionLength + 1);

        // Act
        var resultOfNullDescription = _jobCategoryEntityValidator.TestValidate(jobCategoryWithNullDescription);
        var resultOfEmptyDescription = _jobCategoryEntityValidator.TestValidate(jobCategoryWithEmptyDescription);
        var resultOfLongDescription = _jobCategoryEntityValidator.TestValidate(jobCategoryWithLongDescription);

        // Assert
        resultOfNullDescription.ShouldHaveValidationErrorFor(x => x.Description);
        resultOfEmptyDescription.ShouldHaveValidationErrorFor(x => x.Description);
        resultOfLongDescription.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
