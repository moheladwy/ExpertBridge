// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using FluentValidation;

namespace ExpertBridge.Core.Requests.CreateJobOffer;

/// <summary>
///     Validates CreateJobOfferRequest to ensure all required job offer fields meet constraints.
/// </summary>
/// <remarks>
///     Validates Title, Description, Area, Budget, and WorkerId against entity constraints
///     from JobOffer entity to ensure data integrity during direct job offer creation.
/// </remarks>
public class CreateJobOfferRequestValidator : AbstractValidator<CreateJobOfferRequest>
{
    /// <summary>
    ///     Initializes a new instance of the CreateJobOfferRequestValidator with validation rules.
    /// </summary>
    public CreateJobOfferRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotNull().WithMessage("Title cannot be null")
            .NotEmpty().WithMessage("Title cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxTitleLength)
            .WithMessage($"Title cannot be longer than {GlobalEntitiesConstraints.MaxTitleLength} characters");

        RuleFor(x => x.Description)
            .NotNull().WithMessage("Description cannot be null")
            .NotEmpty().WithMessage("Description cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxDescriptionLength)
            .WithMessage(
                $"Description cannot be longer than {GlobalEntitiesConstraints.MaxDescriptionLength} characters");

        RuleFor(x => x.Area)
            .NotNull().WithMessage("Area cannot be null")
            .NotEmpty().WithMessage("Area cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"Area cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");

        RuleFor(x => x.Budget)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Budget must be greater than or equal to 0");

        RuleFor(x => x.WorkerId)
            .NotNull().WithMessage("WorkerId cannot be null")
            .NotEmpty().WithMessage("WorkerId cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage($"WorkerId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
    }
}
