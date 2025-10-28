// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Comments;

namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for PatchCommentRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: CommentId validation, optional voting flags (Upvote/Downvote),
///     and optional Content validation with XSS prevention.
/// </remarks>
public sealed class PatchCommentRequestValidatorTests
{
  private readonly PatchCommentRequestValidator _validator;

  public PatchCommentRequestValidatorTests()
  {
    _validator = new PatchCommentRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_With_Valid_CommentId_Only()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Upvote_True()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = true,
      Downvote = null,
      Content = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Upvote_False()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = false,
      Downvote = null,
      Content = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Downvote_True()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = true,
      Content = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Downvote_False()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = false,
      Content = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Both_Voting_Flags()
  {
    // Arrange - Both flags can be set (business logic may handle contradictions)
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = true,
      Downvote = true,
      Content = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Valid_Content()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "This is an updated comment with valid content."
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Content_And_Voting()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = true,
      Downvote = false,
      Content = "Updated content with upvote."
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Content_At_Max_Length()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = new string('a', CommentEntityConstraints.MaxContentLength) // Exactly 5000 characters
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Long_Valid_CommentId()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = new string('a', GlobalEntitiesConstraints.MaxIdLength), // Exactly 450 characters
      Upvote = null,
      Downvote = null,
      Content = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region CommentId Validation Tests

  [Fact]
  public async Task Should_Fail_When_CommentId_Is_Null()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = null!,
      Upvote = null,
      Downvote = null,
      Content = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CommentId)
        .WithErrorMessage("CommentId cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_CommentId_Is_Empty()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = string.Empty,
      Upvote = null,
      Downvote = null,
      Content = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CommentId)
        .WithErrorMessage("CommentId cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_CommentId_Exceeds_MaxLength()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1), // 451 characters
      Upvote = null,
      Downvote = null,
      Content = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CommentId)
        .WithErrorMessage($"CommentId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
  }

  #endregion

  #region Content Validation Tests - Length

  [Fact]
  public async Task Should_Pass_When_Content_Is_Null()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Content);
  }

  [Fact]
  public async Task Should_Fail_When_Content_Is_Empty_String()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Exceeds_MaxLength()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = new string('a', CommentEntityConstraints.MaxContentLength + 1) // 5001 characters
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage($"Content cannot be longer than {CommentEntityConstraints.MaxContentLength} characters");
  }

  #endregion

  #region Content Validation Tests - XSS Prevention

  [Fact]
  public async Task Should_Fail_When_Content_Contains_Script_Tag()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "Hello <script>alert('xss')</script> world"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot contain script tags");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Contains_Script_Tag_With_Uppercase()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "Hello <SCRIPT>alert('xss')</SCRIPT> world"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot contain script tags");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Contains_Script_Tag_With_Attributes()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "Hello <script type='text/javascript'>alert('xss')</script> world"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot contain script tags");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Contains_Javascript_Protocol()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "Click here: javascript:alert('xss')"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Contains_Data_Protocol()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "Link: data:text/html,<script>alert('xss')</script>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Contains_Onclick_Handler()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "<div onclick='alert(1)'>Click me</div>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Contains_Onload_Handler()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "<img onload='alert(1)' src='x'>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Contains_Onerror_Handler()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "<img onerror='alert(1)' src='invalid'>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Contains_Onmouseover_Handler()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "<div onmouseover='alert(1)'>Hover</div>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns");
  }

  #endregion

  #region Conditional Validation Tests

  [Fact]
  public async Task Should_Not_Validate_Content_When_Null()
  {
    // Arrange - Content is null, so XSS and length validation should not run
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Content);
  }

  #endregion

  #region Edge Cases

  [Fact]
  public async Task Should_Fail_With_Whitespace_Content()
  {
    // Arrange - Whitespace is considered empty
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "   "
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot be empty when provided");
  }
  [Fact]
  public async Task Should_Pass_With_Content_Containing_Newlines()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "First line\nSecond line\nThird line"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Content_Containing_Special_Characters()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "Special chars: !@#$%^&*()_+-=[]{}|;:',.<>?/~`"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Content_Containing_Unicode_Characters()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "Unicode: 你好 Привет مرحبا 😀"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Content_Containing_Safe_HTML()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "I love <strong>coding</strong> and <em>design</em>."
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Fail_With_Multiple_Validation_Errors()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = string.Empty, // Invalid
      Upvote = true,
      Downvote = true,
      Content = "<script>alert('xss')</script>" // Invalid
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CommentId);
    result.ShouldHaveValidationErrorFor(x => x.Content);
  }

  [Fact]
  public async Task Should_Pass_With_GUID_CommentId()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = Guid.NewGuid().ToString(),
      Upvote = null,
      Downvote = null,
      Content = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Fail_When_Content_Contains_Multiple_XSS_Patterns()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = "<script>alert('xss')</script><img onclick='alert(1)' src='x'>javascript:void(0)"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content);
  }

  [Fact]
  public async Task Should_Pass_With_All_Null_Optional_Fields()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = null,
      Downvote = null,
      Content = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_All_Optional_Fields_Populated()
  {
    // Arrange
    var request = new PatchCommentRequest
    {
      CommentId = "comment123",
      Upvote = true,
      Downvote = false,
      Content = "Updated comment content with all fields."
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion
}
