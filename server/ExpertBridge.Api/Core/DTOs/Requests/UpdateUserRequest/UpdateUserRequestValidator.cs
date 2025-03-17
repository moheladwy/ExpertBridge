using ExpertBridge.Api.Core.Entities;
using ExpertBridge.Api.Core.Entities.User;
using FluentValidation;

namespace ExpertBridge.Api.Core.DTOs.Requests.UpdateUserRequest;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.ProviderId)
            .NotNull().WithMessage("Provider ID is required")
            .NotEmpty().WithMessage("Provider ID can not be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"Provider ID must be less than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email is required")
            .NotEmpty().WithMessage("Email can not be empty")
            .MaximumLength(UserEntityConstraints.MaxEmailLength)
            .WithMessage($"Email must be less than {UserEntityConstraints.MaxEmailLength} characters")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.Username)
            .NotNull().WithMessage("Username is required")
            .NotEmpty().WithMessage("Username can not be empty")
            .MaximumLength(UserEntityConstraints.MaxUsernameLength)
            .WithMessage($"Username must be less than {UserEntityConstraints.MaxUsernameLength} characters");

        RuleFor(x => x.FirstName)
            .NotNull().WithMessage("First name is required")
            .NotEmpty().WithMessage("First name can not be empty")
            .MaximumLength(UserEntityConstraints.MaxNameLength)
            .WithMessage($"First name must be less than {UserEntityConstraints.MaxNameLength} characters");

        RuleFor(x => x.LastName)
            .NotNull().WithMessage("Last name is required")
            .NotEmpty().WithMessage("Last name can not be empty")
            .MaximumLength(UserEntityConstraints.MaxNameLength)
            .WithMessage($"Last name must be less than {UserEntityConstraints.MaxNameLength} characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required")
            .MaximumLength(UserEntityConstraints.MaxPhoneNumberLength).WithMessage(
                $"PhoneNumber must be less than {UserEntityConstraints.MaxPhoneNumberLength} characters")
            .Matches(@"^\+?[0-9]\d{9,20}$").WithMessage("PhoneNumber is not valid");
    }
}
