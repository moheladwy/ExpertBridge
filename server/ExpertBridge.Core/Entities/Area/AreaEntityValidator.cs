// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Entities.Area;

public class AreaEntityValidator : AbstractValidator<Area>
{
    public AreaEntityValidator()
    {
        RuleFor(area => area.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage("Id is too long");

        RuleFor(area => area.ProfileId)
            .NotNull().WithMessage("ProfileId is required")
            .NotEmpty().WithMessage("ProfileId is required");

        RuleFor(area => area.Governorate)
            .NotNull().WithMessage("Governorate is required")
            .IsInEnum().WithMessage("Governorate is not valid");

        RuleFor(area => area.Region)
            .NotNull().WithMessage("Region is required")
            .NotEmpty().WithMessage("Region is required")
            .MaximumLength(AreaEntityConstraints.MaxRegionLength);
    }
}
