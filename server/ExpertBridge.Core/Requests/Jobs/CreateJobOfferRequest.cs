// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace ExpertBridge.Core.Requests.Jobs;

public class CreateJobOfferRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Area { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Budget must be non-negative.")]
    public decimal Budget { get; set; }

    public string WorkerId { get; set; }
}
