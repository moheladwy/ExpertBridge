using FluentValidation;

namespace ExpertBridge.Core.Entities.Job;

public class JobEntityValidator : AbstractValidator<Job>
{
    public JobEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.ActualCost)
            .GreaterThanOrEqualTo(JobEntityConstraints.MinActualCost);

        RuleFor(x => x.StartedAt)
            .NotNull().WithMessage("StartedAt is required");

        RuleFor(x => x.EndedAt)
            .NotEqual(x => x.StartedAt).When(x => x.EndedAt.HasValue)
            .GreaterThanOrEqualTo(x => x.StartedAt)
            .When(x => x.EndedAt.HasValue)
            .WithMessage("EndedAt must be greater than or equal to StartedAt");

        RuleFor(x => x.Status)
            .NotNull().WithMessage("Status is required");

        RuleFor(x => x.JobStatusId)
            .NotNull().WithMessage("JobStatusId is required")
            .NotEmpty().WithMessage("JobStatusId is required");

        RuleFor(x => x.WorkerId)
            .NotNull().WithMessage("WorkerId is required")
            .NotEmpty().WithMessage("WorkerId is required");

        RuleFor(x => x.AuthorId)
            .NotNull().WithMessage("AuthorId is required")
            .NotEmpty().WithMessage("AuthorId is required");

        RuleFor(x => x.JobPostingId)
            .NotNull().WithMessage("JobPostingId is required")
            .NotEmpty().WithMessage("JobPostingId is required");
    }
}
