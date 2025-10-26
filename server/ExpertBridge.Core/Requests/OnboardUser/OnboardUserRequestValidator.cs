// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using FluentValidation;

namespace ExpertBridge.Core.Requests.OnboardUser;

/// <summary>
/// Validates OnboardUserRequest to ensure user interests are properly provided.
/// </summary>
/// <remarks>
/// Validates that Tags collection is provided and contains valid tag identifiers
/// for user onboarding process.
/// </remarks>
public class OnboardUserRequestValidator : AbstractValidator<OnboardUserRequest>
{
    /// <summary>
    /// Initializes a new instance of the OnboardUserRequestValidator with validation rules.
    /// </summary>
    public OnboardUserRequestValidator()
    {
        RuleFor(x => x.Tags)
            .NotNull().WithMessage("Tags cannot be null")
            .NotEmpty().WithMessage("At least one tag must be selected");

        RuleForEach(x => x.Tags)
            .NotNull().WithMessage("Tag ID cannot be null")
            .NotEmpty().WithMessage("Tag ID cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"Tag ID cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
    }
}
