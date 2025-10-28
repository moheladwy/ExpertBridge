namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for GeneratePresignedUrlsRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: file collection validation, content type validation, file size limits,
///     path traversal prevention, and collection count limits.
/// </remarks>
public sealed class GeneratePresignedUrlsRequestValidatorTests
{
  private readonly GeneratePresignedUrlsRequestValidator _validator;

  public GeneratePresignedUrlsRequestValidatorTests()
  {
    _validator = new GeneratePresignedUrlsRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_With_Valid_Single_Image_File()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "image/jpeg",
                    Type = "image",
                    Name = "photo.jpg",
                    Size = 1024 * 1024, // 1 MB
                    Extension = ".jpg"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Valid_Single_Video_File()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "video/mp4",
                    Type = "video",
                    Name = "video.mp4",
                    Size = 50 * 1024 * 1024, // 50 MB
                    Extension = ".mp4"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Valid_Single_Document_File()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "application/pdf",
                    Type = "document",
                    Name = "resume.pdf",
                    Size = 2 * 1024 * 1024, // 2 MB
                    Extension = ".pdf"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Multiple_Valid_Files()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "image/png",
                    Type = "image",
                    Name = "screenshot.png",
                    Size = 500 * 1024,
                    Extension = ".png"
                },
                new FileMetadata
                {
                    ContentType = "video/webm",
                    Type = "video",
                    Name = "recording.webm",
                    Size = 10 * 1024 * 1024,
                    Extension = ".webm"
                },
                new FileMetadata
                {
                    ContentType = "application/pdf",
                    Type = "document",
                    Name = "report.pdf",
                    Size = 1024 * 1024,
                    Extension = ".pdf"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_All_Allowed_Image_Types()
  {
    // Arrange
    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "image/svg+xml" };

    foreach (var contentType in allowedTypes)
    {
      var request = new GeneratePresignedUrlsRequest
      {
        Files = new List<FileMetadata>
                {
                    new FileMetadata
                    {
                        ContentType = contentType,
                        Type = "image",
                        Name = "test.jpg",
                        Size = 1024,
                        Extension = ".jpg"
                    }
                }
      };

      // Act
      var result = await _validator.TestValidateAsync(request);

      // Assert
      result.ShouldNotHaveAnyValidationErrors();
    }
  }

  [Fact]
  public async Task Should_Pass_With_All_Allowed_Video_Types()
  {
    // Arrange
    var allowedTypes = new[] { "video/mp4", "video/mpeg", "video/quicktime", "video/webm", "video/x-msvideo" };

    foreach (var contentType in allowedTypes)
    {
      var request = new GeneratePresignedUrlsRequest
      {
        Files = new List<FileMetadata>
                {
                    new FileMetadata
                    {
                        ContentType = contentType,
                        Type = "video",
                        Name = "test.mp4",
                        Size = 1024,
                        Extension = ".mp4"
                    }
                }
      };

      // Act
      var result = await _validator.TestValidateAsync(request);

      // Assert
      result.ShouldNotHaveAnyValidationErrors();
    }
  }

  [Fact]
  public async Task Should_Pass_With_All_Allowed_Document_Types()
  {
    // Arrange
    var allowedTypes = new[]
    {
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "text/plain"
        };

    foreach (var contentType in allowedTypes)
    {
      var request = new GeneratePresignedUrlsRequest
      {
        Files = new List<FileMetadata>
                {
                    new FileMetadata
                    {
                        ContentType = contentType,
                        Type = "document",
                        Name = "test.pdf",
                        Size = 1024,
                        Extension = ".pdf"
                    }
                }
      };

      // Act
      var result = await _validator.TestValidateAsync(request);

      // Assert
      result.ShouldNotHaveAnyValidationErrors();
    }
  }

  [Fact]
  public async Task Should_Pass_With_File_At_Max_Size_Boundary()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "video/mp4",
                    Type = "video",
                    Name = "large.mp4",
                    Size = 100 * 1024 * 1024, // Exactly 100 MB
                    Extension = ".mp4"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_10_Files_At_Collection_Limit()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = Enumerable.Range(1, 10).Select(i => new FileMetadata
      {
        ContentType = "image/jpeg",
        Type = "image",
        Name = $"photo{i}.jpg",
        Size = 1024,
        Extension = ".jpg"
      }).ToList()
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region Files Collection Validation Tests

