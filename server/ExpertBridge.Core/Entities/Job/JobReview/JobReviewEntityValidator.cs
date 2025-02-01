using System.Data;
using ExpertBridge.Core.Entities.JobCategory;
using FluentValidation;

namespace ExpertBridge.Core.Entities.Job.JobReview;

public class JobReviewEntityValidator : AbstractValidator<JobReview>
{
    public JobReviewEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required");

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
            .NotNull().WithMessage("CreatedAt is required");

        RuleFor(x => x.LastModified)
            .NotEqual(x => x.CreatedAt)
            .When(x => x.LastModified.HasValue)
            .WithMessage("LastModified must be different from CreatedAt");
            
        RuleFor(x => x.IsDeleted)
            .NotNull().WithMessage("IsDeleted is required");
    }
}
