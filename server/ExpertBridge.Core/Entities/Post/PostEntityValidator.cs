// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Entities.Post;

public class PostEntityValidator : AbstractValidator<Post>
{
    public PostEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Title)
            .NotNull().WithMessage("Title is required")
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(PostEntityConstraints.MaxTitleLength).WithMessage($"Title must be less than {PostEntityConstraints.MaxTitleLength} characters");

        RuleFor(x => x.Content)
            .NotNull().WithMessage("Content is required")
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(PostEntityConstraints.MaxContentLength).WithMessage($"Content must be less than {PostEntityConstraints.MaxContentLength} characters");

        // TODO: Add AuthorId validation

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("CreatedAt is required");

        RuleFor(x => x.LastModified)
            .NotEqual(x => x.CreatedAt).WithMessage("LastModified must be different from CreatedAt");

        RuleFor(x => x.isDeleted)
            .NotNull().WithMessage("isDeleted is required");
    }
}
