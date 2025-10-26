// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Users;
using FluentValidation;

namespace ExpertBridge.Core.Requests.UpdateProfileRequest;

/// <summary>
/// Validates UpdateProfileRequest to ensure profile update data meets requirements.
/// </summary>
/// <remarks>
/// Validates optional fields including JobTitle, Bio, FirstName, LastName, Username, PhoneNumber, and Skills.
/// All validations use .When() to only validate when fields are provided (conditional validation).
/// Username includes regex validation for allowed characters. PhoneNumber uses international format validation.
/// Skills collection ensures each skill ID meets length constraints.
/// </remarks>
public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    /// <summary>
    /// Initializes a new instance of the ValidateUpdateProfileRequest with conditional validation rules.
    /// </summary>
    public UpdateProfileRequestValidator()
    {
        When(x => x.JobTitle != null, () =>
        {
            RuleFor(x => x.JobTitle)
                .NotEmpty().WithMessage("Job title cannot be empty when provided")
                .MaximumLength(ProfileEntityConstraints.JobTitleMaxLength)
                .WithMessage($"Job title cannot be longer than {ProfileEntityConstraints.JobTitleMaxLength} characters");
        });

        When(x => x.Bio != null, () =>
        {
            RuleFor(x => x.Bio)
                .NotEmpty().WithMessage("Bio cannot be empty when provided")
                .MaximumLength(ProfileEntityConstraints.BioMaxLength)
                .WithMessage($"Bio cannot be longer than {ProfileEntityConstraints.BioMaxLength} characters");
        });

        When(x => x.FirstName != null, () =>
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name cannot be empty when provided")
                .MaximumLength(UserEntityConstraints.MaxNameLength)
                .WithMessage($"First name cannot be longer than {UserEntityConstraints.MaxNameLength} characters");
        });

        When(x => x.LastName != null, () =>
        {
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name cannot be empty when provided")
                .MaximumLength(UserEntityConstraints.MaxNameLength)
                .WithMessage($"Last name cannot be longer than {UserEntityConstraints.MaxNameLength} characters");
        });

        When(x => x.Username != null, () =>
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username cannot be empty when provided")
                .MaximumLength(UserEntityConstraints.MaxUsernameLength)
                .WithMessage($"Username cannot be longer than {UserEntityConstraints.MaxUsernameLength} characters")
                .Matches(@"^[a-zA-Z0-9_.-]+$")
                .WithMessage("Username can only contain letters, numbers, periods, hyphens, and underscores");
        });

        When(x => x.PhoneNumber != null, () =>
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number cannot be empty when provided")
                .MaximumLength(UserEntityConstraints.MaxPhoneNumberLength)
                .WithMessage($"Phone number cannot be longer than {UserEntityConstraints.MaxPhoneNumberLength} characters")
                .Matches(@"^\+?[0-9]\d{9,17}$")
                .WithMessage("Phone number must be in a valid international format");
        });

        When(x => x.Skills != null && x.Skills.Count > 0, () =>
        {
            RuleFor(x => x.Skills)
                .NotNull().WithMessage("Skills cannot be null");

            RuleForEach(x => x.Skills)
                .NotNull().WithMessage("Skill ID cannot be null")
                .NotEmpty().WithMessage("Skill ID cannot be empty")
                .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
                .WithMessage($"Each skill ID cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
        });
    }
}
