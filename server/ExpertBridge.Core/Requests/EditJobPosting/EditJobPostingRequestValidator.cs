// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.JobPostings;
using FluentValidation;

namespace ExpertBridge.Core.Requests.EditJobPosting;

/// <summary>
///     Validates EditJobPostingRequest to ensure optional fields meet constraints when provided.
/// </summary>
/// <remarks>
///     All fields are optional. Validates Title, Content, Budget, and Area when provided
///     against entity constraints from JobPosting entity.
///     Includes XSS prevention, dangerous pattern detection, and budget validation.
/// </remarks>
public partial class EditJobPostingRequestValidator : AbstractValidator<EditJobPostingRequest>
{
    /// <summary>
    ///     Initializes a new instance of the EditJobPostingRequestValidator with validation rules.
    /// </summary>
    public EditJobPostingRequestValidator()
    {
        When(x => x.Title != null, () =>
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty when provided")
                .MaximumLength(JobPostingEntityConstraints.MaxTitleLength)
                .WithMessage($"Title cannot be longer than {JobPostingEntityConstraints.MaxTitleLength} characters")
                .Must(title => !ScriptTagRegex().IsMatch(title ?? string.Empty))
                .WithMessage("Title cannot contain script tags")
                .Must(title => !HtmlTagRegex().IsMatch(title ?? string.Empty))
                .WithMessage("Title cannot contain HTML tags");
        });

        When(x => x.Content != null, () =>
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content cannot be empty when provided")
                .MaximumLength(JobPostingEntityConstraints.MaxContentLength)
                .WithMessage(
                    $"Content cannot be longer than {JobPostingEntityConstraints.MaxContentLength} characters")
                .Must(content => !ScriptTagRegex().IsMatch(content ?? string.Empty))
                .WithMessage("Content cannot contain script tags")
                .Must(content => !DangerousPatternsRegex().IsMatch(content ?? string.Empty))
                .WithMessage("Content contains potentially dangerous patterns");
        });

        When(x => x.Budget != null, () =>
        {
            RuleFor(x => x.Budget)
                .GreaterThanOrEqualTo(JobPostingEntityConstraints.MinBudget)
                .WithMessage($"Budget must be greater than or equal to {JobPostingEntityConstraints.MinBudget}")
                .LessThanOrEqualTo(1000000m)
                .WithMessage("Budget cannot exceed 1,000,000");
        });

        When(x => x.Area != null, () =>
        {
            RuleFor(x => x.Area)
                .NotEmpty().WithMessage("Area cannot be empty when provided")
                .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
                .WithMessage($"Area cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
        });
    }

    /// <summary>
    ///     Compiled regex for detecting script tags (XSS prevention).
    /// </summary>
    [GeneratedRegex(@"<script[^>]*>.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline, matchTimeoutMilliseconds: 1000)]
    private static partial Regex ScriptTagRegex();

    /// <summary>
    ///     Compiled regex for detecting HTML tags in titles.
    /// </summary>
    [GeneratedRegex(@"<[^>]+>", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
    private static partial Regex HtmlTagRegex();

    /// <summary>
    ///     Compiled regex for detecting dangerous patterns like javascript:, data:text/html, and event handlers.
    /// </summary>
    [GeneratedRegex(@"(javascript:|data:text/html|on\w+\s*=)", RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex DangerousPatternsRegex();
}
