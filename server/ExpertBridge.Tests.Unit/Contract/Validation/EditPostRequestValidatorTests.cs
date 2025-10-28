// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for EditPostRequestValidator covering XSS prevention,
///     optional field validation, and conditional validation rules.
/// </summary>
/// <remarks>
///     Tests validate: Optional Title and Content fields with XSS prevention,
///     dangerous patterns, HTML tag detection, and boundary conditions.
///     All fields are optional - validation only applies when fields are provided.
/// </remarks>
public sealed class EditPostRequestValidatorTests
{
    private readonly EditPostRequestValidator _validator;

    public EditPostRequestValidatorTests()
    {
        _validator = new EditPostRequestValidator();
    }

    #region Happy Path Tests

    [Fact]
    public async Task Should_Pass_When_All_Fields_Null()
    {
        // Arrange
        var request = new EditPostRequest { Title = null, Content = null };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_When_Only_Title_Provided_And_Valid()
    {
        // Arrange
        var request = new EditPostRequest { Title = "Valid Updated Title", Content = null };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_When_Only_Content_Provided_And_Valid()
    {
        // Arrange
        var request = new EditPostRequest
        {
            Title = null, Content = "Valid updated content without any dangerous patterns."
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_When_Both_Fields_Provided_And_Valid()
    {
        // Arrange
        var request = new EditPostRequest
        {
            Title = "Valid Title Update",
            Content = "Valid content update with no script tags or dangerous patterns."
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_When_Title_At_Max_Length()
    {
        // Arrange
        var request = new EditPostRequest
        {
            Title = new string('a', 256), // Max length is 256
            Content = null
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public async Task Should_Pass_When_Content_At_Max_Length()
    {
        // Arrange
        var request = new EditPostRequest
        {
            Title = null, Content = new string('a', 5000) // Max length is 5000
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Content);
    }

    #endregion

    #region Title Validation Tests

    [Fact]
    public async Task Should_Have_Error_When_Title_Is_Empty_String()
    {
        // Arrange
        var request = new EditPostRequest { Title = string.Empty, Content = null };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title cannot be empty when provided");
    }

    [Fact]
    public async Task Should_Have_Error_When_Title_Is_Whitespace()
    {
        // Arrange
        var request = new EditPostRequest { Title = "   ", Content = null };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title cannot be empty when provided");
    }

    [Fact]
    public async Task Should_Have_Error_When_Title_Exceeds_Max_Length()
    {
        // Arrange
        var request = new EditPostRequest
        {
            Title = new string('a', 257), // Max is 256
            Content = null
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title cannot be longer than 256 characters");
    }

    [Fact]
    public async Task Should_Have_Error_When_Title_Contains_Script_Tags()
    {
        // Arrange
        var request = new EditPostRequest { Title = "Bad Title <script>alert('xss')</script>", Content = null };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title cannot contain script tags");
    }

    [Fact]
    public async Task Should_Have_Error_When_Title_Contains_Script_Tags_With_Attributes()
    {
        // Arrange
        var request = new EditPostRequest
        {
            Title = "Title <script type='text/javascript'>malicious()</script>", Content = null
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title cannot contain script tags");
    }

    [Fact]
    public async Task Should_Have_Error_When_Title_Contains_HTML_Tags()
    {
        // Arrange
        var request = new EditPostRequest { Title = "Title with <b>HTML</b> tags", Content = null };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title cannot contain HTML tags");
    }

    [Theory]
    [InlineData("<div>Test</div>")]
    [InlineData("<span>Test</span>")]
    [InlineData("<a href='#'>Link</a>")]
    [InlineData("<img src='x'>")]
    [InlineData("<p>Paragraph</p>")]
    [InlineData("<h1>Heading</h1>")]
    public async Task Should_Have_Error_When_Title_Contains_Various_HTML_Tags(string htmlTitle)
    {
        // Arrange
        var request = new EditPostRequest { Title = htmlTitle, Content = null };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    #endregion

    #region Content Validation Tests

    [Fact]
    public async Task Should_Have_Error_When_Content_Is_Empty_String()
    {
        // Arrange
        var request = new EditPostRequest { Title = null, Content = string.Empty };

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
        var request = new EditPostRequest { Title = null, Content = "   " };

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
        var request = new EditPostRequest
        {
            Title = null, Content = new string('a', 5001) // Max is 5000
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
        var request = new EditPostRequest
        {
            Title = null, Content = "Valid content but has <script>alert('xss')</script> in it"
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
        var request = new EditPostRequest
        {
            Title = null,
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
        var request = new EditPostRequest { Title = null, Content = "Click here: javascript:alert('xss')" };

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
        var request = new EditPostRequest
        {
            Title = null, Content = "Image: data:text/html,<script>alert('xss')</script>"
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
        var request = new EditPostRequest { Title = null, Content = $"Content with {eventHandler} event handler" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage(
                "Content contains potentially dangerous patterns (javascript:, data:, or event handlers)");
    }

    #endregion

    #region Conditional Validation Tests

    [Fact]
    public async Task Should_Not_Validate_Title_When_Null()
    {
        // Arrange
        var request = new EditPostRequest { Title = null, Content = "Valid content" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public async Task Should_Not_Validate_Content_When_Null()
    {
        // Arrange
        var request = new EditPostRequest { Title = "Valid title", Content = null };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public async Task Should_Validate_Only_Provided_Fields()
    {
        // Arrange - Only Title provided with error
        var request = new EditPostRequest
        {
            Title = new string('a', 257), // Invalid
            Content = null // Not validated
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.Content);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task Should_Pass_When_Title_Has_Valid_Special_Characters()
    {
        // Arrange
        var request = new EditPostRequest { Title = "Title with émojis 🎉 and spëcial çharacters!", Content = null };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public async Task Should_Pass_When_Content_Has_Valid_Special_Characters()
    {
        // Arrange
        var request = new EditPostRequest
        {
            Title = null, Content = "Content with émojis 🎉, line breaks\nand special characters: @#$%^&*()"
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public async Task Should_Detect_Case_Insensitive_Script_Tags()
    {
        // Arrange
        var request = new EditPostRequest { Title = "Title with <SCRIPT>alert('xss')</SCRIPT>", Content = null };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public async Task Should_Detect_Case_Insensitive_Javascript_Protocol()
    {
        // Arrange
        var request = new EditPostRequest { Title = null, Content = "Link: JAVASCRIPT:alert('xss')" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public async Task Should_Have_Multiple_Errors_When_Both_Fields_Invalid()
    {
        // Arrange
        var request = new EditPostRequest
        {
            Title = new string('a', 257), // Too long
            Content = "<script>alert('xss')</script>" // Script tag
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    #endregion
}
