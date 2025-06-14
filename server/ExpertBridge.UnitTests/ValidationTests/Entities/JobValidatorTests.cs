// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

// public class JobValidatorTests
// {
//     private readonly JobEntityValidator _jobEntityValidator = new();

//     private readonly Job _validJob = new()
//     {
//         Id = Guid.NewGuid().ToString(),
//         JobStatusId = Guid.NewGuid().ToString(),
//         WorkerId = Guid.NewGuid().ToString(),
//         AuthorId = Guid.NewGuid().ToString(),
//         JobPostingId = Guid.NewGuid().ToString(),
//         ActualCost = 100,
//         StartedAt = DateTime.UtcNow.AddDays(-2),
//         EndedAt = DateTime.UtcNow.AddMinutes(-10)
//     };

//     [Fact]
//     public void ValidateJob_WhenJobIsValid_ShouldReturnTrue()
//     {
//         // No need to arrange anything since the job is already valid
//         // Act
//         var result = _jobEntityValidator.TestValidate(_validJob);
//         // Assert
//         result.ShouldNotHaveAnyValidationErrors();
//     }

//     [Fact]
//     public void ValidateJob_WhenIdIsInvalid_ShouldReturnFalse()
//     {
//         // Arrange
//         var jobWithNullId = _validJob;
//         jobWithNullId.Id = null;

//         var jobWithEmptyId = _validJob;
//         jobWithEmptyId.Id = string.Empty;

//         var jobWithLongId = _validJob;
//         jobWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

//         // Act
//         var resultOfNullId = _jobEntityValidator.TestValidate(jobWithNullId);
//         var resultOfEmptyId = _jobEntityValidator.TestValidate(jobWithEmptyId);
//         var resultOfLongId = _jobEntityValidator.TestValidate(jobWithLongId);

//         // Assert
//         resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
//         resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
//         resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
//     }

//     [Fact]
//     public void ValidateJob_WhenActualCostIsInvalid_ShouldReturnFalse()
//     {
//         // Arrange
//         var jobWithNegativeActualCost = _validJob;
//         jobWithNegativeActualCost.ActualCost = JobEntityConstraints.MinActualCost - 1;

//         // Act
//         var resultOfNegativeActualCost = _jobEntityValidator.TestValidate(jobWithNegativeActualCost);

//         // Assert
//         resultOfNegativeActualCost.ShouldHaveValidationErrorFor(x => x.ActualCost);
//     }

//     [Fact]
//     public void ValidateJob_WhenStartedAtIsInvalid_ShouldReturnFalse()
//     {
//         // Arrange
//         var jobWithStartedAtGreaterThanEndedAt = _validJob;
//         jobWithStartedAtGreaterThanEndedAt.StartedAt = DateTime.UtcNow.AddDays(1);

//         // Act
//         var resultOfStartedAtGreaterThanEndedAt = _jobEntityValidator.TestValidate(jobWithStartedAtGreaterThanEndedAt);

//         // Assert
//         resultOfStartedAtGreaterThanEndedAt.ShouldHaveValidationErrorFor(x => x.StartedAt);
//     }

//     [Fact]
//     public void ValidateJob_WhenEndedAtIsInvalid_ShouldReturnFalse()
//     {
//         // Arrange
//         var jobWithEndedAtLessThanStartedAt = _validJob;
//         jobWithEndedAtLessThanStartedAt.EndedAt = _validJob.StartedAt.AddDays(-1);

//         var jobWithEndedAtGreaterThanCurrentDate = _validJob;
//         jobWithEndedAtGreaterThanCurrentDate.EndedAt = DateTime.UtcNow.AddDays(1);

//         // Act
//         var resultOfEndedAtLessThanStartedAt = _jobEntityValidator.TestValidate(jobWithEndedAtLessThanStartedAt);
//         var resultOfEndedAtGreaterThanCurrentDate =
//             _jobEntityValidator.TestValidate(jobWithEndedAtGreaterThanCurrentDate);

//         // Assert
//         resultOfEndedAtLessThanStartedAt.ShouldHaveValidationErrorFor(x => x.EndedAt);
//         resultOfEndedAtGreaterThanCurrentDate.ShouldHaveValidationErrorFor(x => x.EndedAt);
//     }

//     [Fact]
//     public void ValidateJob_WhenJobStatusIdIsInvalid_ShouldReturnFalse()
//     {
//         // Arrange
//         var jobWithNullJobStatusId = _validJob;
//         jobWithNullJobStatusId.JobStatusId = null;

