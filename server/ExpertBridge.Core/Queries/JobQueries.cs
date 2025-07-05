// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertBridge.Core.Entities.JobOffers;
using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Responses;

namespace ExpertBridge.Core.Queries
{
    public static class JobQueries
    {
        public static IQueryable<JobResponse> SelectJobResponseFromFullJob(this IQueryable<Job> query)
        {
            return query
                .Select(j => SelectJobResponseFromFullJob(j));
        }
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
                Worker = j.Worker.SelectAuthorResponseFromProfile(),
            };
        }
    }
}
