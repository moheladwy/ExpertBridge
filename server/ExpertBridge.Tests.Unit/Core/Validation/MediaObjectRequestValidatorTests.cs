// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
namespace ExpertBridge.Tests.Unit.Core.Validation;

/// <summary>
///     Unit tests for MediaObjectRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: Key validation, Type validation, null/empty checks, and edge cases.
///     MediaObjectRequest represents media files already uploaded to S3.
/// </remarks>
public sealed class MediaObjectRequestValidatorTests
{
  private readonly MediaObjectRequestValidator _validator;

  public MediaObjectRequestValidatorTests()
  {
    _validator = new MediaObjectRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_With_Valid_Image_Request()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = "uploads/images/abc123.jpg",
      Type = "image/jpeg"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Valid_Video_Request()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = "uploads/videos/xyz789.mp4",
      Type = "video/mp4"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Valid_Document_Request()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = "uploads/documents/report.pdf",
      Type = "application/pdf"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_S3_Key_With_Prefix()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = "media/posts/2024/10/image123.png",
      Type = "image/png"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_GUID_In_Key()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = "uploads/550e8400-e29b-41d4-a716-446655440000.jpg",
      Type = "image/jpeg"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region Key Validation Tests

  [Fact]
  public async Task Should_Fail_When_Key_Is_Null()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = null!,
      Type = "image/jpeg"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Key)
        .WithErrorMessage("Media key is required");
  }

  [Fact]
  public async Task Should_Fail_When_Key_Is_Empty()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = string.Empty,
      Type = "image/jpeg"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Key)
        .WithErrorMessage("Media key is required");
  }

  [Fact]
  public async Task Should_Fail_When_Key_Is_Whitespace()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = "   ",
      Type = "image/jpeg"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Key)
        .WithErrorMessage("Media key is required");
  }

  #endregion

  #region Type Validation Tests

  [Fact]
  public async Task Should_Fail_When_Type_Is_Null()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = "uploads/image.jpg",
      Type = null!
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Type)
        .WithErrorMessage("Media type is required");
  }

  [Fact]
  public async Task Should_Fail_When_Type_Is_Empty()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = "uploads/image.jpg",
      Type = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Type)
        .WithErrorMessage("Media type is required");
  }

  [Fact]
  public async Task Should_Fail_When_Type_Is_Whitespace()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = "uploads/image.jpg",
      Type = "   "
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Type)
        .WithErrorMessage("Media type is required");
  }

  #endregion

  #region Edge Cases

  [Fact]
  public async Task Should_Pass_With_Long_S3_Key()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = "uploads/2024/10/26/user123/posts/post456/media/550e8400-e29b-41d4-a716-446655440000-original.jpg",
      Type = "image/jpeg"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Various_Image_Types()
  {
    // Arrange - JPEG
    var jpegRequest = new MediaObjectRequest
    {
      Key = "uploads/image.jpg",
      Type = "image/jpeg"
    };

    // Act
    var jpegResult = await _validator.TestValidateAsync(jpegRequest);

    // Assert
    jpegResult.ShouldNotHaveAnyValidationErrors();

    // Arrange - PNG
    var pngRequest = new MediaObjectRequest
    {
      Key = "uploads/image.png",
      Type = "image/png"
    };

    // Act
    var pngResult = await _validator.TestValidateAsync(pngRequest);

    // Assert
    pngResult.ShouldNotHaveAnyValidationErrors();

    // Arrange - GIF
    var gifRequest = new MediaObjectRequest
    {
      Key = "uploads/animation.gif",
      Type = "image/gif"
    };

    // Act
    var gifResult = await _validator.TestValidateAsync(gifRequest);

    // Assert
    gifResult.ShouldNotHaveAnyValidationErrors();

    // Arrange - WebP
    var webpRequest = new MediaObjectRequest
    {
      Key = "uploads/image.webp",
      Type = "image/webp"
    };

    // Act
    var webpResult = await _validator.TestValidateAsync(webpRequest);

    // Assert
    webpResult.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Various_Video_Types()
  {
    // Arrange - MP4
    var mp4Request = new MediaObjectRequest
    {
      Key = "uploads/video.mp4",
      Type = "video/mp4"
    };

    // Act
    var mp4Result = await _validator.TestValidateAsync(mp4Request);

    // Assert
    mp4Result.ShouldNotHaveAnyValidationErrors();

    // Arrange - WebM
    var webmRequest = new MediaObjectRequest
    {
      Key = "uploads/video.webm",
      Type = "video/webm"
    };

    // Act
    var webmResult = await _validator.TestValidateAsync(webmRequest);

    // Assert
    webmResult.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_PDF_Document()
  {
    // Arrange
    var pdfRequest = new MediaObjectRequest
    {
      Key = "uploads/document.pdf",
      Type = "application/pdf"
    };

    // Act
    var pdfResult = await _validator.TestValidateAsync(pdfRequest);

    // Assert
    pdfResult.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Fail_When_Type_Is_Unsupported_Video_Format()
  {
    // Arrange - video/quicktime is not in allowed types
    var movRequest = new MediaObjectRequest
    {
      Key = "uploads/video.mov",
      Type = "video/quicktime"
    };

    // Act
    var movResult = await _validator.TestValidateAsync(movRequest);

    // Assert
    movResult.ShouldHaveValidationErrorFor(x => x.Type)
        .WithErrorMessage("Media type must be one of: image/jpeg, image/png, image/gif, image/webp, video/mp4, video/webm, application/pdf");
  }

  [Fact]
  public async Task Should_Fail_When_Type_Is_Word_Document()
  {
    // Arrange - Word documents are not supported
    var docxRequest = new MediaObjectRequest
    {
      Key = "uploads/document.docx",
      Type = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    };

    // Act
    var docxResult = await _validator.TestValidateAsync(docxRequest);

    // Assert
    docxResult.ShouldHaveValidationErrorFor(x => x.Type)
        .WithErrorMessage("Media type must be one of: image/jpeg, image/png, image/gif, image/webp, video/mp4, video/webm, application/pdf");
  }

  [Fact]
  public async Task Should_Fail_When_Type_Is_Text_File()
  {
    // Arrange - Text files are not supported
    var textRequest = new MediaObjectRequest
    {
      Key = "uploads/file.txt",
      Type = "text/plain"
    };

    // Act
    var textResult = await _validator.TestValidateAsync(textRequest);

    // Assert
    textResult.ShouldHaveValidationErrorFor(x => x.Type)
        .WithErrorMessage("Media type must be one of: image/jpeg, image/png, image/gif, image/webp, video/mp4, video/webm, application/pdf");
  }

  [Fact]
  public async Task Should_Fail_When_Type_Is_Audio()
  {
    // Arrange - Audio files are not supported
    var request = new MediaObjectRequest
    {
      Key = "uploads/audio.mp3",
      Type = "audio/mpeg"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Type)
        .WithErrorMessage("Media type must be one of: image/jpeg, image/png, image/gif, image/webp, video/mp4, video/webm, application/pdf");
  }

  [Fact]
  public async Task Should_Fail_When_Type_Is_Archive()
  {
    // Arrange - Archive files are not supported
    var request = new MediaObjectRequest
    {
      Key = "uploads/archive.zip",
      Type = "application/zip"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Type)
        .WithErrorMessage("Media type must be one of: image/jpeg, image/png, image/gif, image/webp, video/mp4, video/webm, application/pdf");
  }

  [Fact]
  public async Task Should_Fail_With_Multiple_Validation_Errors()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = string.Empty,
      Type = string.Empty
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Key);
    result.ShouldHaveValidationErrorFor(x => x.Type);
  }

  [Fact]
  public async Task Should_Pass_With_Key_Containing_Special_Characters()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = "uploads/file-name_123.v2.final.jpg",
      Type = "image/jpeg"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Nested_Directory_Structure()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = "media/users/user-123/posts/post-456/attachments/image.jpg",
      Type = "image/jpeg"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Timestamp_In_Key()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = "uploads/2024-10-26T10-30-00Z-image.jpg",
      Type = "image/jpeg"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Short_Key()
  {
    // Arrange
    var request = new MediaObjectRequest
    {
      Key = "a.jpg",
      Type = "image/jpeg"
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion
}
