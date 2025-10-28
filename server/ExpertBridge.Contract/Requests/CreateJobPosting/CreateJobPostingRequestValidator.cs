// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.JobPostings;
using FluentValidation;

namespace ExpertBridge.Contract.Requests.CreateJobPosting;

/// <summary>
///     Validates CreateJobPostingRequest to ensure all required job posting fields meet constraints.
/// </summary>
/// <remarks>
///     Validates Area, Title, Content, and Budget against entity constraints from JobPosting entity
///     to ensure data integrity during job posting creation.
///     Includes XSS prevention, dangerous pattern detection, and budget validation.
/// </remarks>
public partial class CreateJobPostingRequestValidator : AbstractValidator<CreateJobPostingRequest>
{
    /// <summary>
    ///     Initializes a new instance of the CreateJobPostingRequestValidator with validation rules.
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
            .WithMessage($"Title cannot be longer than {JobPostingEntityConstraints.MaxTitleLength} characters")
            .Must(title => !ScriptTagRegex().IsMatch(title ?? string.Empty))
            .WithMessage("Title cannot contain script tags")
            .Must(title => !HtmlTagRegex().IsMatch(title ?? string.Empty))
            .WithMessage("Title cannot contain HTML tags");

        RuleFor(x => x.Content)
            .NotNull().WithMessage("Content cannot be null")
            .NotEmpty().WithMessage("Content cannot be empty")
            .MaximumLength(JobPostingEntityConstraints.MaxContentLength)
            .WithMessage($"Content cannot be longer than {JobPostingEntityConstraints.MaxContentLength} characters")
            .Must(content => !ScriptTagRegex().IsMatch(content ?? string.Empty))
            .WithMessage("Content cannot contain script tags")
            .Must(content => !DangerousPatternsRegex().IsMatch(content ?? string.Empty))
            .WithMessage("Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");

        RuleFor(x => x.Budget)
            .GreaterThanOrEqualTo(JobPostingEntityConstraints.MinBudget)
            .WithMessage($"Budget must be greater than or equal to {JobPostingEntityConstraints.MinBudget}")
            .LessThanOrEqualTo(1000000m)
            .WithMessage("Budget must be less than or equal to 1,000,000");
    }

    /// <summary>
    ///     Compiled regex for detecting script tags (XSS prevention).
    /// </summary>
    [GeneratedRegex(@"<script[^>]*>.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline, 1000)]
    private static partial Regex ScriptTagRegex();

    /// <summary>
    ///     Compiled regex for detecting HTML tags in titles.
    /// </summary>
    [GeneratedRegex(@"<[^>]+>", RegexOptions.None, 1000)]
    private static partial Regex HtmlTagRegex();

    /// <summary>
    ///     Compiled regex for detecting dangerous patterns like javascript:, data:text/html, and event handlers.
    /// </summary>
    [GeneratedRegex(@"(javascript:|data:text/html|on\w+\s*=)", RegexOptions.IgnoreCase, 1000)]
    private static partial Regex DangerousPatternsRegex();
}
