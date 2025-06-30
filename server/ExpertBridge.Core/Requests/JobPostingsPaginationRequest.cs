// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertBridge.Core.Requests
{
    public class JobPostingsPaginationRequest
    {
        public int PageSize { get; set; }
        public int Page { get; set; }
        public double? After { get; set; }
        public string? LastIdCursor { get; set; }
        public string? Embedding { get; set; }
    }
}
