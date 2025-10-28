// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using ExpertBridge.Core.Entities;
using FluentValidation;

namespace ExpertBridge.Contract.Requests.CreateMessage;

/// <summary>
///     Validates CreateMessageRequest to ensure all required message fields meet constraints.
/// </summary>
/// <remarks>
///     Validates ChatId and Content against entity constraints from Message entity configuration
///     to ensure data integrity during message creation.
///     Includes XSS prevention and dangerous pattern detection.
/// </remarks>
public partial class CreateMessageRequestValidator : AbstractValidator<CreateMessageRequest>
{
    /// <summary>
    ///     Initializes a new instance of the CreateMessageRequestValidator with validation rules.
    /// </summary>
    public CreateMessageRequestValidator()
    {
        RuleFor(x => x.ChatId)
            .NotNull().WithMessage("ChatId cannot be null")
            .NotEmpty().WithMessage("ChatId cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"ChatId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Content)
            .NotNull().WithMessage("Content cannot be null")
            .NotEmpty().WithMessage("Content cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxContentLetterLength)
            .WithMessage(
                $"Content cannot be longer than {GlobalEntitiesConstraints.MaxContentLetterLength} characters")
            .Must(content => !ScriptTagRegex().IsMatch(content ?? string.Empty))
            .WithMessage("Content cannot contain script tags")
            .Must(content => !DangerousPatternsRegex().IsMatch(content ?? string.Empty))
            .WithMessage("Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");
    }

    /// <summary>
    ///     Compiled regex for detecting script tags (XSS prevention).
    /// </summary>
    [GeneratedRegex(@"<script[^>]*>.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline, 1000)]
    private static partial Regex ScriptTagRegex();

    /// <summary>
    ///     Compiled regex for detecting dangerous patterns like javascript:, data:text/html, and event handlers.
    /// </summary>
    [GeneratedRegex(@"(javascript:|data:text/html|on\w+\s*=)", RegexOptions.IgnoreCase, 1000)]
    private static partial Regex DangerousPatternsRegex();
}
