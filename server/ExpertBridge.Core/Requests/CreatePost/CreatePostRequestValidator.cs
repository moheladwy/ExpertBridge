// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace ExpertBridge.Core.Requests.CreatePost;

/// <summary>
/// Validates CreatePostRequest to ensure post creation data meets requirements.
/// </summary>
/// <remarks>
/// This validator is currently a placeholder for future validation rules.
/// Consider adding validation for Title, Content length, and Media attachments.
/// </remarks>
public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
}
