using FluentValidation;

namespace ExpertBridge.Core.Entities.User;

public class UserEntityValidator : AbstractValidator<User>
{
    public UserEntityValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required")
            .NotEmpty().WithMessage("Id is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"Id must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.FirebaseId)
            .NotNull().WithMessage("FirebaseId is required")
            .NotEmpty().WithMessage("FirebaseId is required")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength).WithMessage($"FirebaseId must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.FirstName)
            .NotNull().WithMessage("Name is required")
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(UserEntityConstraints.MaxNameLength).WithMessage($"Name must be less than {UserEntityConstraints.MaxNameLength} characters");

        RuleFor(x => x.LastName)
            .NotNull().WithMessage("Name is required")
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(UserEntityConstraints.MaxNameLength).WithMessage($"Name must be less than {UserEntityConstraints.MaxNameLength} characters");

        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email is required")
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(UserEntityConstraints.MaxEmailLength).WithMessage($"Email must be less than {UserEntityConstraints.MaxEmailLength} characters")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.Username)
            .NotNull().WithMessage("Username is required")
            .NotEmpty().WithMessage("Username is required")
            .MaximumLength(UserEntityConstraints.MaxUsernameLength).WithMessage($"Username must be less than {UserEntityConstraints.MaxUsernameLength} characters");

        RuleFor(x => x.isBanned)
            .NotNull().WithMessage("isBanned is required");

        RuleFor(x => x.isDeleted)
            .NotNull().WithMessage("isDeleted is required");

        RuleFor(x => x.CreatedAt)
            .NotNull().WithMessage("CreatedAt is required");
    }
}
