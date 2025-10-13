// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobOffers;
using ExpertBridge.Core.Responses;

namespace ExpertBridge.Core.Queries;

public static class JobOfferQueries
{
    public static IQueryable<JobOfferResponse> SelectJobOfferResponseFromEntity(this IQueryable<JobOffer> query) =>
        query
            .Select(j => SelectJobOfferResponseFromEntity(j));

    public static JobOfferResponse SelectJobOfferResponseFromEntity(this JobOffer j) =>
        new()
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
