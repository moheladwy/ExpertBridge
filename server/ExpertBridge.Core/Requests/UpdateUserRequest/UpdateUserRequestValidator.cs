// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.Users;
using FluentValidation;

namespace ExpertBridge.Core.Requests.UpdateUserRequest;

/// <summary>
/// Validates UpdateUserRequest to ensure all user update fields meet constraints.
/// </summary>
/// <remarks>
/// Validates ProviderId, Email (both required), and optional FirstName, LastName, and PhoneNumber
/// against entity constraints when provided.
/// Phone number validation includes format checking with regex pattern for international numbers.
/// </remarks>
public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    /// <summary>
    /// Initializes a new instance of the UpdateUserRequestValidator with validation rules.
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
            .WithMessage($"Email cannot be longer than {UserEntityConstraints.MaxEmailLength} characters")
            .EmailAddress().WithMessage("Email must be a valid email address");

        When(x => x.FirstName != null, () =>
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name cannot be empty when provided")
                .MaximumLength(UserEntityConstraints.MaxNameLength)
                .WithMessage($"First name cannot be longer than {UserEntityConstraints.MaxNameLength} characters");
        });

        When(x => x.LastName != null, () =>
        {
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name cannot be empty when provided")
                .MaximumLength(UserEntityConstraints.MaxNameLength)
                .WithMessage($"Last name cannot be longer than {UserEntityConstraints.MaxNameLength} characters");
        });

        When(x => x.PhoneNumber != null, () =>
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("PhoneNumber cannot be empty when provided")
                .MaximumLength(UserEntityConstraints.MaxPhoneNumberLength)
                .WithMessage($"PhoneNumber cannot be longer than {UserEntityConstraints.MaxPhoneNumberLength} characters")
                .Matches(@"^\+?[0-9]\d{9,17}$").WithMessage("PhoneNumber is not valid");
        });
    }
}
