// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.Comments;
using FluentValidation;

namespace ExpertBridge.Core.Requests.CreateComment;

/// <summary>
///     Validates CreateCommentRequest to ensure comment creation data meets requirements.
/// </summary>
/// <remarks>
///     Validates Content, PostId/JobPostingId requirement (at least one must be set),
///     and optional ParentCommentId for threaded replies.
/// </remarks>
public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    /// <summary>
    ///     Initializes a new instance of the CreateCommentRequestValidator with validation rules.
    /// </summary>
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotNull().WithMessage("Content cannot be null")
            .NotEmpty().WithMessage("Content cannot be empty")
            .MaximumLength(CommentEntityConstraints.MaxContentLength)
            .WithMessage($"Content cannot be longer than {CommentEntityConstraints.MaxContentLength} characters");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.PostId) || !string.IsNullOrEmpty(x.JobPostingId))
            .WithMessage("Either PostId or JobPostingId must be provided");

        When(x => x.PostId != null, () =>
        {
            RuleFor(x => x.PostId)
                .NotEmpty().WithMessage("PostId cannot be empty when provided")
                .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
                .WithMessage($"PostId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
        });

        When(x => x.JobPostingId != null, () =>
        {
            RuleFor(x => x.JobPostingId)
                .NotEmpty().WithMessage("JobPostingId cannot be empty when provided")
                .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
                .WithMessage($"JobPostingId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
        });

        When(x => x.ParentCommentId != null, () =>
        {
            RuleFor(x => x.ParentCommentId)
                .NotEmpty().WithMessage("ParentCommentId cannot be empty when provided")
                .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
                .WithMessage(
                    $"ParentCommentId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
        });
    }
}
