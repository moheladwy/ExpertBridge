using FluentValidation;

namespace ExpertBridge.Core.Entities.Votes.PostVote;

public class PostVoteEntityValidator : AbstractValidator<PostVote>
{
    public PostVoteEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("CreatedAt is required")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("CreatedAt must be less than or equal to the current date")
            .GreaterThanOrEqualTo(DateTime.MinValue)
            .WithMessage("CreatedAt must be greater than or equal to the minimum date");

        RuleFor(x => x.IsUpvote)
            .NotNull().WithMessage("isUpvote is required");

        RuleFor(x => x.ProfileId)
            .NotNull().WithMessage("ProfileId is required")
            .NotEmpty().WithMessage("ProfileId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"ProfileId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.PostId)
            .NotNull().WithMessage("PostId is required")
            .NotEmpty().WithMessage("PostId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"PostId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");
    }
}
