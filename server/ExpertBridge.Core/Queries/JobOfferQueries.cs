// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobOffers;
using ExpertBridge.Core.Responses;

namespace ExpertBridge.Core.Queries;

/// <summary>
/// Provides extension methods for querying and projecting JobOffer entities.
/// </summary>
/// <remarks>
/// These query extensions project direct job offers to response DTOs with author and worker profiles.
/// </remarks>
public static class JobOfferQueries
{
    /// <summary>
    /// Projects a queryable of JobOffer entities to JobOfferResponse DTOs with author and worker profiles.
    /// </summary>
    /// <param name="query">The source queryable of job offers.</param>
    /// <returns>A queryable of JobOfferResponse objects with populated profile information.</returns>
    public static IQueryable<JobOfferResponse> SelectJobOfferResponseFromEntity(this IQueryable<JobOffer> query)
    {
        return query
            .Select(j => SelectJobOfferResponseFromEntity(j));
    }

    /// <summary>
    /// Projects a single JobOffer entity to a JobOfferResponse DTO with author and worker profiles.
    /// </summary>
    /// <param name="j">The job offer entity to project.</param>
    /// <returns>A JobOfferResponse object with all offer details and participant profiles.</returns>
    public static JobOfferResponse SelectJobOfferResponseFromEntity(this JobOffer j)
    {
        return new JobOfferResponse
        {
            Id = j.Id,
            Description = j.Description,
            Title = j.Title,
            Budget = j.Budget,
            CreatedAt = j.CreatedAt,
            Area = j.Area,
            Author = j.Author.SelectAuthorResponseFromProfile(),
            Worker = j.Worker.SelectAuthorResponseFromProfile()
        };
    }
}
