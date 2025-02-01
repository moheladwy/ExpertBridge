using System;
using FluentValidation;

namespace ExpertBridge.Core.Entities.Vote.PostVote;

public class PostVoteEntityValidation : AbstractValidator<PostVote>
{
    public PostVoteEntityValidation()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("CreatedAt is required")
            .NotEmpty().WithMessage("CreatedAt is required");

        RuleFor(x => x.IsUpvote)
            .NotNull().WithMessage("isUpvote is required");

        RuleFor(x => x.ProfileId)
            .NotNull().WithMessage("ProfileId is required")
            .NotEmpty().WithMessage("ProfileId is required");

        RuleFor(x => x.PostId)
            .NotNull().WithMessage("PostId is required")
            .NotEmpty().WithMessage("PostId is required");
    }
}
