// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using ExpertBridge.Contract.Requests.MediaObject;
using ExpertBridge.Core.Entities.Posts;
using FluentValidation;

namespace ExpertBridge.Contract.Requests.CreatePost;

/// <summary>
///     Validates CreatePostRequest to ensure post creation data meets requirements.
/// </summary>
/// <remarks>
///     Validates Title and Content against entity constraints from Post entity
///     to ensure data integrity during post creation.
///     Enhanced in Phase 3 with XSS prevention and content security rules.
/// </remarks>
public partial class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    /// <summary>
    ///     Initializes a new instance of the CreatePostRequestValidator with validation rules.
    /// </summary>
    public CreatePostRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotNull().WithMessage("Title cannot be null")
            .NotEmpty().WithMessage("Title cannot be empty")
            .MaximumLength(PostEntityConstraints.MaxTitleLength)
            .WithMessage($"Title cannot be longer than {PostEntityConstraints.MaxTitleLength} characters")
            .Must(NotContainScriptTags).WithMessage("Title must not contain HTML script tags")
            .Must(NotContainHtmlTags).WithMessage("Title must not contain HTML tags");

        RuleFor(x => x.Content)
            .NotNull().WithMessage("Content cannot be null")
            .NotEmpty().WithMessage("Content cannot be empty")
            .MaximumLength(PostEntityConstraints.MaxContentLength)
            .WithMessage($"Content cannot be longer than {PostEntityConstraints.MaxContentLength} characters")
            .Must(NotContainScriptTags).WithMessage("Content must not contain script tags")
            .Must(NotContainDangerousPatterns).WithMessage("Content contains potentially unsafe patterns");

        // Validate nested media collection if present
        RuleForEach(x => x.Media)
            .SetValidator(new MediaObjectRequestValidator())
            .When(x => x.Media != null && x.Media.Count > 0);

        // Limit media attachments
        RuleFor(x => x.Media)
            .Must(media => media == null || media.Count <= 10)
            .WithMessage("Cannot attach more than 10 media items per post");
    }

    /// <summary>
    ///     Validates that the input does not contain script tags.
    /// </summary>
    private static bool NotContainScriptTags(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return true;
        }

        return !ScriptTagRegex().IsMatch(input);
    }

    /// <summary>
    ///     Validates that the input does not contain HTML tags (for title field).
    /// </summary>
    private static bool NotContainHtmlTags(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return true;
        }

        return !HtmlTagRegex().IsMatch(input);
    }

    /// <summary>
    ///     Validates that the content does not contain dangerous patterns like javascript:, data:, or event handlers.
    /// </summary>
    private static bool NotContainDangerousPatterns(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return true;
        }

        return !DangerousPatternsRegex().IsMatch(input);
    }

    [GeneratedRegex(@"<script[^>]*>.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline, 1000)]
    private static partial Regex ScriptTagRegex();

    [GeneratedRegex(@"<[^>]+>", RegexOptions.None, 1000)]
    private static partial Regex HtmlTagRegex();

    [GeneratedRegex(@"(javascript:|data:text/html|on\w+\s*=)", RegexOptions.IgnoreCase, 1000)]
    private static partial Regex DangerousPatternsRegex();
}
