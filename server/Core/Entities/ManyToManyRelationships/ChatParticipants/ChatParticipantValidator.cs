using FluentValidation;

namespace Core.Entities.ManyToManyRelationships.ChatParticipants;

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
