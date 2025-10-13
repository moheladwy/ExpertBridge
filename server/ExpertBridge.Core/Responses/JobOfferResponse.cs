// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

public class JobOfferResponse
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public decimal Budget { get; set; }
    public required string Area { get; set; }
    public DateTime? CreatedAt { get; set; }
    public AuthorResponse? Author { get; set; }
    public AuthorResponse? Worker { get; set; }
}
