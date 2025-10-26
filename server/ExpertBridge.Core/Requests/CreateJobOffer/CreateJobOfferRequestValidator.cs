// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using ExpertBridge.Core.Entities;
using FluentValidation;

namespace ExpertBridge.Core.Requests.CreateJobOffer;

/// <summary>
///     Validates CreateJobOfferRequest to ensure all required job offer fields meet constraints.
/// </summary>
/// <remarks>
///     Validates Title, Description, Area, Budget, and WorkerId against entity constraints
///     from JobOffer entity to ensure data integrity during direct job offer creation.
///     Includes XSS prevention, budget range validation, and GUID format validation.
/// </remarks>
public partial class CreateJobOfferRequestValidator : AbstractValidator<CreateJobOfferRequest>
{
    /// <summary>
    ///     Initializes a new instance of the CreateJobOfferRequestValidator with validation rules.
    /// </summary>
    public CreateJobOfferRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotNull().WithMessage("Title cannot be null")
            .NotEmpty().WithMessage("Title cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxTitleLength)
            .WithMessage($"Title cannot be longer than {GlobalEntitiesConstraints.MaxTitleLength} characters")
            .Must(title => !ScriptTagRegex().IsMatch(title ?? string.Empty))
            .WithMessage("Title cannot contain script tags")
            .Must(title => !HtmlTagRegex().IsMatch(title ?? string.Empty))
            .WithMessage("Title cannot contain HTML tags");

        RuleFor(x => x.Description)
            .NotNull().WithMessage("Description cannot be null")
            .NotEmpty().WithMessage("Description cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxDescriptionLength)
            .WithMessage(
                $"Description cannot be longer than {GlobalEntitiesConstraints.MaxDescriptionLength} characters")
            .Must(desc => !ScriptTagRegex().IsMatch(desc ?? string.Empty))
            .WithMessage("Description cannot contain script tags")
            .Must(desc => !DangerousPatternsRegex().IsMatch(desc ?? string.Empty))
            .WithMessage("Description contains potentially dangerous patterns");

        RuleFor(x => x.Area)
            .NotNull().WithMessage("Area cannot be null")
            .NotEmpty().WithMessage("Area cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"Area cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters")
            .Must(area => GuidRegex().IsMatch(area ?? string.Empty))
            .WithMessage("Area must be a valid GUID format");

        RuleFor(x => x.Budget)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Budget must be greater than or equal to 0")
            .LessThanOrEqualTo(1000000m)
            .WithMessage("Budget cannot exceed 1,000,000");

        RuleFor(x => x.WorkerId)
            .NotNull().WithMessage("WorkerId cannot be null")
            .NotEmpty().WithMessage("WorkerId cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"WorkerId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters")
            .Must(id => GuidRegex().IsMatch(id ?? string.Empty))
            .WithMessage("WorkerId must be a valid GUID format");
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

    /// <summary>
    ///     Compiled regex for GUID format validation.
    /// </summary>
    [GeneratedRegex(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
    private static partial Regex GuidRegex();
}
