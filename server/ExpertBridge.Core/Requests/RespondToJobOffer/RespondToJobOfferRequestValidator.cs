// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Requests.RespondToJobOffer;

/// <summary>
///     Validates RespondToJobOfferRequest to ensure job offer response is valid.
/// </summary>
/// <remarks>
///     Validates that the Accept field is provided for job offer acceptance or decline.
/// </remarks>
public class RespondToJobOfferRequestValidator : AbstractValidator<RespondToJobOfferRequest>
{
    /// <summary>
    ///     Initializes a new instance of the RespondToJobOfferRequestValidator with validation rules.
    /// </summary>
    public RespondToJobOfferRequestValidator()
    {
        RuleFor(x => x.Accept)
            .NotNull().WithMessage("Accept field cannot be null");
    }
}
