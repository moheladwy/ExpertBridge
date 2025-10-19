// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Entities.ManyToManyRelationships.ChatParticipants;

/// <summary>
/// Provides validation rules for the <see cref="ChatParticipant"/> entity.
/// </summary>
public class ChatParticipantValidator : AbstractValidator<ChatParticipant>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatParticipantValidator"/> class and defines validation rules.
    /// </summary>
    public ChatParticipantValidator()
    {
        RuleFor(x => x.ChatId)
            .NotNull().WithMessage("Chat Id is required")
            .NotEmpty().WithMessage("Chat Id is required");

        RuleFor(x => x.ProfileId)
            .NotNull().WithMessage("Profile Id is required")
            .NotEmpty().WithMessage("Profile Id is required");
    }
}
