// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.JobPostings;
using FluentValidation;

namespace ExpertBridge.Core.Requests.EditJobPosting;

/// <summary>
/// Validates EditJobPostingRequest to ensure optional fields meet constraints when provided.
/// </summary>
/// <remarks>
/// All fields are optional. Validates Title, Content, Budget, and Area when provided
/// against entity constraints from JobPosting entity.
/// </remarks>
public class EditJobPostingRequestValidator : AbstractValidator<EditJobPostingRequest>
{
    /// <summary>
    /// Initializes a new instance of the EditJobPostingRequestValidator with validation rules.
    /// </summary>
    public EditJobPostingRequestValidator()
    {
        When(x => x.Title != null, () =>
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty when provided")
                .MaximumLength(JobPostingEntityConstraints.MaxTitleLength)
                .WithMessage($"Title cannot be longer than {JobPostingEntityConstraints.MaxTitleLength} characters");
        });

        When(x => x.Content != null, () =>
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content cannot be empty when provided")
                .MaximumLength(JobPostingEntityConstraints.MaxContentLength)
                .WithMessage($"Content cannot be longer than {JobPostingEntityConstraints.MaxContentLength} characters");
        });

        When(x => x.Budget != null, () =>
        {
            RuleFor(x => x.Budget)
                .GreaterThanOrEqualTo(JobPostingEntityConstraints.MinBudget)
                .WithMessage($"Budget must be greater than or equal to {JobPostingEntityConstraints.MinBudget}");
        });

        When(x => x.Area != null, () =>
        {
            RuleFor(x => x.Area)
                .NotEmpty().WithMessage("Area cannot be empty when provided")
                .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
                .WithMessage($"Area cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
        });
    }
}
