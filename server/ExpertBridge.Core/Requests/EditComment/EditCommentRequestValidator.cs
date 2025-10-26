// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Comments;
using FluentValidation;

namespace ExpertBridge.Core.Requests.EditComment;

/// <summary>
/// Validates EditCommentRequest to ensure comment edit data meets requirements.
/// </summary>
/// <remarks>
/// Validates Content when provided against entity constraints from Comment entity.
/// Content is optional.
/// </remarks>
public class EditCommentRequestValidator : AbstractValidator<EditCommentRequest>
{
  /// <summary>
  /// Initializes a new instance of the EditCommentRequestValidator with validation rules.
  /// </summary>
  public EditCommentRequestValidator()
  {
    When(x => x.Content != null, () =>
    {
      RuleFor(x => x.Content)
              .NotEmpty().WithMessage("Content cannot be empty when provided")
              .MaximumLength(CommentEntityConstraints.MaxContentLength)
              .WithMessage($"Content cannot be longer than {CommentEntityConstraints.MaxContentLength} characters");
    });
  }
}
