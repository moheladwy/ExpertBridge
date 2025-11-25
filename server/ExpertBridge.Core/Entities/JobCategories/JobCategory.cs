// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.JobPostings;

namespace ExpertBridge.Core.Entities.JobCategories;

/// <summary>
///     Represents a category for classifying job postings by industry or job type.
/// </summary>
/// <remarks>
///     Job categories help organize and filter job postings, making it easier for workers to find relevant opportunities.
/// </remarks>
public sealed class JobCategory
{
    /// <summary>
    ///     Gets or sets the unique identifier for the job category.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    ///     Gets or sets the name of the job category.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets the description of the job category.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets the collection of job postings in this category.
    /// </summary>
    public ICollection<JobPosting> JobPostings { get; set; } = [];
}
