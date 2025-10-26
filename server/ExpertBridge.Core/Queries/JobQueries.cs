// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Responses;

namespace ExpertBridge.Core.Queries;

/// <summary>
/// Provides extension methods for querying and projecting Job entities.
/// </summary>
/// <remarks>
/// These query extensions project active job contracts to response DTOs with author and worker information.
/// </remarks>
public static class JobQueries
{
    /// <summary>
    /// Projects a queryable of Job entities to JobResponse DTOs with author and worker profiles.
    /// </summary>
    /// <param name="query">The source queryable of jobs.</param>
    /// <returns>A queryable of JobResponse objects with populated profile information.</returns>
    public static IQueryable<JobResponse> SelectJobResponseFromFullJob(this IQueryable<Job> query)
    {
        return query
            .Select(j => SelectJobResponseFromFullJob(j));
    }

    /// <summary>
    /// Projects a single Job entity to a JobResponse DTO with author and worker profiles.
    /// </summary>
    /// <param name="j">The job entity to project.</param>
    /// <returns>A JobResponse object with all job contract details and participant profiles.</returns>
    public static JobResponse SelectJobResponseFromFullJob(this Job j)
    {
        return new JobResponse
        {
            Id = j.Id,
            Description = j.Description,
            Title = j.Title,
            ActualCost = j.ActualCost,
            StartedAt = j.StartedAt,
            Area = j.Area,
            AuthorId = j.AuthorId,
            ChatId = j.ChatId,
            IsCompleted = j.IsCompleted,
            EndedAt = j.EndedAt,
            IsPaid = j.IsPaid,
            UpdatedAt = j.UpdatedAt,
            WorkerId = j.WorkerId,
            Author = j.Author.SelectAuthorResponseFromProfile(),
            Worker = j.Worker.SelectAuthorResponseFromProfile()
        };
    }
}
