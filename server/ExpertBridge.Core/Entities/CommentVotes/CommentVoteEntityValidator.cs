// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Entities.CommentVotes;

public class CommentVoteEntityValidator : AbstractValidator<CommentVote>
{
    public CommentVoteEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("CreatedAt is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("CreatedAt must be less than or equal to the current date")
            .GreaterThanOrEqualTo(DateTime.MinValue)
            .WithMessage("CreatedAt must be greater than or equal to the minimum date");

        RuleFor(x => x.IsUpvote)
            .NotNull().WithMessage("isUpvote is required");

        RuleFor(x => x.CommentId)
            .NotNull().WithMessage("CommentId is required")
            .NotEmpty().WithMessage("CommentId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"CommentId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.ProfileId)
            .NotNull().WithMessage("ProfileId is required")
            .NotEmpty().WithMessage("ProfileId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"ProfileId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");
    }
}
