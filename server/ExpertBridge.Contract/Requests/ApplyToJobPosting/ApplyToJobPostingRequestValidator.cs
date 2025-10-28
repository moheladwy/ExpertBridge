// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using ExpertBridge.Core.Entities;
using FluentValidation;

namespace ExpertBridge.Contract.Requests.ApplyToJobPosting;

/// <summary>
///     Validates ApplyToJobPostingRequest to ensure all required application fields meet constraints.
/// </summary>
/// <remarks>
///     Validates JobPostingId, OfferedCost, and optional CoverLetter against entity constraints
///     from JobApplication entity to ensure data integrity during application submission.
///     Includes XSS prevention for CoverLetter and cost validation with upper limit.
/// </remarks>
public partial class ApplyToJobPostingRequestValidator : AbstractValidator<ApplyToJobPostingRequest>
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
            .WithMessage("OfferedCost must be greater than or equal to 0")
            .LessThanOrEqualTo(1000000m)
            .WithMessage("OfferedCost cannot exceed $1,000,000");

        When(x => x.CoverLetter != null, () =>
        {
            RuleFor(x => x.CoverLetter)
                .MaximumLength(GlobalEntitiesConstraints.MaxCoverLetterLength)
                .WithMessage(
                    $"CoverLetter cannot be longer than {GlobalEntitiesConstraints.MaxCoverLetterLength} characters")
                .Must(letter => !ScriptTagRegex().IsMatch(letter))
                .WithMessage("CoverLetter cannot contain script tags")
                .Must(letter => !DangerousPatternsRegex().IsMatch(letter))
                .WithMessage("CoverLetter contains potentially dangerous content");
        });
    }

    /// <summary>
    ///     Compiled regex for detecting script tags in user input.
    /// </summary>
    [GeneratedRegex(@"<script[\s\S]*?>[\s\S]*?</script>", RegexOptions.IgnoreCase, 1000)]
    private static partial Regex ScriptTagRegex();

    /// <summary>
    ///     Compiled regex for detecting dangerous patterns (javascript:, data:, on* event handlers).
    /// </summary>
    [GeneratedRegex(@"(javascript:|data:text/html|on\w+\s*=)", RegexOptions.IgnoreCase, 1000)]
    private static partial Regex DangerousPatternsRegex();
}
