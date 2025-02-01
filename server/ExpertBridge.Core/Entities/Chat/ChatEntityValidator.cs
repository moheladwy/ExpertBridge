using System;
using FluentValidation;

namespace ExpertBridge.Core.Entities.Chat;

public class ChatEntityValidator : AbstractValidator<Chat>
{
    public ChatEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Chat Id is required")
            .NotEmpty().WithMessage("Chat Id is required");

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("Chat CreatedAt is required");

        RuleFor(x => x.EndedAt)
            .GreaterThanOrEqualTo(x => x.CreatedAt)
            .When(x => x.EndedAt.HasValue);
    }
}
