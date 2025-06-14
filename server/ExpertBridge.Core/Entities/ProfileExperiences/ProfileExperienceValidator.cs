// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Entities.ProfileExperiences;

public class ProfileExperienceValidator : AbstractValidator<ProfileExperience>
{
    public ProfileExperienceValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("ID is required.")
            .NotEmpty().WithMessage("ID is required.")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"ID must not exceed {GlobalEntitiesConstraints.MaxIdLength} characters.");

        RuleFor(x => x.ProfileId)
            .NotNull().WithMessage("Profile ID is required.")
            .NotEmpty().WithMessage("Profile ID is required.")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Profile ID must not exceed {GlobalEntitiesConstraints.MaxIdLength} characters.");

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
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Start date must be less than or equal to today.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be greater than or equal to start date.")
            .When(x => x.EndDate.HasValue);

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
