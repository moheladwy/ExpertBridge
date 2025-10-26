// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Requests.GeneratePresignedUrls;

/// <summary>
/// Validates GeneratePresignedUrlsRequest to ensure file metadata is valid.
/// </summary>
/// <remarks>
/// Validates that Files collection is provided and contains valid file metadata.
/// </remarks>
public class GeneratePresignedUrlsRequestValidator : AbstractValidator<GeneratePresignedUrlsRequest>
{
  /// <summary>
  /// Initializes a new instance of the GeneratePresignedUrlsRequestValidator with validation rules.
  /// </summary>
  public GeneratePresignedUrlsRequestValidator()
  {
    RuleFor(x => x.Files)
        .NotNull().WithMessage("Files cannot be null")
        .NotEmpty().WithMessage("At least one file must be provided");

    RuleForEach(x => x.Files)
        .ChildRules(file =>
        {
          file.RuleFor(f => f.ContentType)
                  .NotNull().WithMessage("ContentType cannot be null")
                  .NotEmpty().WithMessage("ContentType cannot be empty");

          file.RuleFor(f => f.Type)
                  .NotNull().WithMessage("Type cannot be null")
                  .NotEmpty().WithMessage("Type cannot be empty");

          file.RuleFor(f => f.Name)
                  .NotNull().WithMessage("Name cannot be null")
                  .NotEmpty().WithMessage("Name cannot be empty");

          file.RuleFor(f => f.Size)
                  .GreaterThan(0).WithMessage("Size must be greater than 0");

          file.RuleFor(f => f.Extension)
                  .NotNull().WithMessage("Extension cannot be null")
                  .NotEmpty().WithMessage("Extension cannot be empty");
        });
  }
}
