using FluentValidation;

namespace ExpertBridge.Core.Entities.Media;

public class MediaEntityValidator : AbstractValidator<Media>
{
    public MediaEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Name)
            .NotNull().WithMessage("Name is required")
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(MediaEntityConstraints.MaxNameLength).WithMessage($"Name must be less than {MediaEntityConstraints.MaxNameLength} characters");

        RuleFor(x => x.MediaUrl)
            .NotNull().WithMessage("MediaUrl is required")
            .NotEmpty().WithMessage("MediaUrl is required");

        RuleFor(x => x.MediaType)
            .NotNull().WithMessage("MediaType is required");

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("CreatedAt is required");
    }
}
