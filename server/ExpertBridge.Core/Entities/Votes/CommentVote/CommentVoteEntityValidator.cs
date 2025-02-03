using FluentValidation;

namespace ExpertBridge.Core.Entities.Votes.CommentVote;

public class CommentVoteEntityValidator : AbstractValidator<CommentVote>
{
    public CommentVoteEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("CreatedAt is required")
            .NotEmpty().WithMessage("CreatedAt is required");

        RuleFor(x => x.IsUpvote)
            .NotNull().WithMessage("isUpvote is required");

        RuleFor(x => x.CommentId)
            .NotNull().WithMessage("CommentId is required")
            .NotEmpty().WithMessage("CommentId is required");

        RuleFor(x => x.ProfileId)
            .NotNull().WithMessage("ProfileId is required")
            .NotEmpty().WithMessage("ProfileId is required");
    }
}
