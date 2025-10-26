// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.Comments;
using FluentValidation;

namespace ExpertBridge.Core.Requests.PatchComment;

/// <summary>
///     Validates PatchCommentRequest to ensure partial comment update is valid.
/// </summary>
/// <remarks>
///     Validates CommentId and optional Content when provided. Voting fields (Upvote/Downvote)
///     don't require validation as they are boolean flags.
/// </remarks>
public class PatchCommentRequestValidator : AbstractValidator<PatchCommentRequest>
{
    /// <summary>
    ///     Initializes a new instance of the PatchCommentRequestValidator with validation rules.
    /// </summary>
    public PatchCommentRequestValidator()
    {
        RuleFor(x => x.CommentId)
            .NotNull().WithMessage("CommentId cannot be null")
            .NotEmpty().WithMessage("CommentId cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"CommentId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");

        When(x => x.Content != null, () =>
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content cannot be empty when provided")
                .MaximumLength(CommentEntityConstraints.MaxContentLength)
                .WithMessage($"Content cannot be longer than {CommentEntityConstraints.MaxContentLength} characters");
        });
    }
}
