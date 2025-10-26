// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using FluentValidation;

namespace ExpertBridge.Core.Requests.GeneratePresignedUrls;

/// <summary>
///     Validates GeneratePresignedUrlsRequest to ensure file metadata is valid.
/// </summary>
/// <remarks>
///     Validates that Files collection is provided and contains valid file metadata.
///     Includes file type validation, size limits, and path traversal prevention.
/// </remarks>
public partial class GeneratePresignedUrlsRequestValidator : AbstractValidator<GeneratePresignedUrlsRequest>
{
    private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "image/svg+xml"
    };

    private static readonly HashSet<string> AllowedVideoTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "video/mp4", "video/mpeg", "video/quicktime", "video/webm", "video/x-msvideo"
    };

    private static readonly HashSet<string> AllowedDocumentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf", "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "text/plain"
    };

    private const long MaxFileSizeBytes = 100 * 1024 * 1024; // 100 MB

    /// <summary>
    ///     Initializes a new instance of the GeneratePresignedUrlsRequestValidator with validation rules.
    /// </summary>
    public GeneratePresignedUrlsRequestValidator()
    {
        RuleFor(x => x.Files)
            .NotNull().WithMessage("Files cannot be null")
            .NotEmpty().WithMessage("At least one file must be provided")
            .Must(files => files.Count <= 10)
            .WithMessage("Cannot upload more than 10 files at once");

        RuleForEach(x => x.Files)
            .ChildRules(file =>
            {
                file.RuleFor(f => f.ContentType)
                    .NotNull().WithMessage("ContentType cannot be null")
                    .NotEmpty().WithMessage("ContentType cannot be empty")
                    .Must(IsAllowedContentType)
                    .WithMessage("ContentType is not allowed. Only images, videos, and documents are supported.");

                file.RuleFor(f => f.Type)
                    .NotNull().WithMessage("Type cannot be null")
                    .NotEmpty().WithMessage("Type cannot be empty");

                file.RuleFor(f => f.Name)
                    .NotNull().WithMessage("Name cannot be null")
                    .NotEmpty().WithMessage("Name cannot be empty")
                    .Must(name => !PathTraversalRegex().IsMatch(name))
                    .WithMessage("File name contains invalid path traversal characters");

                file.RuleFor(f => f.Size)
                    .GreaterThan(0).WithMessage("Size must be greater than 0")
                    .LessThanOrEqualTo(MaxFileSizeBytes)
                    .WithMessage($"File size cannot exceed {MaxFileSizeBytes / (1024 * 1024)} MB");

                file.RuleFor(f => f.Extension)
                    .NotNull().WithMessage("Extension cannot be null")
                    .NotEmpty().WithMessage("Extension cannot be empty")
                    .Must(ext => !PathTraversalRegex().IsMatch(ext))
                    .WithMessage("Extension contains invalid characters");
            });
    }

    /// <summary>
    ///     Validates that the content type is in the allowed list.
    /// </summary>
    private static bool IsAllowedContentType(string contentType)
    {
        return AllowedImageTypes.Contains(contentType) ||
               AllowedVideoTypes.Contains(contentType) ||
               AllowedDocumentTypes.Contains(contentType);
    }

    /// <summary>
    ///     Compiled regex for detecting path traversal patterns (../, ..\, etc.).
    /// </summary>
    [GeneratedRegex(@"(\.\.[/\\]|[<>:""|?*])", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
    private static partial Regex PathTraversalRegex();
}
