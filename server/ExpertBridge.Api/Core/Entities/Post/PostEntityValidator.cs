using FluentValidation;

namespace ExpertBridge.Api.Core.Entities.Post;

public class PostEntityValidator : AbstractValidator<Post>
{
    public PostEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.AuthorId)
            .NotNull().WithMessage("AuthorId is required")
            .NotEmpty().WithMessage("AuthorId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"AuthorId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Title)
            .NotNull().WithMessage("Title is required")
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(PostEntityConstraints.MaxTitleLength).WithMessage($"Title must be less than {PostEntityConstraints.MaxTitleLength} characters");

        RuleFor(x => x.Content)
            .NotNull().WithMessage("Content is required")
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(PostEntityConstraints.MaxContentLength).WithMessage($"Content must be less than {PostEntityConstraints.MaxContentLength} characters");

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("CreatedAt is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("CreatedAt must be less than or equal to the current date");

        RuleFor(x => x.LastModified)
            .NotEqual(x => x.CreatedAt).WithMessage("LastModified must be different from CreatedAt")
            .GreaterThan(x => x.CreatedAt).WithMessage("LastModified must be greater than or equal to CreatedAt")
            .When(x => x.LastModified.HasValue);

        RuleFor(x => x.isDeleted)
            .NotNull().WithMessage("isDeleted is required");
    }
}
