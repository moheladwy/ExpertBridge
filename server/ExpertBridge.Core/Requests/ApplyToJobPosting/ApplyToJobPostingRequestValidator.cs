// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using FluentValidation;

namespace ExpertBridge.Core.Requests.ApplyToJobPosting;

/// <summary>
///     Validates ApplyToJobPostingRequest to ensure all required application fields meet constraints.
/// </summary>
/// <remarks>
///     Validates JobPostingId, OfferedCost, and optional CoverLetter against entity constraints
///     from JobApplication entity to ensure data integrity during application submission.
/// </remarks>
public class ApplyToJobPostingRequestValidator : AbstractValidator<ApplyToJobPostingRequest>
{
    /// <summary>
    ///     Initializes a new instance of the ApplyToJobPostingRequestValidator with validation rules.
    /// </summary>
    public ApplyToJobPostingRequestValidator()
    {
        RuleFor(x => x.JobPostingId)
            .NotNull().WithMessage("JobPostingId cannot be null")
            .NotEmpty().WithMessage("JobPostingId cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"JobPostingId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.OfferedCost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("OfferedCost must be greater than or equal to 0");

        When(x => x.CoverLetter != null, () =>
        {
            RuleFor(x => x.CoverLetter)
                .MaximumLength(GlobalEntitiesConstraints.MaxCoverLetterLength)
                .WithMessage(
                    $"CoverLetter cannot be longer than {GlobalEntitiesConstraints.MaxCoverLetterLength} characters");
        });
    }
}
