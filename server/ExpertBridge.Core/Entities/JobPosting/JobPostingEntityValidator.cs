using FluentValidation;

namespace ExpertBridge.Core.Entities.JobPosting;

public class JobPostingEntityValidator : AbstractValidator<JobPosting>
{
    public JobPostingEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Title)
            .NotNull().WithMessage("Title is required")
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(JobPostingEntityConstraints.MaxTitleLength).WithMessage($"Title must be less than {JobPostingEntityConstraints.MaxTitleLength} characters");

        RuleFor(x => x.Description)
            .NotNull().WithMessage("Description is required")
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(JobPostingEntityConstraints.MaxDescriptionLength).WithMessage($"Description must be less than {JobPostingEntityConstraints.MaxDescriptionLength} characters");

        RuleFor(x => x.Cost)
            .NotNull().WithMessage("Cost is required")
            .GreaterThanOrEqualTo(JobPostingEntityConstraints.MinCost).WithMessage($"Cost must be greater than or equal to {JobPostingEntityConstraints.MinCost}");
    }
}
