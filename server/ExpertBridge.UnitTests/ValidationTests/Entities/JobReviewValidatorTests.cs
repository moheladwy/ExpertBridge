using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.JobReviews;
using FluentValidation.TestHelper;

namespace ExpertBridge.UnitTests.ValidationTests.Entities;

public class JobReviewValidatorTests
{
    private readonly JobReviewEntityValidator _jobReviewEntityValidator = new();

    private readonly JobReview _validJobReview = new()
    {
        Id = Guid.NewGuid().ToString(),
        WorkerId = Guid.NewGuid().ToString(),
        CustomerId = Guid.NewGuid().ToString(),
        JobId = Guid.NewGuid().ToString(),
        Content = "Job Review Content",
        Rating = 5,
        CreatedAt = DateTime.UtcNow.AddMinutes(-2),
        LastModified = DateTime.UtcNow.AddMinutes(-1),
        IsDeleted = false
    };

    [Fact]
    public void ValidateJobReview_WhenJobReviewIsValid_ShouldReturnTrue()
    {
        // No need to arrange anything since the job review is already valid
        // Act
        var result = _jobReviewEntityValidator.TestValidate(_validJobReview);
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidateJobReview_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobReviewWithNullId = _validJobReview;
        jobReviewWithNullId.Id = null;

        var jobReviewWithEmptyId = _validJobReview;
        jobReviewWithEmptyId.Id = string.Empty;

        var jobReviewWithLongId = _validJobReview;
        jobReviewWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _jobReviewEntityValidator.TestValidate(jobReviewWithNullId);
        var resultOfEmptyId = _jobReviewEntityValidator.TestValidate(jobReviewWithEmptyId);
        var resultOfLongId = _jobReviewEntityValidator.TestValidate(jobReviewWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidateJobReview_WhenRatingIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobReviewWithNegativeRating = _validJobReview;
        jobReviewWithNegativeRating.Rating = JobReviewEntityConstraints.MinRating - 1;

        var jobReviewWithRatingGreaterThanFive = _validJobReview;
        jobReviewWithRatingGreaterThanFive.Rating = JobReviewEntityConstraints.MaxRating + 1;

        // Act
        var resultOfNegativeRating = _jobReviewEntityValidator.TestValidate(jobReviewWithNegativeRating);
        var resultOfRatingGreaterThanFive = _jobReviewEntityValidator.TestValidate(jobReviewWithRatingGreaterThanFive);

        // Assert
        resultOfNegativeRating.ShouldHaveValidationErrorFor(x => x.Rating);
        resultOfRatingGreaterThanFive.ShouldHaveValidationErrorFor(x => x.Rating);
    }

    [Fact]
    public void ValidateJobReview_WhenContentIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobReviewWithNullContent = _validJobReview;
        jobReviewWithNullContent.Content = null;

        var jobReviewWithEmptyContent = _validJobReview;
        jobReviewWithEmptyContent.Content = string.Empty;

        var jobReviewWithLongContent = _validJobReview;
        jobReviewWithLongContent.Content = new string('a', JobReviewEntityConstraints.MaxReviewLength + 1);

        // Act
        var resultOfNullContent = _jobReviewEntityValidator.TestValidate(jobReviewWithNullContent);
        var resultOfEmptyContent = _jobReviewEntityValidator.TestValidate(jobReviewWithEmptyContent);
        var resultOfLongContent = _jobReviewEntityValidator.TestValidate(jobReviewWithLongContent);

        // Assert
        resultOfNullContent.ShouldHaveValidationErrorFor(x => x.Content);
        resultOfEmptyContent.ShouldHaveValidationErrorFor(x => x.Content);
        resultOfLongContent.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void ValidateJobReview_WhenCreatedAtIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobReviewWithCreatedAtGreaterThanNow = _validJobReview;
        jobReviewWithCreatedAtGreaterThanNow.CreatedAt = DateTime.UtcNow.AddDays(1);

        var jobReviewWithCreatedAtGreaterThanLastModified = _validJobReview;
        jobReviewWithCreatedAtGreaterThanLastModified.CreatedAt = DateTime.UtcNow.AddDays(1);
        jobReviewWithCreatedAtGreaterThanLastModified.LastModified = DateTime.UtcNow;

        // Act
        var resultOfCreatedAtGreaterThanNow =
            _jobReviewEntityValidator.TestValidate(jobReviewWithCreatedAtGreaterThanNow);
        var resultOfCreatedAtGreaterThanLastModified =
            _jobReviewEntityValidator.TestValidate(jobReviewWithCreatedAtGreaterThanLastModified);

        // Assert
        resultOfCreatedAtGreaterThanNow.ShouldHaveValidationErrorFor(x => x.CreatedAt);
        resultOfCreatedAtGreaterThanLastModified.ShouldHaveValidationErrorFor(x => x.CreatedAt);
    }

