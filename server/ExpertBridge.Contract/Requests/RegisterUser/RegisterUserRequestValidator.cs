// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.Users;
using FluentValidation;

namespace ExpertBridge.Contract.Requests.RegisterUser;

/// <summary>
///     Validates RegisterUserRequestValidator to ensure all required user registration fields meet constraints.
/// </summary>
/// <remarks>
///     Validates ProviderId, Email, Username, FirstName, and LastName against entity constraints
///     to ensure data integrity during user registration with Firebase authentication.
///     Includes email validation and name pattern validation.
/// </remarks>
public partial class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    /// <summary>
    ///     Initializes a new instance of the RegisterUserRequestValidator with validation rules.
    /// </summary>
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
            .MaximumLength(UserEntityConstraints.MaxEmailLength)
            .WithMessage($"Email cannot be longer than {UserEntityConstraints.MaxEmailLength} characters");

        When(x => x.Email != null, () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email must be a valid email address")
                .Must(email => !email.Contains("..", StringComparison.Ordinal))
                .WithMessage("Email cannot contain consecutive dots")
                .Must(email => !email.StartsWith('.'))
                .WithMessage("Email cannot start with a dot")
                .Must(email => !email.EndsWith('.'))
                .WithMessage("Email cannot end with a dot");
        });

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

        When(x => x.FirstName != null, () =>
        {
            RuleFor(x => x.FirstName)
                .Must(name => ValidNameRegex().IsMatch(name))
                .WithMessage("FirstName can only contain letters, spaces, hyphens, and apostrophes");
        });

        RuleFor(x => x.LastName)
            .NotNull().WithMessage("LastName cannot be null")
            .NotEmpty().WithMessage("LastName cannot be empty")
            .MaximumLength(UserEntityConstraints.MaxNameLength)
            .WithMessage($"LastName cannot be longer than {UserEntityConstraints.MaxNameLength} characters");

        When(x => x.LastName != null, () =>
        {
            RuleFor(x => x.LastName)
                .Must(name => ValidNameRegex().IsMatch(name))
                .WithMessage("LastName can only contain letters, spaces, hyphens, and apostrophes");
        });
    }

    /// <summary>
    ///     Compiled regex for validating names (letters, spaces, hyphens, apostrophes only).
    /// </summary>
    [GeneratedRegex(@"^[a-zA-Z\s'\-]+$", RegexOptions.None, 1000)]
    private static partial Regex ValidNameRegex();
}