//         var jobWithEmptyJobStatusId = _validJob;
//         jobWithEmptyJobStatusId.JobStatusId = string.Empty;

//         var jobWithLongJobStatusId = _validJob;
//         jobWithLongJobStatusId.JobStatusId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

//         // Act
//         var resultOfNullJobStatusId = _jobEntityValidator.TestValidate(jobWithNullJobStatusId);
//         var resultOfEmptyJobStatusId = _jobEntityValidator.TestValidate(jobWithEmptyJobStatusId);
//         var resultOfLongJobStatusId = _jobEntityValidator.TestValidate(jobWithLongJobStatusId);

//         // Assert
//         resultOfNullJobStatusId.ShouldHaveValidationErrorFor(x => x.JobStatusId);
//         resultOfEmptyJobStatusId.ShouldHaveValidationErrorFor(x => x.JobStatusId);
//         resultOfLongJobStatusId.ShouldHaveValidationErrorFor(x => x.JobStatusId);
//     }

//     [Fact]
//     public void ValidateJob_WhenWorkerIdIsInvalid_ShouldReturnFalse()
//     {
//         // Arrange
//         var jobWithNullWorkerId = _validJob;
//         jobWithNullWorkerId.WorkerId = null;

//         var jobWithEmptyWorkerId = _validJob;
//         jobWithEmptyWorkerId.WorkerId = string.Empty;

//         var jobWithLongWorkerId = _validJob;
//         jobWithLongWorkerId.WorkerId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

//         // Act
//         var resultOfNullWorkerId = _jobEntityValidator.TestValidate(jobWithNullWorkerId);
//         var resultOfEmptyWorkerId = _jobEntityValidator.TestValidate(jobWithEmptyWorkerId);
//         var resultOfLongWorkerId = _jobEntityValidator.TestValidate(jobWithLongWorkerId);

//         // Assert
//         resultOfNullWorkerId.ShouldHaveValidationErrorFor(x => x.WorkerId);
//         resultOfEmptyWorkerId.ShouldHaveValidationErrorFor(x => x.WorkerId);
//         resultOfLongWorkerId.ShouldHaveValidationErrorFor(x => x.WorkerId);
//     }

//     [Fact]
//     public void ValidateJob_WhenAuthorIdIsInvalid_ShouldReturnFalse()
//     {
//         // Arrange
//         var jobWithNullAuthorId = _validJob;
//         jobWithNullAuthorId.AuthorId = null;

//         var jobWithEmptyAuthorId = _validJob;
//         jobWithEmptyAuthorId.AuthorId = string.Empty;

//         var jobWithLongAuthorId = _validJob;
//         jobWithLongAuthorId.AuthorId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

//         // Act
//         var resultOfNullAuthorId = _jobEntityValidator.TestValidate(jobWithNullAuthorId);
//         var resultOfEmptyAuthorId = _jobEntityValidator.TestValidate(jobWithEmptyAuthorId);
//         var resultOfLongAuthorId = _jobEntityValidator.TestValidate(jobWithLongAuthorId);

//         // Assert
//         resultOfNullAuthorId.ShouldHaveValidationErrorFor(x => x.AuthorId);
//         resultOfEmptyAuthorId.ShouldHaveValidationErrorFor(x => x.AuthorId);
//         resultOfLongAuthorId.ShouldHaveValidationErrorFor(x => x.AuthorId);
//     }

//     [Fact]
//     public void ValidateJob_WhenJobPostingIdIsInvalid_ShouldReturnFalse()
//     {
//         // Arrange
//         var jobWithNullJobPostingId = _validJob;
//         jobWithNullJobPostingId.JobPostingId = null;

//         var jobWithEmptyJobPostingId = _validJob;
//         jobWithEmptyJobPostingId.JobPostingId = string.Empty;

//         var jobWithLongJobPostingId = _validJob;
//         jobWithLongJobPostingId.JobPostingId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

//         // Act
//         var resultOfNullJobPostingId = _jobEntityValidator.TestValidate(jobWithNullJobPostingId);
//         var resultOfEmptyJobPostingId = _jobEntityValidator.TestValidate(jobWithEmptyJobPostingId);
//         var resultOfLongJobPostingId = _jobEntityValidator.TestValidate(jobWithLongJobPostingId);

//         // Assert
//         resultOfNullJobPostingId.ShouldHaveValidationErrorFor(x => x.JobPostingId);
//         resultOfEmptyJobPostingId.ShouldHaveValidationErrorFor(x => x.JobPostingId);
//         resultOfLongJobPostingId.ShouldHaveValidationErrorFor(x => x.JobPostingId);
//     }
// }
