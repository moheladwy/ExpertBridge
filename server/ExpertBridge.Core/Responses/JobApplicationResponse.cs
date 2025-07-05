// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.Profiles;

namespace ExpertBridge.Core.Responses
{
    public class JobApplicationResponse
    {
        public required string Id { get; set; }
        public required string JobPostingId { get; set; }
        public required string ApplicantId { get; set; }
        public string? CoverLetter { get; set; }
        public required decimal OfferedCost { get; set; }
        public DateTime AppliedAt { get; set; }
        public ApplicantResponse? Applicant { get; set; }
    }
}
