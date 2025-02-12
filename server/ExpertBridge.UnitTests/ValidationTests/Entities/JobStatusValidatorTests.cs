using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.Job.JobStatus;
using FluentValidation.TestHelper;

namespace ExpertBridge.UnitTests.ValidationTests.Entities;

public class JobStatusValidatorTests
{
    private readonly JobStatusEntityValidator _jobStatusEntityValidator = new();
    private readonly JobStatus _validJobStatus = new()
    {
        Id = Guid.NewGuid().ToString(),
        Status = JobStatusEnum.Completed
    };

    [Fact]
    public void ValidateJobStatus_WhenJobStatusIsValid_ShouldReturnTrue()
    {
        // No need to arrange anything since the job status is already valid
        // Act
        var result = _jobStatusEntityValidator.TestValidate(_validJobStatus);
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidateJobStatus_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobStatusWithNullId = _validJobStatus;
        jobStatusWithNullId.Id = null;

        var jobStatusWithEmptyId = _validJobStatus;
        jobStatusWithEmptyId.Id = string.Empty;

        var jobStatusWithLongId = _validJobStatus;
        jobStatusWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _jobStatusEntityValidator.TestValidate(jobStatusWithNullId);
        var resultOfEmptyId = _jobStatusEntityValidator.TestValidate(jobStatusWithEmptyId);
        var resultOfLongId = _jobStatusEntityValidator.TestValidate(jobStatusWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidateJobStatus_WhenStatusIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var jobStatusWithInvalidStatus = _validJobStatus;
        jobStatusWithInvalidStatus.Status = (JobStatusEnum)1000;

        // Act
        var resultOfInvalidStatus = _jobStatusEntityValidator.TestValidate(jobStatusWithInvalidStatus);

        // Assert
        resultOfInvalidStatus.ShouldHaveValidationErrorFor(x => x.Status);
    }
}
