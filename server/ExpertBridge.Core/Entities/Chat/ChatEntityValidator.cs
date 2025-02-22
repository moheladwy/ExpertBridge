using System;
using FluentValidation;

namespace ExpertBridge.Core.Entities.Chat;

public class ChatEntityValidator : AbstractValidator<Chat>
{
    public ChatEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Chat Id is required")
            .NotEmpty().WithMessage("Chat Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Chat Id must not exceed {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("Chat CreatedAt is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Chat CreatedAt must not be in the future")
            .When(x => x.CreatedAt != DateTime.MinValue)
            .LessThan(x => x.EndedAt).WithMessage("Chat CreatedAt must be less than EndedAt")
            .When(x => x.EndedAt.HasValue);

        RuleFor(x => x.EndedAt)
            .GreaterThan(x => x.CreatedAt).WithMessage("Chat EndedAt must be greater than CreatedAt")
            .When(x => x.EndedAt.HasValue);
    }
}
