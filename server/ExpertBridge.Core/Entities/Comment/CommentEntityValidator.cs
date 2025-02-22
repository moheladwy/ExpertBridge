using FluentValidation;

namespace ExpertBridge.Core.Entities.Comment;

public class CommentEntityValidator : AbstractValidator<Comment>
{
    public CommentEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.AuthorId)
            .NotNull().WithMessage("AuthorId is required")
            .NotEmpty().WithMessage("AuthorId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"AuthorId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.ParentId)
            .NotEmpty().WithMessage("ParentId cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"ParentId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Content)
            .NotNull().WithMessage("Content is required")
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(CommentEntityConstraints.MaxContentLength).WithMessage("Content must be less than 1000 characters");

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("CreatedAt is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("CreatedAt must be less than or equal to the current date")
            .When(x => x.CreatedAt != DateTime.MinValue)
            .LessThan(x => x.LastModified).WithMessage("CreatedAt must be less than LastModified")
            .When(x => x.LastModified.HasValue);

        RuleFor(x => x.LastModified)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("LastModified must be less than or equal to the current date")
            .When(x => x.LastModified.HasValue)
            .GreaterThan(x => x.CreatedAt).WithMessage("LastModified must be greater than or equal to CreatedAt")
            .When(x => x.CreatedAt != DateTime.MaxValue);
    }
}
