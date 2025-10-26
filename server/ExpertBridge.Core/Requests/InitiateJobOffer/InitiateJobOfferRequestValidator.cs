// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using ExpertBridge.Core.Entities;
using FluentValidation;

namespace ExpertBridge.Core.Requests.InitiateJobOffer;

/// <summary>
///     Validates InitiateJobOfferRequest to ensure all required job offer fields meet constraints.
/// </summary>
/// <remarks>
///     Validates ContractorProfileId, Title, Description, ProposedRate, and optional JobPostingId
///     against entity constraints to ensure data integrity during job offer initiation.
///     Includes XSS prevention and rate validation with upper limit.
/// </remarks>
public partial class InitiateJobOfferRequestValidator : AbstractValidator<InitiateJobOfferRequest>
{
    /// <summary>
    ///     Initializes a new instance of the InitiateJobOfferRequestValidator with validation rules.
    /// </summary>
    public InitiateJobOfferRequestValidator()
    {
        RuleFor(x => x.ContractorProfileId)
            .NotNull().WithMessage("ContractorProfileId cannot be null")
            .NotEmpty().WithMessage("ContractorProfileId cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage(
                $"ContractorProfileId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Title)
            .NotNull().WithMessage("Title cannot be null")
            .NotEmpty().WithMessage("Title cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxTitleLength)
            .WithMessage($"Title cannot be longer than {GlobalEntitiesConstraints.MaxTitleLength} characters")
            .Must(title => !ScriptTagRegex().IsMatch(title))
            .WithMessage("Title cannot contain script tags")
            .Must(title => !HtmlTagRegex().IsMatch(title))
            .WithMessage("Title cannot contain HTML tags");

        RuleFor(x => x.Description)
            .NotNull().WithMessage("Description cannot be null")
            .NotEmpty().WithMessage("Description cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxDescriptionLength)
            .WithMessage(
                $"Description cannot be longer than {GlobalEntitiesConstraints.MaxDescriptionLength} characters")
            .Must(description => !ScriptTagRegex().IsMatch(description))
            .WithMessage("Description cannot contain script tags")
            .Must(description => !DangerousPatternsRegex().IsMatch(description))
            .WithMessage("Description contains potentially dangerous content");

        RuleFor(x => x.ProposedRate)
            .GreaterThan(0)
            .WithMessage("ProposedRate must be greater than 0")
            .LessThanOrEqualTo(1000000.0)
            .WithMessage("ProposedRate cannot exceed $1,000,000");

        When(x => x.JobPostingId != null, () =>
        {
            RuleFor(x => x.JobPostingId)
                .NotEmpty().WithMessage("JobPostingId cannot be empty when provided")
                .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
                .WithMessage($"JobPostingId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
        });
    }

    /// <summary>
    ///     Compiled regex for detecting script tags in user input.
    /// </summary>
    [GeneratedRegex(@"<script[\s\S]*?>[\s\S]*?</script>", RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex ScriptTagRegex();

    /// <summary>
    ///     Compiled regex for detecting HTML tags.
    /// </summary>
    [GeneratedRegex(@"<[^>]+>", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
    private static partial Regex HtmlTagRegex();

    /// <summary>
    ///     Compiled regex for detecting dangerous patterns (javascript:, data:, on* event handlers).
    /// </summary>
    [GeneratedRegex(@"(javascript:|data:text/html|on\w+\s*=)", RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex DangerousPatternsRegex();
}
