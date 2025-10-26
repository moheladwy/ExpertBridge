// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentValidation.TestHelper;

namespace ExpertBridge.Tests.Unit.Core.Validation;

/// <summary>
///     Unit tests for CreateMessageRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: ChatId validation, Content validation with XSS prevention,
///     dangerous patterns detection, length limits, and edge cases.
/// </remarks>
public sealed class CreateMessageRequestValidatorTests
{
  private readonly CreateMessageRequestValidator _validator;

  public CreateMessageRequestValidatorTests()
  {
    _validator = new CreateMessageRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_With_Valid_Request()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = "Hello, how are you?"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_ChatId_At_Max_Length()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = new string('A', GlobalEntitiesConstraints.MaxIdLength),
      Content = "Test message"
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
    var request = new CreateMessageRequest
    {
      ChatId = "chat-456",
      Content = new string('B', GlobalEntitiesConstraints.MaxContentLetterLength)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Long_Valid_Message()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-789",
      Content = "This is a longer message with multiple sentences. " +
                 "It contains various punctuation marks! Does it work? " +
                 "Yes, it should pass validation without any issues."
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region ChatId Validation Tests

  [Fact]
  public async Task Should_Fail_When_ChatId_Is_Null()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = null!,
      Content = "Test message"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ChatId)
        .WithErrorMessage("ChatId cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_ChatId_Is_Empty()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = string.Empty,
      Content = "Test message"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ChatId)
        .WithErrorMessage("ChatId cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_ChatId_Is_Whitespace()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "   ",
      Content = "Test message"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ChatId)
        .WithErrorMessage("ChatId cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_ChatId_Exceeds_Max_Length()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = new string('X', GlobalEntitiesConstraints.MaxIdLength + 1),
      Content = "Test message"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ChatId)
        .WithErrorMessage($"ChatId cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
  }

  #endregion

  #region Content Validation Tests

  [Fact]
  public async Task Should_Fail_When_Content_Is_Null()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = null!
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Is_Empty()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Is_Whitespace()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = "   "
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Content_Exceeds_Max_Length()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = new string('X', GlobalEntitiesConstraints.MaxContentLetterLength + 1)
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage($"Content cannot be longer than {GlobalEntitiesConstraints.MaxContentLetterLength} characters");
  }

  #endregion

  #region XSS Prevention Tests

  [Theory]
  [InlineData("<script>alert('xss')</script>")]
  [InlineData("Hello <script>malicious()</script> there")]
  [InlineData("<SCRIPT>XSS</SCRIPT>")]
  [InlineData("<script src='evil.js'></script>")]
  [InlineData("Message before<script type='text/javascript'>code()</script>after")]
  public async Task Should_Fail_When_Content_Contains_Script_Tags(string contentWithScript)
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = contentWithScript
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot contain script tags");
  }

  [Fact]
  public async Task Should_Detect_Script_Tags_Case_Insensitively()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = "Message with <ScRiPt>alert('xss')</sCrIpT>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content cannot contain script tags");
  }

  #endregion

  #region Dangerous Patterns Tests

  [Theory]
  [InlineData("Click here: javascript:alert('xss')")]
  [InlineData("Link with javascript:void(0)")]
  [InlineData("JAVASCRIPT:malicious()")]
  public async Task Should_Fail_When_Content_Contains_Javascript_Protocol(string contentWithJavascript)
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = contentWithJavascript
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");
  }

  [Theory]
  [InlineData("Data URI: data:text/html,<script>alert('xss')</script>")]
  [InlineData("DATA:TEXT/HTML,malicious")]
  public async Task Should_Fail_When_Content_Contains_Data_Protocol(string contentWithData)
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = contentWithData
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");
  }

  [Theory]
  [InlineData("<div onclick='malicious()'>Click</div>")]
  [InlineData("<img onload='evil()'>")]
  [InlineData("<body onmouseover='attack()'>")]
  [InlineData("<a href='#' onmousedown='bad()'>Link</a>")]
  public async Task Should_Fail_When_Content_Contains_Event_Handlers(string contentWithEvent)
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = contentWithEvent
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");
  }

  #endregion

  #region Edge Cases

  [Fact]
  public async Task Should_Pass_With_Unicode_Characters()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = "Hello 你好 🌟 مرحبا Привет"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Emojis()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = "Great work! 👍 🎉 🚀"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Special_Characters()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = "Price: $100.50 | Discount: 20% | Rate: 4.5★"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Newlines_And_Formatting()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = "Line 1\nLine 2\nLine 3"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_HTML_Entities()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = "5 &gt; 3 &amp; 2 &lt; 7"
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
    var request = new CreateMessageRequest
    {
      ChatId = string.Empty,
      Content = "<script>alert('xss')</script>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.ChatId);
    result.ShouldHaveValidationErrorFor(x => x.Content);
  }

  [Fact]
  public async Task Should_Pass_With_URLs_In_Content()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = "Check out this link: https://example.com"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Email_In_Content()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = "Contact me at user@example.com"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Code_Snippets()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = "Try this: var x = 10; console.log(x);"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Single_Character_Content()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = "!"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Quotes_In_Content()
  {
    // Arrange
    var request = new CreateMessageRequest
    {
      ChatId = "chat-123",
      Content = "He said \"Hello\" and I replied 'Hi'"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion
}
