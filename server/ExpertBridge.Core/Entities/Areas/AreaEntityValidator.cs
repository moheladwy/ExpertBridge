

using FluentValidation;

namespace ExpertBridge.Core.Entities.Areas;

public class AreaEntityValidator : AbstractValidator<Area>
{
    public AreaEntityValidator()
    {
        RuleFor(area => area.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id is longer than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(area => area.ProfileId)
            .NotNull().WithMessage("ProfileId is required")
            .NotEmpty().WithMessage("ProfileId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"ProfileId is longer than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(area => area.Governorate)
            .NotNull().WithMessage("Governorate is required")
            .IsInEnum().WithMessage("Governorate is not valid");

        RuleFor(area => area.Region)
            .NotNull().WithMessage("Region is required")
            .NotEmpty().WithMessage("Region is required")
            .MaximumLength(AreaEntityConstraints.MaxRegionLength);
    }
}
