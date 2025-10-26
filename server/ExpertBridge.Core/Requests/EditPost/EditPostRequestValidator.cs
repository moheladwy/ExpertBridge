// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Posts;
using FluentValidation;

namespace ExpertBridge.Core.Requests.EditPost;

/// <summary>
/// Validates EditPostRequest to ensure post edit data meets requirements.
/// </summary>
/// <remarks>
/// Validates Title and Content when provided against entity constraints from Post entity.
/// All fields are optional.
/// </remarks>
public class EditPostRequestValidator : AbstractValidator<EditPostRequest>
{
  /// <summary>
  /// Initializes a new instance of the EditPostRequestValidator with validation rules.
  /// </summary>
  public EditPostRequestValidator()
  {
    When(x => x.Title != null, () =>
    {
      RuleFor(x => x.Title)
              .NotEmpty().WithMessage("Title cannot be empty when provided")
              .MaximumLength(PostEntityConstraints.MaxTitleLength)
              .WithMessage($"Title cannot be longer than {PostEntityConstraints.MaxTitleLength} characters");
    });

    When(x => x.Content != null, () =>
    {
      RuleFor(x => x.Content)
              .NotEmpty().WithMessage("Content cannot be empty when provided")
              .MaximumLength(PostEntityConstraints.MaxContentLength)
              .WithMessage($"Content cannot be longer than {PostEntityConstraints.MaxContentLength} characters");
    });
  }
}
