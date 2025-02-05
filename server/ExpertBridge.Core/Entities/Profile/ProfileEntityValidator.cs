using FluentValidation;

namespace ExpertBridge.Core.Entities.Profile;

public class ProfileEntityValidator : AbstractValidator<Profile>
{
    public ProfileEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required.")
            .NotEmpty().WithMessage("Id is required.")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters.");

        RuleFor(x => x.UserId)
            .NotNull().WithMessage("User Id is required.")
            .NotEmpty().WithMessage("User Id is required.")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"User Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters.");

        RuleFor(x => x.JobTitle)
            .NotEmpty().WithMessage("Job title is required.")
            .NotEmpty().WithMessage("Job title is required.")
            .MaximumLength(ProfileEntityConstraints.JobTitleMaxLength);

        RuleFor(x => x.Bio)
            .NotEmpty().WithMessage("Bio is required.")
            .NotEmpty().WithMessage("Bio is required.")
            .MaximumLength(ProfileEntityConstraints.BioMaxLength);

        RuleFor(x => x.Rating)
            .InclusiveBetween(ProfileEntityConstraints.RatingMinValue, ProfileEntityConstraints.RatingMaxValue);
    }
}
