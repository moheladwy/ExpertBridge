// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Contract.Requests.MediaObject;

/// <summary>
///     Validates MediaObjectRequest for file uploads.
/// </summary>
public class MediaObjectRequestValidator : AbstractValidator<MediaObjectRequest>
{
    private static readonly string[] AllowedTypes =
    {
        "image/jpeg", "image/png", "image/gif", "image/webp", "video/mp4", "video/webm", "application/pdf"
    };

    public MediaObjectRequestValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Media type is required")
            .Must(BeAllowedMediaType).WithMessage($"Media type must be one of: {string.Join(", ", AllowedTypes)}");

        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("Media key is required")
            .MaximumLength(500).WithMessage("Media key cannot exceed 500 characters")
            .Must(BeSafeFileName).WithMessage("Media key contains invalid characters");
    }

    private static bool BeAllowedMediaType(string? type)
    {
        if (string.IsNullOrEmpty(type))
        {
            return false;
        }

        return AllowedTypes.Contains(type, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeSafeFileName(string? key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        // Prevent path traversal attacks
        return !key.Contains("..", StringComparison.Ordinal) &&
               !key.Contains('\\', StringComparison.Ordinal) &&
               !key.StartsWith('/');
    }
}
