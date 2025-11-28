// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Contract.Responses;

/// <summary>
///     Represents the response DTO for media attachment information.
/// </summary>
/// <remarks>
///     Media objects can be attached to posts, comments, and job postings.
///     Supported types include images, videos, and documents.
/// </remarks>
public sealed class MediaObjectResponse
{
    /// <summary>
    ///     Gets or sets the unique identifier of the media object.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///     Gets or sets the name or filename of the media object.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the MIME type of the media object.
    /// </summary>
    /// <remarks>
    ///     Examples include "image/jpeg", "video/mp4", "application/pdf", etc.
    /// </remarks>
    public required string Type { get; set; }

    /// <summary>
    ///     Gets or sets the URL where the media object can be accessed.
    /// </summary>
    public required string Url { get; set; }
}
