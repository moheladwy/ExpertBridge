using FluentValidation;

namespace ExpertBridge.Core.Entities.Badge;

public class BadgeEntityValidator : AbstractValidator<Badge>
{
    public BadgeEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Name)
            .NotNull().WithMessage("Name is required")
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(BadgeEntityConstraints.MaxNameLength).WithMessage($"Name must be less than {BadgeEntityConstraints.MaxNameLength} characters");

        RuleFor(x => x.Description)
            .NotNull().WithMessage("Description is required")
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(BadgeEntityConstraints.MaxDescriptionLength).WithMessage($"Description must be less than {BadgeEntityConstraints.MaxDescriptionLength} characters");
    }
}
