// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Skills;

namespace ExpertBridge.Core.Requests.UpdateProfileRequest;

using FluentValidation;

public class ValidateUpdateProfileRequest : AbstractValidator<UpdateProfileRequest>
{
    public ValidateUpdateProfileRequest()
    {
        RuleFor(x => x.JobTitle)
            .MaximumLength(256)
            .WithMessage("Job title must be at most 256 characters long.")
            .When(x => x.JobTitle != null);

        RuleFor(x => x.Bio)
            .MaximumLength(5000)
            .WithMessage("Bio must be at most 5000 characters long.")
            .When(x => x.Bio != null);

        RuleFor(x => x.FirstName)
            .MaximumLength(256)
            .WithMessage("First name must be at most 256 characters long.")
            .When(x => x.FirstName != null);

        RuleFor(x => x.LastName)
            .MaximumLength(256)
            .WithMessage("Last name must be at most 256 characters long.")
            .When(x => x.LastName != null);

        RuleFor(x => x.Username)
            .MaximumLength(256)
            .WithMessage("Username must be at most characters long.")
            .Matches(@"^[a-zA-Z0-9_.-]+$")
            .WithMessage("Username can only contain letters, numbers, and underscores.")
            .When(x => x.Username != null);

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Phone number must be in a valid international format.")
            .When(x => x.PhoneNumber != null);

        RuleFor(x => x.Skills)
            .Must(skills => skills
                .TrueForAll(s => s.Length <= SkillEntityConstraints.MaxNameLength))
            .WithMessage("Each skill must be at most 256 characters long.")
            .When(x => x.Skills.Count > 0);
    }
}