  [Fact]
  public async Task Should_Fail_When_Files_Is_Empty()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>()
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Files)
        .WithErrorMessage("At least one file must be provided");
  }

  [Fact]
  public async Task Should_Fail_When_Files_Exceeds_10_Count_Limit()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = Enumerable.Range(1, 11).Select(i => new FileMetadata
      {
        ContentType = "image/jpeg",
        Type = "image",
        Name = $"photo{i}.jpg",
        Size = 1024,
        Extension = ".jpg"
      }).ToList()
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Files)
        .WithErrorMessage("Cannot upload more than 10 files at once");
  }

  #endregion

  #region ContentType Validation Tests

  [Fact]
  public async Task Should_Fail_When_ContentType_Is_Null()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = null!,
                    Type = "image",
                    Name = "photo.jpg",
                    Size = 1024,
                    Extension = ".jpg"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Files[0].ContentType")
        .WithErrorMessage("ContentType cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_ContentType_Is_Empty()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = string.Empty,
                    Type = "image",
                    Name = "photo.jpg",
                    Size = 1024,
                    Extension = ".jpg"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Files[0].ContentType")
        .WithErrorMessage("ContentType cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_ContentType_Is_Not_Allowed()
  {
    // Arrange
    var disallowedTypes = new[] { "application/x-executable", "text/html", "application/javascript", "image/tiff" };

    foreach (var contentType in disallowedTypes)
    {
      var request = new GeneratePresignedUrlsRequest
      {
        Files = new List<FileMetadata>
                {
                    new FileMetadata
                    {
                        ContentType = contentType,
                        Type = "other",
                        Name = "file.bin",
                        Size = 1024,
                        Extension = ".bin"
                    }
                }
      };

      // Act
      var result = await _validator.TestValidateAsync(request);

      // Assert
      result.ShouldHaveValidationErrorFor("Files[0].ContentType")
          .WithErrorMessage("ContentType is not allowed. Only images, videos, and documents are supported.");
    }
  }

  [Fact]
  public async Task Should_Pass_When_ContentType_Is_Case_Insensitive()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "IMAGE/JPEG", // Uppercase
                    Type = "image",
                    Name = "photo.jpg",
                    Size = 1024,
                    Extension = ".jpg"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region Type Validation Tests

  [Fact]
  public async Task Should_Fail_When_Type_Is_Null()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "image/jpeg",
                    Type = null!,
                    Name = "photo.jpg",
                    Size = 1024,
                    Extension = ".jpg"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Files[0].Type")
        .WithErrorMessage("Type cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_Type_Is_Empty()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "image/jpeg",
                    Type = string.Empty,
                    Name = "photo.jpg",
                    Size = 1024,
                    Extension = ".jpg"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Files[0].Type")
        .WithErrorMessage("Type cannot be empty");
  }

  #endregion

  #region Name Validation Tests

  [Fact]
  public async Task Should_Fail_When_Name_Is_Empty()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "image/jpeg",
                    Type = "image",
                    Name = string.Empty,
                    Size = 1024,
                    Extension = ".jpg"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Files[0].Name")
        .WithErrorMessage("Name cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Name_Contains_Path_Traversal_Patterns()
  {
    // Arrange
    var maliciousNames = new[] { "../../../etc/passwd", "..\\..\\windows\\system32", "file<script>.jpg", "file:name.jpg", "file|name.jpg", "file?.jpg", "file*.jpg" };

    foreach (var maliciousName in maliciousNames)
    {
      var request = new GeneratePresignedUrlsRequest
      {
        Files = new List<FileMetadata>
                {
                    new FileMetadata
                    {
                        ContentType = "image/jpeg",
                        Type = "image",
                        Name = maliciousName,
                        Size = 1024,
                        Extension = ".jpg"
                    }
                }
      };

      // Act
      var result = await _validator.TestValidateAsync(request);

      // Assert
      result.ShouldHaveValidationErrorFor("Files[0].Name")
          .WithErrorMessage("File name contains invalid path traversal characters");
    }
  }

  [Fact]
  public async Task Should_Pass_When_Name_Is_Valid()
  {
    // Arrange
    var validNames = new[] { "photo.jpg", "my-document.pdf", "video_2024.mp4", "report-final.docx" };

    foreach (var validName in validNames)
    {
      var request = new GeneratePresignedUrlsRequest
      {
        Files = new List<FileMetadata>
                {
                    new FileMetadata
                    {
                        ContentType = "image/jpeg",
                        Type = "image",
                        Name = validName,
                        Size = 1024,
                        Extension = ".jpg"
                    }
                }
      };

      // Act
      var result = await _validator.TestValidateAsync(request);

      // Assert
      result.ShouldNotHaveAnyValidationErrors();
    }
  }

  #endregion

  #region Size Validation Tests

  [Fact]
  public async Task Should_Fail_When_Size_Is_Zero()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "image/jpeg",
                    Type = "image",
                    Name = "photo.jpg",
                    Size = 0,
                    Extension = ".jpg"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Files[0].Size")
        .WithErrorMessage("Size must be greater than 0");
  }

  [Fact]
  public async Task Should_Fail_When_Size_Is_Negative()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "image/jpeg",
                    Type = "image",
                    Name = "photo.jpg",
                    Size = -1024,
                    Extension = ".jpg"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Files[0].Size")
        .WithErrorMessage("Size must be greater than 0");
  }

  [Fact]
  public async Task Should_Fail_When_Size_Exceeds_100MB_Limit()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "video/mp4",
                    Type = "video",
                    Name = "huge.mp4",
                    Size = (100 * 1024 * 1024) + 1, // 100 MB + 1 byte
                    Extension = ".mp4"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Files[0].Size")
        .WithErrorMessage("File size cannot exceed 100 MB");
  }

  #endregion

  #region Extension Validation Tests

  [Fact]
  public async Task Should_Fail_When_Extension_Is_Empty()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "image/jpeg",
                    Type = "image",
                    Name = "photo.jpg",
                    Size = 1024,
                    Extension = string.Empty
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Files[0].Extension")
        .WithErrorMessage("Extension cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Extension_Contains_Path_Traversal_Patterns()
  {
    // Arrange
    var maliciousExtensions = new[] { "../.jpg", "..\\.png", ".<script>", ".|pdf" };

    foreach (var maliciousExtension in maliciousExtensions)
    {
      var request = new GeneratePresignedUrlsRequest
      {
        Files = new List<FileMetadata>
                {
                    new FileMetadata
                    {
                        ContentType = "image/jpeg",
                        Type = "image",
                        Name = "photo.jpg",
                        Size = 1024,
                        Extension = maliciousExtension
                    }
                }
      };

      // Act
      var result = await _validator.TestValidateAsync(request);

      // Assert
      result.ShouldHaveValidationErrorFor("Files[0].Extension")
          .WithErrorMessage("Extension contains invalid characters");
    }
  }

  #endregion

  #region Multiple Files Validation Tests

  [Fact]
  public async Task Should_Fail_When_Second_File_In_Collection_Is_Invalid()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "image/jpeg",
                    Type = "image",
                    Name = "valid.jpg",
                    Size = 1024,
                    Extension = ".jpg"
                },
                new FileMetadata
                {
                    ContentType = "application/x-executable", // Invalid type
                    Type = "executable",
                    Name = "malicious.exe",
                    Size = 1024,
                    Extension = ".exe"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Files[1].ContentType")
        .WithErrorMessage("ContentType is not allowed. Only images, videos, and documents are supported.");
  }

  [Fact]
  public async Task Should_Fail_When_Multiple_Files_Have_Validation_Errors()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = string.Empty, // Invalid
                    Type = "image",
                    Name = "photo.jpg",
                    Size = 1024,
                    Extension = ".jpg"
                },
                new FileMetadata
                {
                    ContentType = "image/jpeg",
                    Type = "image",
                    Name = "../../../etc/passwd", // Invalid path traversal
                    Size = 1024,
                    Extension = ".jpg"
                },
                new FileMetadata
                {
                    ContentType = "video/mp4",
                    Type = "video",
                    Name = "huge.mp4",
                    Size = (100 * 1024 * 1024) + 1, // Exceeds limit
                    Extension = ".mp4"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Files[0].ContentType");
    result.ShouldHaveValidationErrorFor("Files[1].Name");
    result.ShouldHaveValidationErrorFor("Files[2].Size");
  }

  #endregion

  #region Edge Cases

  [Fact]
  public async Task Should_Pass_With_Minimal_Valid_File_Size()
  {
    // Arrange
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "image/jpeg",
                    Type = "image",
                    Name = "tiny.jpg",
                    Size = 1, // 1 byte
                    Extension = ".jpg"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Long_Valid_File_Name()
  {
    // Arrange
    var longName = new string('a', 200) + ".jpg";
    var request = new GeneratePresignedUrlsRequest
    {
      Files = new List<FileMetadata>
            {
                new FileMetadata
                {
                    ContentType = "image/jpeg",
                    Type = "image",
                    Name = longName,
                    Size = 1024,
                    Extension = ".jpg"
                }
            }
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Various_Valid_Extensions()
  {
    // Arrange
    var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".mp4", ".mpeg", ".pdf", ".docx", ".xlsx", ".txt" };

    foreach (var extension in validExtensions)
    {
      var request = new GeneratePresignedUrlsRequest
      {
        Files = new List<FileMetadata>
                {
                    new FileMetadata
                    {
                        ContentType = "image/jpeg",
                        Type = "image",
                        Name = $"file{extension}",
                        Size = 1024,
                        Extension = extension
                    }
                }
      };

      // Act
      var result = await _validator.TestValidateAsync(request);

      // Assert
      result.ShouldNotHaveAnyValidationErrors();
    }
  }

  #endregion
}
