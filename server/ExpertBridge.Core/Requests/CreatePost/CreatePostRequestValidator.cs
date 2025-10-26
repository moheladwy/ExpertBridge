// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Posts;
using FluentValidation;

namespace ExpertBridge.Core.Requests.CreatePost;

/// <summary>
/// Validates CreatePostRequest to ensure post creation data meets requirements.
/// </summary>
/// <remarks>
/// Validates Title and Content against entity constraints from Post entity
/// to ensure data integrity during post creation.
/// </remarks>
public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
  /// <summary>
  /// Initializes a new instance of the CreatePostRequestValidator with validation rules.
  /// </summary>
  public CreatePostRequestValidator()
  {
    RuleFor(x => x.Title)
        .NotNull().WithMessage("Title cannot be null")
        .NotEmpty().WithMessage("Title cannot be empty")
        .MaximumLength(PostEntityConstraints.MaxTitleLength)
        .WithMessage($"Title cannot be longer than {PostEntityConstraints.MaxTitleLength} characters");

    RuleFor(x => x.Content)
        .NotNull().WithMessage("Content cannot be null")
        .NotEmpty().WithMessage("Content cannot be empty")
        .MaximumLength(PostEntityConstraints.MaxContentLength)
        .WithMessage($"Content cannot be longer than {PostEntityConstraints.MaxContentLength} characters");
  }
}
