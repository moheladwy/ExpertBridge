// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Entities.Jobs;

public class JobEntityValidator : AbstractValidator<Job>
{
    public JobEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");
        RuleFor(x => x.Title)
           .NotEmpty().WithMessage("Title is required.")
           .MaximumLength(GlobalEntitiesConstraints.MaxTitleLength).WithMessage($"Title must be less than {GlobalEntitiesConstraints.MaxTitleLength} characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(GlobalEntitiesConstraints.MaxDescriptionLength).WithMessage($"Description must be less than {GlobalEntitiesConstraints.MaxDescriptionLength} characters.");

        RuleFor(x => x.Status)
            .NotNull().WithMessage("Status is required.")
            .IsInEnum().WithMessage("Status is not a valid value.");

        RuleFor(x => x.WorkerId)
            .NotNull().WithMessage("WorkerId is required")
            .NotEmpty().WithMessage("WorkerId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"WorkerId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.AuthorId)
            .NotNull().WithMessage("AuthorId is required")
            .NotEmpty().WithMessage("AuthorId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"AuthorId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.JobPostingId)
            .NotNull().WithMessage("JobPostingId is required")
            .NotEmpty().WithMessage("JobPostingId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"JobPostingId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.CreatedAt)
            .NotEmpty().WithMessage("CreatedAt is required.");

        // market decide i guess
        // RuleFor(x => x.ActualCost)
        //     .GreaterThanOrEqualTo(JobEntityConstraints.MinActualCost)
        //     .WithMessage($"ActualCost must be greater than or equal to {JobEntityConstraints.MinActualCost}");

        // not required anymore
        // RuleFor(x => x.StartedAt)
        //     .NotNull().WithMessage("StartedAt is required")
        //     .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("StartedAt must be less than or equal to current date")
        //     .GreaterThanOrEqualTo(DateTime.MinValue)
        //     .WithMessage("StartedAt must be greater than or equal to minimum date")
        //     .LessThan(x => x.EndedAt).WithMessage("StartedAt must be less than EndedAt")
        //     .When(x => x.EndedAt.HasValue);

        RuleFor(x => x.EndedAt)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("EndedAt must be less than or equal to current date")
            .When(x => x.StartedAt != DateTime.MinValue)
            .GreaterThanOrEqualTo(DateTime.MinValue).WithMessage("EndedAt must be greater than or equal to minimum date")
            .When(x => x.EndedAt.HasValue)
            .GreaterThan(x => x.StartedAt).WithMessage("EndedAt must be greater than StartedAt")
            .When(x => x.EndedAt.HasValue);





    }
}