    [Fact]
    public void ValidateJobReview_WhenLastModifiedIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobReviewWithLastModifiedGreaterThanNow = _validJobReview;
        jobReviewWithLastModifiedGreaterThanNow.LastModified = DateTime.UtcNow.AddDays(1);

        var jobReviewWithLastModifiedLessThanCreatedAt = _validJobReview;
        jobReviewWithLastModifiedLessThanCreatedAt.LastModified = _validJobReview.CreatedAt.Value.AddDays(1);

        // Act
        var resultOfLastModifiedGreaterThanNow =
            _jobReviewEntityValidator.TestValidate(jobReviewWithLastModifiedGreaterThanNow);
        var resultOfLastModifiedLessThanCreatedAt =
            _jobReviewEntityValidator.TestValidate(jobReviewWithLastModifiedLessThanCreatedAt);

        // Assert
        resultOfLastModifiedGreaterThanNow.ShouldHaveValidationErrorFor(x => x.LastModified);
        resultOfLastModifiedLessThanCreatedAt.ShouldHaveValidationErrorFor(x => x.LastModified);
    }

    [Fact]
    public void ValidateJobReview_WhenWorkerIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobReviewWithNullWorkerId = _validJobReview;
        jobReviewWithNullWorkerId.WorkerId = null;

        var jobReviewWithEmptyWorkerId = _validJobReview;
        jobReviewWithEmptyWorkerId.WorkerId = string.Empty;

        var jobReviewWithLongWorkerId = _validJobReview;
        jobReviewWithLongWorkerId.WorkerId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullWorkerId = _jobReviewEntityValidator.TestValidate(jobReviewWithNullWorkerId);
        var resultOfEmptyWorkerId = _jobReviewEntityValidator.TestValidate(jobReviewWithEmptyWorkerId);
        var resultOfLongWorkerId = _jobReviewEntityValidator.TestValidate(jobReviewWithLongWorkerId);

        // Assert
        resultOfNullWorkerId.ShouldHaveValidationErrorFor(x => x.WorkerId);
        resultOfEmptyWorkerId.ShouldHaveValidationErrorFor(x => x.WorkerId);
        resultOfLongWorkerId.ShouldHaveValidationErrorFor(x => x.WorkerId);
    }

    [Fact]
    public void ValidateJobReview_WhenCustomerIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobReviewWithNullCustomerId = _validJobReview;
        jobReviewWithNullCustomerId.CustomerId = null;

        var jobReviewWithEmptyCustomerId = _validJobReview;
        jobReviewWithEmptyCustomerId.CustomerId = string.Empty;

        var jobReviewWithLongCustomerId = _validJobReview;
        jobReviewWithLongCustomerId.CustomerId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullCustomerId = _jobReviewEntityValidator.TestValidate(jobReviewWithNullCustomerId);
        var resultOfEmptyCustomerId = _jobReviewEntityValidator.TestValidate(jobReviewWithEmptyCustomerId);
        var resultOfLongCustomerId = _jobReviewEntityValidator.TestValidate(jobReviewWithLongCustomerId);

        // Assert
        resultOfNullCustomerId.ShouldHaveValidationErrorFor(x => x.CustomerId);
        resultOfEmptyCustomerId.ShouldHaveValidationErrorFor(x => x.CustomerId);
        resultOfLongCustomerId.ShouldHaveValidationErrorFor(x => x.CustomerId);
    }

    [Fact]
    public void ValidateJobReview_WhenJobIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobReviewWithNullJobId = _validJobReview;
        jobReviewWithNullJobId.JobId = null;

        var jobReviewWithEmptyJobId = _validJobReview;
        jobReviewWithEmptyJobId.JobId = string.Empty;

        var jobReviewWithLongJobId = _validJobReview;
        jobReviewWithLongJobId.JobId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullJobId = _jobReviewEntityValidator.TestValidate(jobReviewWithNullJobId);
        var resultOfEmptyJobId = _jobReviewEntityValidator.TestValidate(jobReviewWithEmptyJobId);
        var resultOfLongJobId = _jobReviewEntityValidator.TestValidate(jobReviewWithLongJobId);

        // Assert
        resultOfNullJobId.ShouldHaveValidationErrorFor(x => x.JobId);
        resultOfEmptyJobId.ShouldHaveValidationErrorFor(x => x.JobId);
        resultOfLongJobId.ShouldHaveValidationErrorFor(x => x.JobId);
    }
}
