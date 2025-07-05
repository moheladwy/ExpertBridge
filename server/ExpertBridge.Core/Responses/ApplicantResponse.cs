// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertBridge.Core.Responses
{
    public class ApplicantResponse
    {
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? JobTitle { get; set; }
        public int Reputation { get; set; }
        public int JobsDone { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
