using FluentValidation;

namespace ExpertBridge.Core.Entities.Job;

public class JobEntityValidator : AbstractValidator<Job>
{
    public JobEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.ActualCost)
            .GreaterThanOrEqualTo(JobEntityConstraints.MinActualCost);

        RuleFor(x => x.StartedAt)
            .NotNull().WithMessage("StartedAt is required");

        RuleFor(x => x.EndedAt)
            .NotNull().WithMessage("EndedAt is required")
            .GreaterThanOrEqualTo(x => x.StartedAt);

        RuleFor(x => x.Status)
            .NotNull().WithMessage("Status is required");

        // TODO: Add rules for the rest of the properties
    }
}
