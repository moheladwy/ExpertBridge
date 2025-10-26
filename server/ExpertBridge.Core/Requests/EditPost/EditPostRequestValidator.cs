// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using ExpertBridge.Core.Entities.Posts;
using FluentValidation;

namespace ExpertBridge.Core.Requests.EditPost;

/// <summary>
///     Validates EditPostRequest to ensure post edit data meets requirements.
/// </summary>
/// <remarks>
///     Validates Title and Content when provided against entity constraints from Post entity.
///     All fields are optional. Includes XSS prevention and dangerous pattern detection.
/// </remarks>
public partial class EditPostRequestValidator : AbstractValidator<EditPostRequest>
{
    /// <summary>
    ///     Initializes a new instance of the EditPostRequestValidator with validation rules.
    /// </summary>
    public EditPostRequestValidator()
    {
        When(x => x.Title != null, () =>
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty when provided")
                .MaximumLength(PostEntityConstraints.MaxTitleLength)
                .WithMessage($"Title cannot be longer than {PostEntityConstraints.MaxTitleLength} characters")
                .Must(title => !ScriptTagRegex().IsMatch(title ?? string.Empty))
                .WithMessage("Title cannot contain script tags")
                .Must(title => !HtmlTagRegex().IsMatch(title ?? string.Empty))
                .WithMessage("Title cannot contain HTML tags");
        });

        When(x => x.Content != null, () =>
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content cannot be empty when provided")
                .MaximumLength(PostEntityConstraints.MaxContentLength)
                .WithMessage($"Content cannot be longer than {PostEntityConstraints.MaxContentLength} characters")
                .Must(content => !ScriptTagRegex().IsMatch(content ?? string.Empty))
                .WithMessage("Content cannot contain script tags")
                .Must(content => !DangerousPatternsRegex().IsMatch(content ?? string.Empty))
                .WithMessage("Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");
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
