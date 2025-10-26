// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using FluentValidation;

namespace ExpertBridge.Core.Requests.UpdateJobStatus;

/// <summary>
/// Validates UpdateJobStatusRequest to ensure job status update is valid.
/// </summary>
/// <remarks>
/// Validates that Status field is properly provided and meets length constraints.
/// </remarks>
public class UpdateJobStatusRequestValidator : AbstractValidator<UpdateJobStatusRequest>
{
  /// <summary>
  /// Initializes a new instance of the UpdateJobStatusRequestValidator with validation rules.
  /// </summary>
  public UpdateJobStatusRequestValidator()
  {
    RuleFor(x => x.Status)
        .NotNull().WithMessage("Status cannot be null")
        .NotEmpty().WithMessage("Status cannot be empty")
        .MaximumLength(GlobalEntitiesConstraints.MaxEnumsLength)
        .WithMessage($"Status cannot be longer than {GlobalEntitiesConstraints.MaxEnumsLength} characters");
  }
}
