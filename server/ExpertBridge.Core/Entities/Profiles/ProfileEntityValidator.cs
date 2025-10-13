// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Media;
using FluentValidation;

namespace ExpertBridge.Core.Entities.Profiles;

public class ProfileEntityValidator : AbstractValidator<Profile>
{
    public ProfileEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required.")
            .NotEmpty().WithMessage("Id is required.")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters.");

        RuleFor(x => x.UserId)
            .NotNull().WithMessage("User Id is required.")
            .NotEmpty().WithMessage("User Id is required.")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"User Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters.");

        RuleFor(x => x.JobTitle)
            .NotEmpty().WithMessage("Job title is required.")
            .NotEmpty().WithMessage("Job title is required.")
            .MaximumLength(ProfileEntityConstraints.JobTitleMaxLength);

        RuleFor(x => x.Bio)
            .NotEmpty().WithMessage("Bio is required.")
            .NotEmpty().WithMessage("Bio is required.")
            .MaximumLength(ProfileEntityConstraints.BioMaxLength);

        RuleFor(x => x.ProfilePictureUrl)
            .NotEmpty().WithMessage("Profile picture URL is required.")
            .NotEmpty().WithMessage("Profile picture URL is required.")
            .MaximumLength(MediaEntityConstraints.MaxMediaUrlLength)
            .WithMessage(
                $"Profile picture URL must be less than {MediaEntityConstraints.MaxMediaUrlLength} characters.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(ProfileEntityConstraints.RatingMinValue, ProfileEntityConstraints.RatingMaxValue)
            .WithMessage(
                $"Rating must be between {ProfileEntityConstraints.RatingMinValue} and {ProfileEntityConstraints.RatingMaxValue}.");

        RuleFor(x => x.RatingCount)
            .InclusiveBetween(ProfileEntityConstraints.RatingCountMinValue,
                ProfileEntityConstraints.RatingCountMaxValue)
            .WithMessage(
                $"Rating count must be between {ProfileEntityConstraints.RatingCountMinValue} and {ProfileEntityConstraints.RatingCountMaxValue}.");
    }
}
