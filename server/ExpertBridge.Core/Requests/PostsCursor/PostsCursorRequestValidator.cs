// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using FluentValidation;

namespace ExpertBridge.Core.Requests.PostsCursor;

/// <summary>
/// Validates PostsCursorRequest to ensure pagination parameters are valid.
/// </summary>
/// <remarks>
/// Validates PageSize, Page, and optional cursor fields for post pagination.
/// </remarks>
public class PostsCursorRequestValidator : AbstractValidator<PostsCursorRequest>
{
  /// <summary>
  /// Initializes a new instance of the PostsCursorRequestValidator with validation rules.
  /// </summary>
  public PostsCursorRequestValidator()
  {
    RuleFor(x => x.PageSize)
        .GreaterThan(0).WithMessage("PageSize must be greater than 0")
        .LessThanOrEqualTo(100).WithMessage("PageSize cannot exceed 100");

    RuleFor(x => x.Page)
        .GreaterThan(0).WithMessage("Page must be greater than 0");

    When(x => x.After.HasValue, () =>
    {
      RuleFor(x => x.After)
              .GreaterThanOrEqualTo(0).WithMessage("After must be greater than or equal to 0")
              .LessThanOrEqualTo(1).WithMessage("After must be less than or equal to 1");
    });

    When(x => x.LastIdCursor != null, () =>
    {
      RuleFor(x => x.LastIdCursor)
              .NotEmpty().WithMessage("LastIdCursor cannot be empty when provided")
              .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
              .WithMessage($"LastIdCursor cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
    });
  }
}
