using FluentValidation;

namespace ExpertBridge.Api.Core.Entities.Job;

public class JobEntityValidator : AbstractValidator<Job>
{
    public JobEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.ActualCost)
            .GreaterThanOrEqualTo(JobEntityConstraints.MinActualCost)
            .WithMessage($"ActualCost must be greater than or equal to {JobEntityConstraints.MinActualCost}");

        RuleFor(x => x.StartedAt)
            .NotNull().WithMessage("StartedAt is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("StartedAt must be less than or equal to current date")
            .GreaterThanOrEqualTo(DateTime.MinValue)
            .WithMessage("StartedAt must be greater than or equal to minimum date")
            .LessThan(x => x.EndedAt).WithMessage("StartedAt must be less than EndedAt")
            .When(x => x.EndedAt.HasValue);

        RuleFor(x => x.EndedAt)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("EndedAt must be less than or equal to current date")
            .When(x => x.StartedAt != DateTime.MinValue)
            .GreaterThanOrEqualTo(DateTime.MinValue).WithMessage("EndedAt must be greater than or equal to minimum date")
            .When(x => x.EndedAt.HasValue)
            .GreaterThan(x => x.StartedAt).WithMessage("EndedAt must be greater than StartedAt")
            .When(x => x.EndedAt.HasValue);

        RuleFor(x => x.JobStatusId)
            .NotNull().WithMessage("JobStatusId is required")
            .NotEmpty().WithMessage("JobStatusId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"JobStatusId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

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
    }
}
