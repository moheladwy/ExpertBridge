// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using ExpertBridge.Core.Entities.Comments;
using FluentValidation;

namespace ExpertBridge.Core.Requests.EditComment;

/// <summary>
///     Validates EditCommentRequest to ensure comment edit data meets requirements.
/// </summary>
/// <remarks>
///     Validates Content when provided against entity constraints from Comment entity.
///     Content is optional. Includes XSS prevention and dangerous pattern detection.
/// </remarks>
public partial class EditCommentRequestValidator : AbstractValidator<EditCommentRequest>
{
    /// <summary>
    ///     Initializes a new instance of the EditCommentRequestValidator with validation rules.
    /// </summary>
    public EditCommentRequestValidator()
    {
        When(x => x.Content != null, () =>
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content cannot be empty when provided")
                .MaximumLength(CommentEntityConstraints.MaxContentLength)
                .WithMessage($"Content cannot be longer than {CommentEntityConstraints.MaxContentLength} characters")
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
    ///     Compiled regex for detecting dangerous patterns like javascript:, data:text/html, and event handlers.
    /// </summary>
    [GeneratedRegex(@"(javascript:|data:text/html|on\w+\s*=)", RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex DangerousPatternsRegex();
}
