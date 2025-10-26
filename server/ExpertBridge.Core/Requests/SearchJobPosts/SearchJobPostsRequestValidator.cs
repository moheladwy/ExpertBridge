// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Requests.SearchJobPosts;

/// <summary>
///     Validates SearchJobPostsRequest to ensure search query and filters are valid.
/// </summary>
/// <remarks>
///     Validates Query, Limit, and budget range constraints for job posting search.
/// </remarks>
public class SearchJobPostsRequestValidator : AbstractValidator<SearchJobPostsRequest>
{
    /// <summary>
    ///     Initializes a new instance of the SearchJobPostsRequestValidator with validation rules.
    /// </summary>
    public SearchJobPostsRequestValidator()
    {
        RuleFor(x => x.Query)
            .NotNull().WithMessage("Query cannot be null")
            .NotEmpty().WithMessage("Query cannot be empty")
            .MinimumLength(2).WithMessage("Query must be at least 2 characters long")
            .MaximumLength(200).WithMessage("Query cannot exceed 200 characters");

        When(x => x.Limit.HasValue, () =>
        {
            RuleFor(x => x.Limit)
                .GreaterThan(0).WithMessage("Limit must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Limit cannot exceed 100");
        });

        When(x => x.MinBudget.HasValue, () =>
        {
            RuleFor(x => x.MinBudget)
                .GreaterThanOrEqualTo(0).WithMessage("MinBudget must be greater than or equal to 0")
                .LessThanOrEqualTo(1000000m).WithMessage("MinBudget cannot exceed 1,000,000");
        });

        When(x => x.MaxBudget.HasValue, () =>
        {
            RuleFor(x => x.MaxBudget)
                .GreaterThanOrEqualTo(0).WithMessage("MaxBudget must be greater than or equal to 0")
                .LessThanOrEqualTo(1000000m).WithMessage("MaxBudget cannot exceed 1,000,000");
        });

        When(x => x.MinBudget.HasValue && x.MaxBudget.HasValue, () =>
        {
            RuleFor(x => x)
                .Must(x => x.MinBudget <= x.MaxBudget)
                .WithMessage("MinBudget cannot be greater than MaxBudget");
        });
    }
}
