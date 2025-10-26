// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.JobPostings;
using FluentValidation;

namespace ExpertBridge.Core.Requests.CreateJobPosting;

/// <summary>
/// Validates CreateJobPostingRequest to ensure all required job posting fields meet constraints.
/// </summary>
/// <remarks>
/// Validates Area, Title, Content, and Budget against entity constraints from JobPosting entity
/// to ensure data integrity during job posting creation.
/// </remarks>
public class CreateJobPostingRequestValidator : AbstractValidator<CreateJobPostingRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateJobPostingRequestValidator with validation rules.
    /// </summary>
    public CreateJobPostingRequestValidator()
    {
        RuleFor(x => x.Area)
            .NotNull().WithMessage("Area cannot be null")
            .NotEmpty().WithMessage("Area cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"Area cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Title)
            .NotNull().WithMessage("Title cannot be null")
            .NotEmpty().WithMessage("Title cannot be empty")
            .MaximumLength(JobPostingEntityConstraints.MaxTitleLength)
            .WithMessage($"Title cannot be longer than {JobPostingEntityConstraints.MaxTitleLength} characters");

        RuleFor(x => x.Content)
            .NotNull().WithMessage("Content cannot be null")
            .NotEmpty().WithMessage("Content cannot be empty")
            .MaximumLength(JobPostingEntityConstraints.MaxContentLength)
            .WithMessage($"Content cannot be longer than {JobPostingEntityConstraints.MaxContentLength} characters");

        RuleFor(x => x.Budget)
            .GreaterThanOrEqualTo(JobPostingEntityConstraints.MinBudget)
            .WithMessage($"Budget must be greater than or equal to {JobPostingEntityConstraints.MinBudget}");
    }
}
