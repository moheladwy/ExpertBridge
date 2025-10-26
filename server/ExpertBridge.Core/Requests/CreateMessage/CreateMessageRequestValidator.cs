// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using FluentValidation;

namespace ExpertBridge.Core.Requests.CreateMessage;

/// <summary>
/// Validates CreateMessageRequest to ensure all required message fields meet constraints.
/// </summary>
/// <remarks>
/// Validates ChatId and Content against entity constraints from Message entity configuration
/// to ensure data integrity during message creation.
/// </remarks>
public class CreateMessageRequestValidator : AbstractValidator<CreateMessageRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateMessageRequestValidator with validation rules.
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
            .WithMessage($"Content cannot be longer than {GlobalEntitiesConstraints.MaxContentLetterLength} characters");
    }
}
