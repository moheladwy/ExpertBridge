using FluentValidation;

namespace ExpertBridge.Core.Entities.Comment;

public class CommentEntityValidation : AbstractValidator<Comment>
{
    public CommentEntityValidation()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Content)
            .NotNull().WithMessage("Content is required")
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(CommentEntityConstraints.MaxContentLength).WithMessage("Content must be less than 1000 characters");

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("CreatedAt is required")
            .NotEmpty().WithMessage("CreatedAt is required");

        RuleFor(x => x.LastModified)
            .NotEqual(x => x.CreatedAt).WithMessage("LastModified must be different from CreatedAt");
    }
}
