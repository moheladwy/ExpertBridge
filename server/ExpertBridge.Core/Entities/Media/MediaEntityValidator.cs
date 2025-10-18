// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Entities.Media;

/// <summary>
/// Provides validation rules for the <see cref="MediaObject"/> entity.
/// </summary>
public class MediaEntityValidator : AbstractValidator<MediaObject>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediaEntityValidator"/> class and defines validation rules.
    /// </summary>
    public MediaEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Name)
            .NotNull().WithMessage("Name is required")
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(MediaEntityConstraints.MaxNameLength)
            .WithMessage($"Name must be less than {MediaEntityConstraints.MaxNameLength} characters");

        RuleFor(x => x.Type)
            .NotNull().WithMessage("Media Type is required");

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("CreatedAt is required");

        RuleFor(x => x.LastModified)
            .NotEqual(x => x.CreatedAt).WithMessage("LastModified must be different from CreatedAt")
            .When(x => x.LastModified.HasValue);
    }
}
