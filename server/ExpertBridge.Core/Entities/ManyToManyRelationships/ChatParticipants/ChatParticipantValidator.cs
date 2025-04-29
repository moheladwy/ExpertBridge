// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ChatParticipants;

public class ChatParticipantValidator : AbstractValidator<ChatParticipant>
{
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
