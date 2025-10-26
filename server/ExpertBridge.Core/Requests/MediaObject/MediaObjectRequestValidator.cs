// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Requests.MediaObject;

/// <summary>
/// Validates MediaObjectRequest to ensure media object information is valid.
/// </summary>
/// <remarks>
/// Validates that Key and Type fields are properly provided for media attachments.
/// </remarks>
public class MediaObjectRequestValidator : AbstractValidator<MediaObjectRequest>
{
  /// <summary>
  /// Initializes a new instance of the MediaObjectRequestValidator with validation rules.
  /// </summary>
  public MediaObjectRequestValidator()
  {
    RuleFor(x => x.Key)
        .NotNull().WithMessage("Key cannot be null")
        .NotEmpty().WithMessage("Key cannot be empty");

    RuleFor(x => x.Type)
        .NotNull().WithMessage("Type cannot be null")
        .NotEmpty().WithMessage("Type cannot be empty");
  }
}
