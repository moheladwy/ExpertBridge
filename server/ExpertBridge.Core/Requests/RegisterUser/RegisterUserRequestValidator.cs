using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.Users;
using FluentValidation;

namespace ExpertBridge.Core.Requests.RegisterUser;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.ProviderId)
            .NotNull().WithMessage("FirebaseId cannot be null")
            .NotEmpty().WithMessage("FirebaseId cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"FirebaseId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email cannot be null")
            .NotEmpty().WithMessage("Email cannot be empty")
            .EmailAddress().WithMessage("Email must be a valid email address")
            .MaximumLength(UserEntityConstraints.MaxEmailLength)
            .WithMessage($"Email cannot be longer than {UserEntityConstraints.MaxEmailLength} characters");

        RuleFor(x => x.Username)
            .NotNull().WithMessage("Username cannot be null")
            .NotEmpty().WithMessage("Username cannot be empty")
            .MaximumLength(UserEntityConstraints.MaxUsernameLength)
            .WithMessage($"Username cannot be longer than {UserEntityConstraints.MaxUsernameLength} characters");

        RuleFor(x => x.FirstName)
            .NotNull().WithMessage("FirstName cannot be null")
            .NotEmpty().WithMessage("FirstName cannot be empty")
            .MaximumLength(UserEntityConstraints.MaxNameLength)
            .WithMessage($"FirstName cannot be longer than {UserEntityConstraints.MaxNameLength} characters");

        RuleFor(x => x.LastName)
            .NotNull().WithMessage("LastName cannot be null")
            .NotEmpty().WithMessage("LastName cannot be empty")
            .MaximumLength(UserEntityConstraints.MaxNameLength)
            .WithMessage($"LastName cannot be longer than {UserEntityConstraints.MaxNameLength} characters");
    }
}
