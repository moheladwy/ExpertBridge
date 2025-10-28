// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Queries;

/// <summary>
/// Unit tests for MediaObjectQueries extension methods.
/// </summary>
/// <remarks>
/// Tests cover SelectMediaObjectResponse projection method with S3 URL construction.
/// Uses in-memory EF Core database for realistic query execution.
/// </remarks>
public sealed class MediaObjectQueriesTests : IDisposable
{
  private readonly ExpertBridgeDbContext _context;

  public MediaObjectQueriesTests()
  {
    _context = InMemoryDbContextFixture.Create();
  }

  #region SelectMediaObjectResponse Tests

  [Fact]
  public async Task SelectMediaObjectResponse_Should_Construct_S3Url_Correctly()
  {
    // Arrange
    var media = (PostMedia)TestDataBuilder.CreatePostMedia(
        postId: "post1",
        type: "image/jpeg",
        key: "profile-images/123.jpg",
        id: "media1");
    _context.Set<PostMedia>().Add(media);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Set<PostMedia>()
        .SelectMediaObjectResponse()
        .FirstAsync();

    // Assert
    result.ShouldNotBeNull();
    result.Url.ShouldBe("https://expert-bridge-media.s3.amazonaws.com/profile-images/123.jpg");
    result.Type.ShouldBe("image/jpeg");
  }

  [Theory]
  [InlineData("image/jpeg", "profile-images/123.jpg")]
  [InlineData("image/png", "posts/456.png")]
  [InlineData("video/mp4", "videos/demo.mp4")]
  [InlineData("application/pdf", "documents/file.pdf")]
  public async Task SelectMediaObjectResponse_Should_Handle_DifferentMediaTypes(
      string type, string key)
  {
    // Arrange
    var media = (PostMedia)TestDataBuilder.CreatePostMedia(
        postId: "post1",
        type: type,
        key: key);
    _context.Set<PostMedia>().Add(media);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Set<PostMedia>()
        .SelectMediaObjectResponse()
        .FirstAsync();

    // Assert
    result.Url.ShouldContain(key);
    result.Type.ShouldBe(type);
  }

  [Fact]
  public async Task SelectMediaObjectResponse_Should_Include_MediaId()
  {
    // Arrange
    var mediaId = "unique-media-id-123";
    var media = (PostMedia)TestDataBuilder.CreatePostMedia(
        postId: "post1",
        type: "image/jpeg",
        key: "test.jpg",
        id: mediaId);
    _context.Set<PostMedia>().Add(media);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Set<PostMedia>()
        .SelectMediaObjectResponse()
        .FirstAsync();

    // Assert
    result.Id.ShouldBe(mediaId);
  }

  [Fact]
  public async Task SelectMediaObjectResponse_Should_Include_MediaName()
  {
    // Arrange
    var media = (PostMedia)TestDataBuilder.CreatePostMedia(
        postId: "post1",
        type: "image/jpeg",
        key: "test.jpg");
    _context.Set<PostMedia>().Add(media);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Set<PostMedia>()
        .SelectMediaObjectResponse()
        .FirstAsync();

    // Assert
    result.Name.ShouldNotBeNull();
    result.Name.ShouldNotBeEmpty();
  }

  [Fact]
  public async Task SelectMediaObjectResponse_Should_Project_MultipleMediaObjects()
  {
    // Arrange
    var media1 = (PostMedia)TestDataBuilder.CreatePostMedia(postId: "post1", type: "image/jpeg", key: "img1.jpg", id: "m1");
    var media2 = (PostMedia)TestDataBuilder.CreatePostMedia(postId: "post1", type: "image/png", key: "img2.png", id: "m2");
    var media3 = (PostMedia)TestDataBuilder.CreatePostMedia(postId: "post2", type: "video/mp4", key: "vid1.mp4", id: "m3");

    _context.Set<PostMedia>().AddRange(media1, media2, media3);
    await _context.SaveChangesAsync();

    // Act
    var results = await _context.Set<PostMedia>()
        .SelectMediaObjectResponse()
        .ToListAsync();

    // Assert
    results.Count.ShouldBe(3);
    results.ShouldAllBe(r => r.Url.StartsWith("https://expert-bridge-media.s3.amazonaws.com/"));
  }

  [Fact]
  public async Task SelectMediaObjectResponse_Should_Return_Empty_When_NoMedia()
  {
    // Arrange - no data added

    // Act
    var results = await _context.Set<PostMedia>()
        .SelectMediaObjectResponse()
        .ToListAsync();

    // Assert
    results.ShouldBeEmpty();
  }

  [Fact]
  public async Task SelectMediaObjectResponse_Should_Use_HardcodedS3Url()
  {
    // Arrange
    var media = (PostMedia)TestDataBuilder.CreatePostMedia(
        postId: "post1",
        type: "image/jpeg",
        key: "test-key.jpg");
    _context.Set<PostMedia>().Add(media);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Set<PostMedia>()
        .SelectMediaObjectResponse()
        .FirstAsync();

    // Assert
    // TODO: This test documents the current hardcoded S3 URL issue
    // Consider refactoring to use configuration-based S3 bucket URL
    result.Url.ShouldStartWith("https://expert-bridge-media.s3.amazonaws.com/");
  }

  #endregion

  public void Dispose()
  {
    _context.Dispose();
  }
}
