// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using FluentValidation;

namespace ExpertBridge.Core.Requests.InitiateJobOffer;

/// <summary>
///     Validates InitiateJobOfferRequest to ensure all required job offer fields meet constraints.
/// </summary>
/// <remarks>
///     Validates ContractorProfileId, Title, Description, ProposedRate, and optional JobPostingId
///     against entity constraints to ensure data integrity during job offer initiation.
/// </remarks>
public class InitiateJobOfferRequestValidator : AbstractValidator<InitiateJobOfferRequest>
{
    /// <summary>
    ///     Initializes a new instance of the InitiateJobOfferRequestValidator with validation rules.
    /// </summary>
    public InitiateJobOfferRequestValidator()
    {
        RuleFor(x => x.ContractorProfileId)
            .NotNull().WithMessage("ContractorProfileId cannot be null")
            .NotEmpty().WithMessage("ContractorProfileId cannot be empty")
            .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
            .WithMessage(
                $"ContractorProfileId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");

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

        RuleFor(x => x.ProposedRate)
            .GreaterThan(0)
            .WithMessage("ProposedRate must be greater than 0");

        When(x => x.JobPostingId != null, () =>
        {
            RuleFor(x => x.JobPostingId)
                .NotEmpty().WithMessage("JobPostingId cannot be empty when provided")
                .MaximumLength(GlobalEntitiesConstraints.MaxIdLength)
                .WithMessage($"JobPostingId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
        });
    }
}
