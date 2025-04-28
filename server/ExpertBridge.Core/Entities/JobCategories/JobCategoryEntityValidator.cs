

using FluentValidation;

namespace ExpertBridge.Core.Entities.JobCategories;

public class JobCategoryEntityValidator : AbstractValidator<JobCategory>
{
    public JobCategoryEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Name)
            .NotNull().WithMessage("Name is required")
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(JobCategoryEntityConstraints.MaxNameLength).WithMessage($"Name must be less than {JobCategoryEntityConstraints.MaxNameLength} characters");

        RuleFor(x => x.Description)
            .NotNull().WithMessage("Description is required")
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(JobCategoryEntityConstraints.MaxDescriptionLength).WithMessage($"Description must be less than {JobCategoryEntityConstraints.MaxDescriptionLength} characters");
    }
}
