// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Entities.JobCategories;

/// <summary>
/// Provides validation rules for the <see cref="JobCategory"/> entity.
/// </summary>
public class JobCategoryEntityValidator : AbstractValidator<JobCategory>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JobCategoryEntityValidator"/> class and defines validation rules.
    /// </summary>
    public JobCategoryEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Name)
            .NotNull().WithMessage("Name is required")
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(JobCategoryEntityConstraints.MaxNameLength)
            .WithMessage($"Name must be less than {JobCategoryEntityConstraints.MaxNameLength} characters");

        RuleFor(x => x.Description)
            .NotNull().WithMessage("Description is required")
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(JobCategoryEntityConstraints.MaxDescriptionLength).WithMessage(
                $"Description must be less than {JobCategoryEntityConstraints.MaxDescriptionLength} characters");
    }
}
