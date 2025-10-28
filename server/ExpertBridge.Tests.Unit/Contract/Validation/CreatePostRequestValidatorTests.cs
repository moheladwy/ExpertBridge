// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for CreatePostRequestValidator covering XSS prevention,
///     media validation, and standard field validation.
/// </summary>
/// <remarks>
///     Tests validate: Title, Content, Media collection with nested validation,
///     XSS prevention, dangerous patterns, and business rules (max 10 media items).
/// </remarks>
public sealed class CreatePostRequestValidatorTests
{
  private readonly CreatePostRequestValidator _validator;

  public CreatePostRequestValidatorTests()
  {
    _validator = new CreatePostRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_When_All_Required_Fields_Valid()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Post Title",
      Content = "This is valid post content without any script tags or dangerous patterns."
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Valid_With_Media()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Post with Media",
      Content = "Content here with valid media attachments",
      Media = new List<MediaObjectRequest>
            {
                new() { Type = "image/jpeg", Key = "valid-file-key.jpg" }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Media_Is_Null()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = "Valid content",
      Media = null
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Media);
  }

  [Fact]
  public async Task Should_Pass_When_Media_Is_Empty_List()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = "Valid content",
      Media = new List<MediaObjectRequest>()
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Media);
  }

  #endregion

  #region Title Validation Tests

  [Fact]
  public async Task Should_Have_Error_When_Title_Is_Null()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = null!,
      Content = "Valid content"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title cannot be null");
  }

  [Fact]
  public async Task Should_Have_Error_When_Title_Is_Empty()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = string.Empty,
      Content = "Valid content"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title cannot be empty");
  }

  [Fact]
  public async Task Should_Have_Error_When_Title_Exceeds_Max_Length()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = new string('a', 257), // Max is 256
      Content = "Valid content"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title);
  }

  [Fact]
  public async Task Should_Pass_When_Title_At_Max_Length()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = new string('a', 256), // Max length is 256
      Content = "Valid content"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Title);
  }

  [Fact]
  public async Task Should_Have_Error_When_Title_Contains_Script_Tags()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Bad Title <script>alert('xss')</script>",
      Content = "Valid content"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title must not contain HTML script tags");
  }

  [Fact]
  public async Task Should_Have_Error_When_Title_Contains_HTML_Tags()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Title with <b>HTML</b> tags",
      Content = "Valid content"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
        .WithErrorMessage("Title must not contain HTML tags");
  }

  [Theory]
  [InlineData("<div>Test</div>")]
  [InlineData("<span>Test</span>")]
  [InlineData("<a href='#'>Link</a>")]
  [InlineData("<img src='x'>")]
  public async Task Should_Have_Error_When_Title_Contains_Various_HTML_Tags(string htmlTitle)
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = htmlTitle,
      Content = "Valid content"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title);
  }

  #endregion

  #region Content Validation Tests

  [Fact]
  public async Task Should_Have_Error_When_Content_Is_Null()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = null!
    };

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
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = string.Empty
    };

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
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = new string('a', 5001) // Max is 5000
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content);
  }

  [Fact]
  public async Task Should_Pass_When_Content_At_Max_Length()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = new string('a', 5000) // Max length is 5000
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Content);
  }

  [Fact]
  public async Task Should_Have_Error_When_Content_Contains_Script_Tags()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = "Content with <script>malicious code</script> embedded"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content must not contain script tags");
  }

  [Fact]
  public async Task Should_Have_Error_When_Content_Contains_Dangerous_Patterns()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = "Content with javascript:alert('xss') link"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content)
        .WithErrorMessage("Content contains potentially unsafe patterns");
  }

  [Theory]
  [InlineData("data:text/html,<script>alert('xss')</script>")]
  [InlineData("<div onclick='alert()'>Click me</div>")]
  [InlineData("<img onload='alert()' src='x'>")]
  [InlineData("<body onload='malicious()'>")]
  [InlineData("javascript:void(0)")]
  public async Task Should_Have_Error_When_Content_Contains_Event_Handlers(string dangerousContent)
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = dangerousContent
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content);
  }

  #endregion

  #region Media Validation Tests

  [Fact]
  public async Task Should_Have_Error_When_Media_Count_Exceeds_10()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = "Valid content",
      Media = Enumerable.Range(1, 11)
            .Select(i => new MediaObjectRequest
            {
              Type = "image/jpeg",
              Key = $"file{i}.jpg"
            })
            .ToList()
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Media)
        .WithErrorMessage("Cannot attach more than 10 media items per post");
  }

  [Fact]
  public async Task Should_Pass_When_Media_Count_Is_10()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = "Valid content",
      Media = Enumerable.Range(1, 10)
            .Select(i => new MediaObjectRequest
            {
              Type = "image/jpeg",
              Key = $"valid-file-key-{i}.jpg"
            })
            .ToList()
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Media);
  }

  [Fact]
  public async Task Should_Have_Error_When_Media_Has_Invalid_Type()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = "Valid content",
      Media = new List<MediaObjectRequest>
            {
                new() { Type = "application/exe", Key = "virus.exe" }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Media[0].Type");
  }

  [Fact]
  public async Task Should_Have_Error_When_Media_Key_Contains_Path_Traversal()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = "Valid content",
      Media = new List<MediaObjectRequest>
            {
                new() { Type = "image/jpeg", Key = "../../../etc/passwd" }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Media[0].Key");
  }

  [Theory]
  [InlineData("file\\path\\traversal.jpg")]
  [InlineData("/absolute/path/file.jpg")]
  [InlineData("..\\windows\\system32\\file.jpg")]
  [InlineData("../../secret.txt")]
  public async Task Should_Have_Error_When_Media_Key_Has_Unsafe_Characters(string unsafeKey)
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = "Valid content",
      Media = new List<MediaObjectRequest>
            {
                new() { Type = "image/jpeg", Key = unsafeKey }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Media[0].Key");
  }

  [Fact]
  public async Task Should_Pass_When_Media_Has_Valid_Image_Type()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = "Valid content",
      Media = new List<MediaObjectRequest>
            {
                new() { Type = "image/png", Key = "valid-image.png" },
                new() { Type = "image/jpeg", Key = "valid-photo.jpg" },
                new() { Type = "image/gif", Key = "valid-animation.gif" }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Media_Has_Valid_Video_Type()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = "Valid content",
      Media = new List<MediaObjectRequest>
            {
                new() { Type = "video/mp4", Key = "valid-video.mp4" },
                new() { Type = "video/webm", Key = "valid-video.webm" }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region Edge Cases

  [Fact]
  public async Task Should_Handle_Whitespace_Only_Title()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "   ",
      Content = "Valid content"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title);
  }

  [Fact]
  public async Task Should_Handle_Whitespace_Only_Content()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = "   "
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content);
  }

  [Fact]
  public async Task Should_Pass_When_Title_And_Content_Have_Unicode_Characters()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Title with émojis 🚀 and spëcial çhars",
      Content = "Content with 日本語 characters and symbols: ™ © ®"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Detect_Script_Tags_With_Different_Casing()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Title with <SCRIPT>alert('xss')</SCRIPT>",
      Content = "Valid content"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title);
  }

  [Fact]
  public async Task Should_Detect_Script_Tags_With_Attributes()
  {
    // Arrange
    var request = new CreatePostRequest
    {
      Title = "Valid Title",
      Content = "Content with <script type='text/javascript'>alert('xss')</script>"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Content);
  }

  #endregion
}
