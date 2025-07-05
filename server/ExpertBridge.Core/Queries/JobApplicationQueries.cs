// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertBridge.Core.Entities.JobApplications;
using ExpertBridge.Core.Entities.Tags;
using ExpertBridge.Core.Responses;

namespace ExpertBridge.Core.Queries
{
    public static class JobApplicationQueries
    {
        public static IQueryable<JobApplicationResponse> SelectJobApplicationResponseFromEntity(this IQueryable<JobApplication> query)
        {
            return query
                .Select(j => SelectJobApplicationResponseFromEntity(j));
        }

        public static JobApplicationResponse SelectJobApplicationResponseFromEntity(this JobApplication j)
        {
            return new JobApplicationResponse
            {
                Applicant = j.Applicant.SelectApplicantResponseFromProfile(),
                ApplicantId = j.ApplicantId,
                JobPostingId = j.JobPostingId,
                OfferedCost = j.OfferedCost,
                AppliedAt = j.CreatedAt!.Value,
                CoverLetter = j.CoverLetter,
            };
        }
    }
}
