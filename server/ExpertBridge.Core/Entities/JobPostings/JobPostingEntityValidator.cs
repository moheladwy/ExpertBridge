// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Entities.JobPostings;

/// <summary>
/// Provides validation rules for the <see cref="JobPosting"/> entity.
/// </summary>
public class JobPostingEntityValidator : AbstractValidator<JobPosting>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JobPostingEntityValidator"/> class and defines validation rules.
    /// </summary>
    public JobPostingEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.AuthorId)
            .NotNull().WithMessage("AuthorId is required")
            .NotEmpty().WithMessage("AuthorId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"AuthorId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        //RuleFor(x => x.AreaId)
        //    .NotNull().WithMessage("AreaId is required")
        //    .NotEmpty().WithMessage("AreaId is required")
        //    .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"AreaId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Area)
            .NotNull().WithMessage("Area is required")
            .NotEmpty().WithMessage("Area is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"Area length must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        //RuleFor(x => x.CategoryId)
        //    .NotNull().WithMessage("CategoryId is required")
        //    .NotEmpty().WithMessage("CategoryId is required")
        //    .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"CategoryId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Title)
            .NotNull().WithMessage("Title is required")
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(JobPostingEntityConstraints.MaxTitleLength)
            .WithMessage($"Title must be less than {JobPostingEntityConstraints.MaxTitleLength} characters");

        RuleFor(x => x.Content)
            .NotNull().WithMessage("Content is required")
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(JobPostingEntityConstraints.MaxContentLength)
            .WithMessage($"Content must be less than {JobPostingEntityConstraints.MaxContentLength} characters");

        RuleFor(x => x.Budget)
            .NotNull().WithMessage("Budget is required")
            .GreaterThanOrEqualTo(JobPostingEntityConstraints.MinBudget)
            .WithMessage($"Budget must be greater than or equal to {JobPostingEntityConstraints.MinBudget}");
    }
}
