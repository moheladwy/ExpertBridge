// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.Users;
using FluentValidation;

namespace ExpertBridge.Contract.Requests.UpdateUserRequest;

/// <summary>
///     Validates UpdateUserRequest to ensure all user update fields meet constraints.
/// </summary>
/// <remarks>
///     Validates ProviderId, Email (both required), and optional FirstName, LastName, and PhoneNumber
///     against entity constraints when provided.
///     Phone number validation includes E.164 format checking.
///     Name validation prevents special characters and script injection.
/// </remarks>
public partial class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    /// <summary>
    ///     Initializes a new instance of the UpdateUserRequestValidator with validation rules.
    /// </summary>
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.ProviderId)
            .NotNull().WithMessage("Provider ID cannot be null")
            .NotEmpty().WithMessage("Provider ID cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"Provider ID cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");

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

        When(x => x.FirstName != null, () =>
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name cannot be empty when provided")
                .MaximumLength(UserEntityConstraints.MaxNameLength)
                .WithMessage($"First name cannot be longer than {UserEntityConstraints.MaxNameLength} characters")
                .Must(name => ValidNameRegex().IsMatch(name ?? string.Empty))
                .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");
        });

        When(x => x.LastName != null, () =>
        {
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name cannot be empty when provided")
                .MaximumLength(UserEntityConstraints.MaxNameLength)
                .WithMessage($"Last name cannot be longer than {UserEntityConstraints.MaxNameLength} characters")
                .Must(name => ValidNameRegex().IsMatch(name ?? string.Empty))
                .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");
        });

        When(x => x.PhoneNumber != null, () =>
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("PhoneNumber cannot be empty when provided")
                .MaximumLength(UserEntityConstraints.MaxPhoneNumberLength)
                .WithMessage(
                    $"PhoneNumber cannot be longer than {UserEntityConstraints.MaxPhoneNumberLength} characters")
                .Must(phone => E164PhoneRegex().IsMatch(phone ?? string.Empty))
                .WithMessage("PhoneNumber must be in E.164 format (+[country code][number], 10-15 digits)");
        });
    }

    /// <summary>
    ///     Compiled regex for validating names (letters, spaces, hyphens, apostrophes only).
    /// </summary>
    [GeneratedRegex(@"^[a-zA-Z\s'\-]+$", RegexOptions.None, 1000)]
    private static partial Regex ValidNameRegex();

    /// <summary>
    ///     Compiled regex for E.164 phone number format validation.
    /// </summary>
    [GeneratedRegex(@"^\+[1-9]\d{9,14}$", RegexOptions.None, 1000)]
    private static partial Regex E164PhoneRegex();
}
