// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for EditCommentRequestValidator covering XSS prevention
///     and optional field validation for comment editing.
/// </summary>
/// <remarks>
///     Tests validate: Optional Content field with XSS prevention,
///     dangerous patterns, length limits, and conditional validation.
///     Content is optional - validation only applies when field is provided.
/// </remarks>
public sealed class EditCommentRequestValidatorTests
{
  private readonly EditCommentRequestValidator _validator;

  public EditCommentRequestValidatorTests()
  {
    _validator = new EditCommentRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_When_Content_Is_Null()
  {
    // Arrange
    var request = new EditCommentRequest
    {
      Content = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Content_Is_Valid()
  {
    // Arrange
    var request = new EditCommentRequest
    {
      Content = "This is valid updated content without any dangerous patterns."
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
    var request = new EditCommentRequest
    {
      Content = new string('a', 5000) // Max length is 5000
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Content);
  }

  #endregion

  #region Content Validation Tests

  [Fact]
  public async Task Should_Have_Error_When_Content_Is_Empty_String()
  {
    // Arrange
    var request = new EditCommentRequest
    {
      Content = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Have_Error_When_Content_Is_Whitespace()
  {
    // Arrange
    var request = new EditCommentRequest
    {
      Content = "   "
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot be empty when provided");
  }

  [Fact]
  public async Task Should_Have_Error_When_Content_Exceeds_Max_Length()
  {
    // Arrange
    var request = new EditCommentRequest
    {
      Content = new string('a', 5001) // Max is 5000
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
    var request = new EditCommentRequest
    {
      Content = "Valid content but has <script>alert('xss')</script> in it"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot contain script tags");
  }

  [Fact]
  public async Task Should_Have_Error_When_Content_Contains_Script_Tags_With_Attributes()
  {
    // Arrange
    var request = new EditCommentRequest
    {
      Content = "Content <script type='text/javascript'>malicious()</script>"
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
    var request = new EditCommentRequest
    {
      Content = @"Some content
                <script>
                    var x = 'malicious';
                    doSomethingBad();
                </script>
                More content"
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
    var request = new EditCommentRequest
    {
      Content = "Click here: javascript:alert('xss')"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");
  }

  [Fact]
  public async Task Should_Have_Error_When_Content_Contains_Data_Protocol()
  {
    // Arrange
    var request = new EditCommentRequest
    {
      Content = "Image: data:text/html,<script>alert('xss')</script>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");
  }

  [Theory]
  [InlineData("onclick=alert('xss')")]
  [InlineData("onload=malicious()")]
  [InlineData("onerror=badFunction()")]
  [InlineData("onmouseover=hack()")]
  [InlineData("onfocus=exploit()")]
  public async Task Should_Have_Error_When_Content_Contains_Event_Handlers(string eventHandler)
  {
    // Arrange
    var request = new EditCommentRequest
    {
      Content = $"Content with {eventHandler} event handler"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");
  }

  [Fact]
  public async Task Should_Detect_Case_Insensitive_Script_Tags()
  {
    // Arrange
    var request = new EditCommentRequest
    {
      Content = "Content with <SCRIPT>alert('xss')</SCRIPT>"
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
    var request = new EditCommentRequest
    {
      Content = "Link: JAVASCRIPT:alert('xss')"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content);
  }

  #endregion

  #region Conditional Validation Tests

  [Fact]
  public async Task Should_Not_Validate_Content_When_Null()
  {
    // Arrange
    var request = new EditCommentRequest
    {
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
  public async Task Should_Pass_When_Content_Has_Valid_Special_Characters()
  {
    // Arrange
    var request = new EditCommentRequest
    {
      Content = "Content with émojis 🎉, line breaks\nand special characters: @#$%^&*()"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Content);
  }

  [Fact]
  public async Task Should_Pass_When_Content_Contains_Safe_HTML_Entities()
  {
    // Arrange
    var request = new EditCommentRequest
    {
      Content = "Content with &lt;safe&gt; HTML entities and &amp; symbols"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Content);
  }

  [Fact]
  public async Task Should_Pass_When_Content_Has_Markdown_Formatting()
  {
    // Arrange
    var request = new EditCommentRequest
    {
      Content = "Content with **bold** and *italic* markdown formatting"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Content);
  }

  [Fact]
  public async Task Should_Pass_When_Content_Has_URLs()
  {
    // Arrange
    var request = new EditCommentRequest
    {
      Content = "Check out https://example.com for more info"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Content);
  }

  [Fact]
  public async Task Should_Have_Error_When_Content_Has_Mixed_Dangerous_Patterns()
  {
    // Arrange
    var request = new EditCommentRequest
    {
      Content = "<script>alert('xss')</script> and javascript:void(0)"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content);
  }

  #endregion
}
