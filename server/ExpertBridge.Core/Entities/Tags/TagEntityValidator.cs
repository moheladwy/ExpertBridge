// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Entities.Tags;

public class TagEntityValidator : AbstractValidator<Tag>
{
    public TagEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.EnglishName)
            .NotNull().WithMessage("EnglishName is required")
            .NotEmpty().WithMessage("EnglishName is required")
            .MaximumLength(TagEntityConstraints.MaxNameLength).WithMessage($"Name must be less than {TagEntityConstraints.MaxNameLength} characters");

        RuleFor(x => x.Description)
            .NotNull().WithMessage("Description is required")
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(TagEntityConstraints.MaxDescriptionLength).WithMessage($"Description must be less than {TagEntityConstraints.MaxDescriptionLength} characters");
    }
}
