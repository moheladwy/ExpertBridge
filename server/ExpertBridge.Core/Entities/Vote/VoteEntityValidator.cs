// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Entities.Vote;

public class VoteEntityValidator : AbstractValidator<Vote>
{
    public VoteEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required");

        // TODO: Add validation rules for User ID.

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("CreatedAt is required")
            .NotEmpty().WithMessage("CreatedAt is required");

        RuleFor(x => x.IsUpvote)
            .NotNull().WithMessage("isUpvote is required");
    }
}
