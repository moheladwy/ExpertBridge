using FluentValidation;

namespace ExpertBridge.Core.Entities.Media;

public class MediaEntityValidator : AbstractValidator<Media>
{
    public MediaEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Name)
            .NotNull().WithMessage("Name is required")
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(MediaEntityConstraints.MaxNameLength).WithMessage($"Name must be less than {MediaEntityConstraints.MaxNameLength} characters");

        RuleFor(x => x.MediaUrl)
            .NotNull().WithMessage("MediaUrl is required")
            .NotEmpty().WithMessage("MediaUrl is required")
            .MaximumLength(MediaEntityConstraints.MaxMediaUrlLength).WithMessage($"MediaUrl must be less than {MediaEntityConstraints.MaxMediaUrlLength} characters");

        RuleFor(x => x.MediaType)
            .NotNull().WithMessage("MediaType is required");

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("CreatedAt is required");

        RuleFor(x => x.LastModified)
            .NotEqual(x => x.CreatedAt).WithMessage("LastModified must be different from CreatedAt")
            .When(x => x.LastModified.HasValue);
    }
}
