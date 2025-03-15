using FluentValidation;

namespace ExpertBridge.Api.Core.Entities.Job.JobStatus;

public class JobStatusEntityValidator : AbstractValidator<JobStatus>
{
    public JobStatusEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required.")
            .NotEmpty().WithMessage("Id is required.")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters.");

        RuleFor(x => x.Status)
            .NotNull().WithMessage("Status is required.")
            .IsInEnum().WithMessage("Status is not valid.");
    }
}
