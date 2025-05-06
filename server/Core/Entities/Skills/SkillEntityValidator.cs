

using FluentValidation;

namespace Core.Entities.Skills;

public class SkillEntityValidator : AbstractValidator<Skill>
{
    public SkillEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Name)
            .NotNull().WithMessage("Name is required")
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(SkillEntityConstraints.MaxNameLength).WithMessage($"Name must be less than {SkillEntityConstraints.MaxNameLength} characters");

        RuleFor(x => x.Description)
            .NotNull().WithMessage("Description is required")
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(SkillEntityConstraints.MaxDescriptionLength).WithMessage($"Description must be less than {SkillEntityConstraints.MaxDescriptionLength} characters");
    }
}
