using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.JobPosting;
using FluentValidation.TestHelper;

namespace ExpertBridge.UnitTests.ValidationTests;

public class JobPostingValidatorTests
{
    private readonly JobPostingEntityValidator _jobPostingEntityValidator = new();
    private readonly JobPosting _validJobPosting = new()
    {
        Id = Guid.NewGuid().ToString(),
        AuthorId = Guid.NewGuid().ToString(),
        AreaId = Guid.NewGuid().ToString(),
        CategoryId = Guid.NewGuid().ToString(),
        Title = "Job Posting Title",
        Description = "Job Posting Description",
        Cost = 1000
    };

    [Fact]
    public void ValidateJobPosting_WhenJobPostingIsValid_ShouldReturnTrue()
    {
        // No need to arrange anything since the job posting is already valid
        // Act
        var result = _jobPostingEntityValidator.TestValidate(_validJobPosting);
        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidateJobPosting_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobPostingWithNullId = _validJobPosting;
        jobPostingWithNullId.Id = null;

        var jobPostingWithEmptyId = _validJobPosting;
        jobPostingWithEmptyId.Id = string.Empty;

        var jobPostingWithLongId = _validJobPosting;
        jobPostingWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _jobPostingEntityValidator.TestValidate(jobPostingWithNullId);
        var resultOfEmptyId = _jobPostingEntityValidator.TestValidate(jobPostingWithEmptyId);
        var resultOfLongId = _jobPostingEntityValidator.TestValidate(jobPostingWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidateJobPosting_WhenAuthorIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobPostingWithNullAuthorId = _validJobPosting;
        jobPostingWithNullAuthorId.AuthorId = null;

        var jobPostingWithEmptyAuthorId = _validJobPosting;
        jobPostingWithEmptyAuthorId.AuthorId = string.Empty;

        var jobPostingWithLongAuthorId = _validJobPosting;
        jobPostingWithLongAuthorId.AuthorId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullAuthorId = _jobPostingEntityValidator.TestValidate(jobPostingWithNullAuthorId);
        var resultOfEmptyAuthorId = _jobPostingEntityValidator.TestValidate(jobPostingWithEmptyAuthorId);
        var resultOfLongAuthorId = _jobPostingEntityValidator.TestValidate(jobPostingWithLongAuthorId);

        // Assert
        resultOfNullAuthorId.ShouldHaveValidationErrorFor(x => x.AuthorId);
        resultOfEmptyAuthorId.ShouldHaveValidationErrorFor(x => x.AuthorId);
        resultOfLongAuthorId.ShouldHaveValidationErrorFor(x => x.AuthorId);
    }

    [Fact]
    public void ValidateJobPosting_WhenAreaIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobPostingWithNullAreaId = _validJobPosting;
        jobPostingWithNullAreaId.AreaId = null;

        var jobPostingWithEmptyAreaId = _validJobPosting;
        jobPostingWithEmptyAreaId.AreaId = string.Empty;

        var jobPostingWithLongAreaId = _validJobPosting;
        jobPostingWithLongAreaId.AreaId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullAreaId = _jobPostingEntityValidator.TestValidate(jobPostingWithNullAreaId);
        var resultOfEmptyAreaId = _jobPostingEntityValidator.TestValidate(jobPostingWithEmptyAreaId);
        var resultOfLongAreaId = _jobPostingEntityValidator.TestValidate(jobPostingWithLongAreaId);

        // Assert
        resultOfNullAreaId.ShouldHaveValidationErrorFor(x => x.AreaId);
        resultOfEmptyAreaId.ShouldHaveValidationErrorFor(x => x.AreaId);
        resultOfLongAreaId.ShouldHaveValidationErrorFor(x => x.AreaId);
    }

    [Fact]
    public void ValidateJobPosting_WhenCategoryIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobPostingWithNullCategoryId = _validJobPosting;
        jobPostingWithNullCategoryId.CategoryId = null;

        var jobPostingWithEmptyCategoryId = _validJobPosting;
        jobPostingWithEmptyCategoryId.CategoryId = string.Empty;

        var jobPostingWithLongCategoryId = _validJobPosting;
        jobPostingWithLongCategoryId.CategoryId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullCategoryId = _jobPostingEntityValidator.TestValidate(jobPostingWithNullCategoryId);
        var resultOfEmptyCategoryId = _jobPostingEntityValidator.TestValidate(jobPostingWithEmptyCategoryId);
        var resultOfLongCategoryId = _jobPostingEntityValidator.TestValidate(jobPostingWithLongCategoryId);

        // Assert
        resultOfNullCategoryId.ShouldHaveValidationErrorFor(x => x.CategoryId);
        resultOfEmptyCategoryId.ShouldHaveValidationErrorFor(x => x.CategoryId);
        resultOfLongCategoryId.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void ValidateJobPosting_WhenTitleIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobPostingWithNullTitle = _validJobPosting;
        jobPostingWithNullTitle.Title = null;

        var jobPostingWithEmptyTitle = _validJobPosting;
        jobPostingWithEmptyTitle.Title = string.Empty;

        var jobPostingWithLongTitle = _validJobPosting;
        jobPostingWithLongTitle.Title = new string('a', JobPostingEntityConstraints.MaxTitleLength + 1);

        // Act
        var resultOfNullTitle = _jobPostingEntityValidator.TestValidate(jobPostingWithNullTitle);
        var resultOfEmptyTitle = _jobPostingEntityValidator.TestValidate(jobPostingWithEmptyTitle);
        var resultOfLongTitle = _jobPostingEntityValidator.TestValidate(jobPostingWithLongTitle);

        // Assert
        resultOfNullTitle.ShouldHaveValidationErrorFor(x => x.Title);
        resultOfEmptyTitle.ShouldHaveValidationErrorFor(x => x.Title);
        resultOfLongTitle.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void ValidateJobPosting_WhenDescriptionIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobPostingWithNullDescription = _validJobPosting;
        jobPostingWithNullDescription.Description = null;

        var jobPostingWithEmptyDescription = _validJobPosting;
        jobPostingWithEmptyDescription.Description = string.Empty;

        var jobPostingWithLongDescription = _validJobPosting;
        jobPostingWithLongDescription.Description = new string('a', JobPostingEntityConstraints.MaxDescriptionLength + 1);

        // Act
        var resultOfNullDescription = _jobPostingEntityValidator.TestValidate(jobPostingWithNullDescription);
        var resultOfEmptyDescription = _jobPostingEntityValidator.TestValidate(jobPostingWithEmptyDescription);
        var resultOfLongDescription = _jobPostingEntityValidator.TestValidate(jobPostingWithLongDescription);

        // Assert
        resultOfNullDescription.ShouldHaveValidationErrorFor(x => x.Description);
        resultOfEmptyDescription.ShouldHaveValidationErrorFor(x => x.Description);
        resultOfLongDescription.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void ValidateJobPosting_WhenCostIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobPostingWithNegativeCost = _validJobPosting;
        jobPostingWithNegativeCost.Cost = JobPostingEntityConstraints.MinCost - 1;

        // Act
        var resultOfNegativeCost = _jobPostingEntityValidator.TestValidate(jobPostingWithNegativeCost);

        // Assert
        resultOfNegativeCost.ShouldHaveValidationErrorFor(x => x.Cost);
    }
}
