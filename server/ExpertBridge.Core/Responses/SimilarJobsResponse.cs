// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Responses;

public class SimilarJobsResponse
{
    public string JobPostingId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string AuthorName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public decimal Budget { get; set; }
    public string Area { get; set; }
    public double RelevanceScore { get; set; }
}
