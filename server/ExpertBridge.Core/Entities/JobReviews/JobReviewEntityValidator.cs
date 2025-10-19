// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Entities.JobReviews;

/// <summary>
/// Provides validation rules for the <see cref="JobReview"/> entity.
/// </summary>
public class JobReviewEntityValidator : AbstractValidator<JobReview>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JobReviewEntityValidator"/> class and defines validation rules.
    /// </summary>
    public JobReviewEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Rating)
            .NotNull().WithMessage("Rating is required")
            .NotEmpty().WithMessage("Rating is required")
            .InclusiveBetween(
                JobReviewEntityConstraints.MinRating,
                JobReviewEntityConstraints.MaxRating)
            .WithMessage("Rating must be between 0 and 5");

        RuleFor(x => x.Content)
            .NotNull().WithMessage("Content is required")
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(JobReviewEntityConstraints.MaxReviewLength)
            .WithMessage($"Content must be less than {JobReviewEntityConstraints.MaxReviewLength} characters");

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("CreatedAt is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("CreatedAt must be less than or equal to the current date")
            .When(x => x.CreatedAt != DateTime.MinValue)
            .LessThan(x => x.LastModified).WithMessage("CreatedAt must be less than LastModified")
            .When(x => x.LastModified.HasValue);

        RuleFor(x => x.LastModified)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("LastModified must be less than or equal to the current date")
            .When(x => x.LastModified.HasValue)
            .GreaterThan(x => x.CreatedAt).WithMessage("LastModified must be greater than or equal to CreatedAt")
            .When(x => x.CreatedAt != DateTime.MaxValue);

        RuleFor(x => x.IsDeleted)
            .NotNull().WithMessage("IsDeleted is required");

        RuleFor(x => x.WorkerId)
            .NotNull().WithMessage("WorkerId is required")
            .NotEmpty().WithMessage("WorkerId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"WorkerId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.CustomerId)
            .NotNull().WithMessage("CustomerId is required")
            .NotEmpty().WithMessage("CustomerId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"CustomerId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.JobId)
            .NotNull().WithMessage("JobId is required")
            .NotEmpty().WithMessage("JobId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"JobId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");
    }
}
