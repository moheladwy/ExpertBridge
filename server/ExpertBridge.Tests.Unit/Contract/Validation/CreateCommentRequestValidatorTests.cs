// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for CreateCommentRequestValidator covering XSS prevention,
///     Either/Or validation for PostId/JobPostingId, and optional field validation.
/// </summary>
/// <remarks>
///     Tests validate: Content (required), Either PostId OR JobPostingId (required),
///     ParentCommentId (optional), XSS prevention, dangerous patterns, and ID length limits.
/// </remarks>
public sealed class CreateCommentRequestValidatorTests
{
    private readonly CreateCommentRequestValidator _validator;

    public CreateCommentRequestValidatorTests()
    {
        _validator = new CreateCommentRequestValidator();
    }

    #region Happy Path Tests

    [Fact]
    public async Task Should_Pass_When_All_Required_Fields_Valid_With_PostId()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "This is a valid comment without any dangerous patterns.",
            PostId = "valid-post-id-123",
            JobPostingId = null,
            ParentCommentId = null
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_When_All_Required_Fields_Valid_With_JobPostingId()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "This is a valid comment on a job posting.",
            PostId = null,
            JobPostingId = "valid-job-posting-id-456",
            ParentCommentId = null
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_When_ParentCommentId_Provided()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "This is a nested reply comment.",
            PostId = "valid-post-id",
            ParentCommentId = "parent-comment-id-789"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_When_Content_At_Max_Length()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = new string('a', 5000), // Max length is 5000
            PostId = "valid-post-id"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public async Task Should_Pass_When_PostId_At_Max_Length()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content", PostId = new string('a', 450) // Max ID length is 450
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PostId);
    }

    [Fact]
    public async Task Should_Pass_When_JobPostingId_At_Max_Length()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content", JobPostingId = new string('a', 450) // Max ID length is 450
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.JobPostingId);
    }

    [Fact]
    public async Task Should_Pass_When_ParentCommentId_At_Max_Length()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content",
            PostId = "valid-post-id",
            ParentCommentId = new string('a', 450) // Max ID length is 450
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ParentCommentId);
    }

    #endregion

    #region Content Validation Tests

    [Fact]
    public async Task Should_Have_Error_When_Content_Is_Null()
    {
        // Arrange
        var request = new CreateCommentRequest { Content = null!, PostId = "valid-post-id" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("Content cannot be null");
    }

    [Fact]
    public async Task Should_Have_Error_When_Content_Is_Empty()
    {
        // Arrange
        var request = new CreateCommentRequest { Content = string.Empty, PostId = "valid-post-id" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("Content cannot be empty");
    }

    [Fact]
    public async Task Should_Have_Error_When_Content_Is_Whitespace()
    {
        // Arrange
        var request = new CreateCommentRequest { Content = "   ", PostId = "valid-post-id" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("Content cannot be empty");
    }

    [Fact]
    public async Task Should_Have_Error_When_Content_Exceeds_Max_Length()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = new string('a', 5001), // Max is 5000
            PostId = "valid-post-id"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("Content cannot be longer than 5000 characters");
    }

    [Fact]
    public async Task Should_Have_Error_When_Content_Contains_Script_Tags()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content but has <script>alert('xss')</script> in it", PostId = "valid-post-id"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("Content cannot contain script tags");
    }

    [Fact]
    public async Task Should_Have_Error_When_Content_Contains_Script_Tags_Multiline()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = @"Some content
                <script>
                    var x = 'malicious';
                    doSomethingBad();
                </script>
                More content",
            PostId = "valid-post-id"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("Content cannot contain script tags");
    }

    [Fact]
    public async Task Should_Have_Error_When_Content_Contains_Javascript_Protocol()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Click here: javascript:alert('xss')", PostId = "valid-post-id"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage(
                "Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");
    }

    [Fact]
    public async Task Should_Have_Error_When_Content_Contains_Data_Protocol()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Image: data:text/html,<script>alert('xss')</script>", PostId = "valid-post-id"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage(
                "Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");
    }

    [Theory]
    [InlineData("onclick=alert('xss')")]
    [InlineData("onload=malicious()")]
    [InlineData("onerror=badFunction()")]
    [InlineData("onmouseover=hack()")]
    public async Task Should_Have_Error_When_Content_Contains_Event_Handlers(string eventHandler)
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = $"Content with {eventHandler} event handler", PostId = "valid-post-id"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage(
                "Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");
    }

    [Fact]
    public async Task Should_Detect_Case_Insensitive_Script_Tags()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Content with <SCRIPT>alert('xss')</SCRIPT>", PostId = "valid-post-id"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public async Task Should_Detect_Case_Insensitive_Javascript_Protocol()
    {
        // Arrange
        var request = new CreateCommentRequest { Content = "Link: JAVASCRIPT:alert('xss')", PostId = "valid-post-id" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    #endregion

    #region PostId/JobPostingId Either/Or Validation Tests

    [Fact]
    public async Task Should_Have_Error_When_Both_PostId_And_JobPostingId_Are_Null()
    {
        // Arrange
        var request = new CreateCommentRequest { Content = "Valid content", PostId = null, JobPostingId = null };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Either PostId or JobPostingId must be provided");
    }

    [Fact]
    public async Task Should_Have_Error_When_Both_PostId_And_JobPostingId_Are_Empty()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content", PostId = string.Empty, JobPostingId = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Either PostId or JobPostingId must be provided");
    }

    [Fact]
    public async Task Should_Pass_When_Only_PostId_Is_Provided()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content", PostId = "valid-post-id", JobPostingId = null
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public async Task Should_Pass_When_Only_JobPostingId_Is_Provided()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content", PostId = null, JobPostingId = "valid-job-posting-id"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public async Task Should_Pass_When_Both_PostId_And_JobPostingId_Are_Provided()
    {
        // Arrange - The validator allows both to be set (business logic might prevent this elsewhere)
        var request = new CreateCommentRequest
        {
            Content = "Valid content", PostId = "valid-post-id", JobPostingId = "valid-job-posting-id"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    #endregion

    #region PostId Validation Tests

    [Fact]
    public async Task Should_Have_Error_When_PostId_Is_Empty_String()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content",
            PostId = string.Empty,
            JobPostingId = "valid-job-posting-id" // Provide the other ID to satisfy Either/Or rule
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PostId)
            .WithErrorMessage("PostId cannot be empty when provided");
    }

    [Fact]
    public async Task Should_Have_Error_When_PostId_Exceeds_Max_Length()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content", PostId = new string('a', 451) // Max is 450
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PostId)
            .WithErrorMessage("PostId cannot be longer than 450 characters");
    }

    [Fact]
    public async Task Should_Not_Validate_PostId_When_Null()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content", PostId = null, JobPostingId = "valid-job-posting-id"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PostId);
    }

    #endregion

    #region JobPostingId Validation Tests

    [Fact]
    public async Task Should_Have_Error_When_JobPostingId_Is_Empty_String()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content",
            PostId = "valid-post-id", // Provide the other ID to satisfy Either/Or rule
            JobPostingId = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.JobPostingId)
            .WithErrorMessage("JobPostingId cannot be empty when provided");
    }

    [Fact]
    public async Task Should_Have_Error_When_JobPostingId_Exceeds_Max_Length()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content", JobPostingId = new string('a', 451) // Max is 450
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.JobPostingId)
            .WithErrorMessage("JobPostingId cannot be longer than 450 characters");
    }

    [Fact]
    public async Task Should_Not_Validate_JobPostingId_When_Null()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content", PostId = "valid-post-id", JobPostingId = null
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.JobPostingId);
    }

    #endregion

    #region ParentCommentId Validation Tests

    [Fact]
    public async Task Should_Have_Error_When_ParentCommentId_Is_Empty_String()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content", PostId = "valid-post-id", ParentCommentId = string.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ParentCommentId)
            .WithErrorMessage("ParentCommentId cannot be empty when provided");
    }

    [Fact]
    public async Task Should_Have_Error_When_ParentCommentId_Exceeds_Max_Length()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content", PostId = "valid-post-id", ParentCommentId = new string('a', 451) // Max is 450
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ParentCommentId)
            .WithErrorMessage("ParentCommentId cannot be longer than 450 characters");
    }

    [Fact]
    public async Task Should_Not_Validate_ParentCommentId_When_Null()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Valid content", PostId = "valid-post-id", ParentCommentId = null
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ParentCommentId);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task Should_Pass_When_Content_Has_Valid_Special_Characters()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Content with émojis 🎉, line breaks\nand special characters: @#$%^&*()",
            PostId = "valid-post-id"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public async Task Should_Have_Multiple_Errors_When_Multiple_Fields_Invalid()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "<script>alert('xss')</script>", // Script tag
            PostId = string.Empty, // Empty
            JobPostingId = string.Empty, // Empty
            ParentCommentId = new string('a', 451) // Too long
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content);
        result.ShouldHaveValidationErrorFor(x => x);
        result.ShouldHaveValidationErrorFor(x => x.ParentCommentId);
    }

    [Fact]
    public async Task Should_Pass_When_Content_Contains_Safe_HTML_Entities()
    {
        // Arrange
        var request = new CreateCommentRequest
        {
            Content = "Content with &lt;safe&gt; HTML entities and &amp; symbols", PostId = "valid-post-id"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Content);
    }

    #endregion
}
