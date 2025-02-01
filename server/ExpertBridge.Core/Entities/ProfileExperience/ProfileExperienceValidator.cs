using FluentValidation;

namespace ExpertBridge.Core.Entities.ProfileExperience;

public class ProfileExperienceValidator : AbstractValidator<ProfileExperience>
{
    public ProfileExperienceValidator()
    {
        RuleFor(x => x.Title)
            .NotNull().WithMessage("Title is required.")
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(ProfileExperienceConstraints.MaxTitleLength).WithMessage($"Title must not exceed {ProfileExperienceConstraints.MaxTitleLength} characters.");

        RuleFor(x => x.Description)
            .NotNull().WithMessage("Description is required.")
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(ProfileExperienceConstraints.MaxDescriptionLength).WithMessage($"Description must not exceed {ProfileExperienceConstraints.MaxDescriptionLength} characters.");

        RuleFor(x => x.StartDate)
            .NotNull().WithMessage("Start date is required.")
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be greater than or equal to start date.");

        RuleFor(x => x.Company)
            .NotNull().WithMessage("Company is required.")
            .NotEmpty().WithMessage("Company is required.")
            .MaximumLength(ProfileExperienceConstraints.MaxCompanyLength).WithMessage($"Company must not exceed {ProfileExperienceConstraints.MaxCompanyLength} characters.");

        RuleFor(x => x.Location)
            .NotNull().WithMessage("Location is required.")
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(ProfileExperienceConstraints.MaxLocationLength).WithMessage($"Location must not exceed {ProfileExperienceConstraints.MaxLocationLength} characters.");
    }
}
