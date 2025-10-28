// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using FluentValidation;

namespace ExpertBridge.Contract.Requests.UpdateProfileSkills;

/// <summary>
///     Validates UpdateProfileSkillsRequest to ensure skill IDs are valid.
/// </summary>
/// <remarks>
///     Validates that Skills collection is provided and each skill ID meets constraints.
/// </remarks>
public class UpdateProfileSkillsRequestValidator : AbstractValidator<UpdateProfileSkillsRequest>
{
    /// <summary>
    ///     Initializes a new instance of the UpdateProfileSkillsRequestValidator with validation rules.
    /// </summary>
    public UpdateProfileSkillsRequestValidator()
    {
        RuleFor(x => x.Skills)
            .NotNull().WithMessage("Skills cannot be null");

        RuleForEach(x => x.Skills)
            .NotNull().WithMessage("Skill ID cannot be null")
            .NotEmpty().WithMessage("Skill ID cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"Skill ID cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
    }
}
