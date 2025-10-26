// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Requests.SearchPost;

/// <summary>
///     Validates SearchPostRequest to ensure search query is valid.
/// </summary>
/// <remarks>
///     Validates that Query field is properly provided and Limit is within acceptable range.
/// </remarks>
public class SearchPostRequestValidator : AbstractValidator<SearchPostRequest>
{
    /// <summary>
    ///     Initializes a new instance of the SearchPostRequestValidator with validation rules.
    /// </summary>
    public SearchPostRequestValidator()
    {
        RuleFor(x => x.Query)
            .NotNull().WithMessage("Query cannot be null")
            .NotEmpty().WithMessage("Query cannot be empty");

        When(x => x.Limit.HasValue, () =>
        {
            RuleFor(x => x.Limit)
                .GreaterThan(0).WithMessage("Limit must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Limit cannot exceed 100");
        });
    }
}
