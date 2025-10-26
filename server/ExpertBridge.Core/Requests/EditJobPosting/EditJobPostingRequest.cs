// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests.EditJobPosting;

/// <summary>
/// Represents a request to edit an existing job posting.
/// </summary>
/// <remarks>
/// All properties are optional. Only provided properties will be updated.
/// </remarks>
public class EditJobPostingRequest
{
    /// <summary>
    /// Gets or sets the new title for the job posting.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the new detailed description for the job.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the new budget for the job.
    /// </summary>
    public decimal? Budget { get; set; }

    /// <summary>
    /// Gets or sets the new geographical area or work location for the job.
    /// </summary>
    public string? Area { get; set; }
}
