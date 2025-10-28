// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Queries;
using ExpertBridge.Tests.Unit.Contract.Queries._Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace ExpertBridge.Tests.Unit.Contract.Queries;

/// <summary>
///     Tests for <see cref="PostQueries" /> extension methods.
/// </summary>
public sealed class PostQueriesTests : IDisposable
{
  private readonly ExpertBridgeDbContext _context;

  public PostQueriesTests()
  {
    _context = InMemoryDbContextFixture.Create();
  }

  public void Dispose()
  {
    _context.Dispose();
  }

  [Fact]
  public async Task FullyPopulatedPostQuery_ShouldIncludeAuthor()
  {
    // Arrange
    var authorProfile = TestDataBuilder.CreateProfile(id: "author-1", userId: "user-1");
    var post = TestDataBuilder.CreatePost(
        authorId: authorProfile.Id,
        title: "Test Post",
        content: "Test Content",
        id: "post-1"
    );

    _context.Profiles.Add(authorProfile);
    _context.Posts.Add(post);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Posts
        .FullyPopulatedPostQuery()
        .FirstAsync();

    // Assert
    result.ShouldNotBeNull();
    result.Author.ShouldNotBeNull();
    result.Author.Id.ShouldBe("author-1");
  }

  [Fact]
  public async Task FullyPopulatedPostQuery_ShouldIncludeVotes()
  {
    // Arrange
    var authorProfile = TestDataBuilder.CreateProfile(id: "author-1", userId: "user-1");
    var voterProfile = TestDataBuilder.CreateProfile(id: "voter-1", userId: "user-2");
    var post = TestDataBuilder.CreatePost(
        authorId: authorProfile.Id,
        title: "Test Post",
        content: "Test Content",
        id: "post-1"
    );
    var vote = TestDataBuilder.CreatePostVote(postId: post.Id, profileId: voterProfile.Id, isUpvote: true);

    _context.Profiles.AddRange(authorProfile, voterProfile);
    _context.Posts.Add(post);
    _context.PostVotes.Add(vote);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Posts
        .FullyPopulatedPostQuery()
        .FirstAsync();

    // Assert
    result.Votes.ShouldNotBeEmpty();
    result.Votes.Count.ShouldBe(1);
    result.Votes.First().IsUpvote.ShouldBeTrue();
  }

  [Fact]
  public async Task FullyPopulatedPostQuery_ShouldIncludeMedias()
  {
    // Arrange
    var authorProfile = TestDataBuilder.CreateProfile(id: "author-1", userId: "user-1");
    var post = TestDataBuilder.CreatePost(
        authorId: authorProfile.Id,
        title: "Test Post",
        content: "Test Content",
        id: "post-1"
    );
    var media = TestDataBuilder.CreatePostMedia(postId: post.Id, type: "image/jpeg", key: "test-image.jpg");

    _context.Profiles.Add(authorProfile);
    _context.Posts.Add(post);
    _context.PostMedias.Add((ExpertBridge.Core.Entities.Media.PostMedia.PostMedia)media);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Posts
        .FullyPopulatedPostQuery()
        .FirstAsync();

    // Assert
    result.Medias.ShouldNotBeEmpty();
    result.Medias.Count.ShouldBe(1);
    result.Medias.First().Key.ShouldBe("test-image.jpg");
  }

  [Fact]
  public async Task FullyPopulatedPostQuery_ShouldIncludeComments()
  {
    // Arrange
    var authorProfile = TestDataBuilder.CreateProfile(id: "author-1", userId: "user-1");
    var commenterProfile = TestDataBuilder.CreateProfile(id: "commenter-1", userId: "user-2");
    var post = TestDataBuilder.CreatePost(
        authorId: authorProfile.Id,
        title: "Test Post",
        content: "Test Content",
        id: "post-1"
    );
    var comment = TestDataBuilder.CreateComment(
        authorId: commenterProfile.Id,
        postId: post.Id,
        content: "Test Comment"
    );

    _context.Profiles.AddRange(authorProfile, commenterProfile);
    _context.Posts.Add(post);
    _context.Comments.Add(comment);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Posts
        .FullyPopulatedPostQuery()
        .FirstAsync();

    // Assert
    result.Comments.ShouldNotBeEmpty();
    result.Comments.Count.ShouldBe(1);
    result.Comments.First().Content.ShouldBe("Test Comment");
  }

  [Fact]
  public async Task FullyPopulatedPostQuery_ShouldIncludePostTagsWithTags()
  {
    // Arrange
    var authorProfile = TestDataBuilder.CreateProfile(id: "author-1", userId: "user-1");
    var post = TestDataBuilder.CreatePost(
        authorId: authorProfile.Id,
        title: "Test Post",
        content: "Test Content",
        id: "post-1"
    );
    var tag = TestDataBuilder.CreateTag(englishName: "Technology", arabicName: "تكنولوجيا", id: "tag-1");
    var postTag = TestDataBuilder.CreatePostTag(postId: post.Id, tagId: tag.Id);

    _context.Profiles.Add(authorProfile);
    _context.Posts.Add(post);
    _context.Tags.Add(tag);
    _context.PostTags.Add(postTag);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Posts
        .FullyPopulatedPostQuery()
        .FirstAsync();

    // Assert
    result.PostTags.ShouldNotBeEmpty();
    result.PostTags.Count.ShouldBe(1);
    result.PostTags.First().Tag.ShouldNotBeNull();
    result.PostTags.First().Tag.EnglishName.ShouldBe("Technology");
  }

  [Fact]
  public async Task SelectPostResponseFromFullPost_ShouldCalculateVotesCorrectly()
  {
    // Arrange
    var authorProfile = TestDataBuilder.CreateProfile(id: "author-1", userId: "user-1");
    var voter1 = TestDataBuilder.CreateProfile(id: "voter-1", userId: "user-2");
    var voter2 = TestDataBuilder.CreateProfile(id: "voter-2", userId: "user-3");
    var voter3 = TestDataBuilder.CreateProfile(id: "voter-3", userId: "user-4");

    var post = TestDataBuilder.CreatePost(
        authorId: authorProfile.Id,
        title: "Test Post",
        content: "Test Content",
        id: "post-1"
    );

    var upvote1 = TestDataBuilder.CreatePostVote(postId: post.Id, profileId: voter1.Id, isUpvote: true);
    var upvote2 = TestDataBuilder.CreatePostVote(postId: post.Id, profileId: voter2.Id, isUpvote: true);
    var downvote = TestDataBuilder.CreatePostVote(postId: post.Id, profileId: voter3.Id, isUpvote: false);

    _context.Profiles.AddRange(authorProfile, voter1, voter2, voter3);
    _context.Posts.Add(post);
    _context.PostVotes.AddRange(upvote1, upvote2, downvote);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Posts
        .FullyPopulatedPostQuery()
        .SelectPostResponseFromFullPost(userProfileId: null)
        .FirstAsync();

    // Assert
    result.Upvotes.ShouldBe(2);
    result.Downvotes.ShouldBe(1);
  }

  [Fact]
  public async Task SelectPostResponseFromFullPost_ShouldCalculateCommentsCountExcludingDeleted()
  {
    // Arrange
    var authorProfile = TestDataBuilder.CreateProfile(id: "author-1", userId: "user-1");
    var commenterProfile = TestDataBuilder.CreateProfile(id: "commenter-1", userId: "user-2");
    var post = TestDataBuilder.CreatePost(
        authorId: authorProfile.Id,
        title: "Test Post",
        content: "Test Content",
        id: "post-1"
    );

    var comment1 = TestDataBuilder.CreateComment(
        authorId: commenterProfile.Id,
        postId: post.Id,
        content: "Active Comment 1"
    );
    var comment2 = TestDataBuilder.CreateComment(
        authorId: commenterProfile.Id,
        postId: post.Id,
        content: "Active Comment 2"
    );
    var deletedComment = TestDataBuilder.CreateComment(
        authorId: commenterProfile.Id,
        postId: post.Id,
        content: "Deleted Comment"
    );
    deletedComment.IsDeleted = true;

    _context.Profiles.AddRange(authorProfile, commenterProfile);
    _context.Posts.Add(post);
    _context.Comments.AddRange(comment1, comment2, deletedComment);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Posts
        .FullyPopulatedPostQuery()
        .SelectPostResponseFromFullPost(userProfileId: null)
        .FirstAsync();

    // Assert - Query correctly excludes deleted comments via Comment.Count property which filters
    // However, the current implementation uses p.Comments.Count which includes deleted
    // This test validates the actual behavior - it counts only non-deleted comments from FullyPopulatedPostQuery
    result.Comments.ShouldBe(2); // Correctly excludes deleted comment
  }

  [Fact]
  public async Task SelectPostResponseFromFullPost_ShouldSetIsUpvotedTrueWhenUserUpvoted()
  {
    // Arrange
    var authorProfile = TestDataBuilder.CreateProfile(id: "author-1", userId: "user-1");
    var voterProfile = TestDataBuilder.CreateProfile(id: "voter-1", userId: "user-2");
    var post = TestDataBuilder.CreatePost(
        authorId: authorProfile.Id,
        title: "Test Post",
        content: "Test Content",
        id: "post-1"
    );
    var upvote = TestDataBuilder.CreatePostVote(postId: post.Id, profileId: voterProfile.Id, isUpvote: true);

    _context.Profiles.AddRange(authorProfile, voterProfile);
    _context.Posts.Add(post);
    _context.PostVotes.Add(upvote);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Posts
        .FullyPopulatedPostQuery()
        .SelectPostResponseFromFullPost(userProfileId: voterProfile.Id)
        .FirstAsync();

    // Assert
    result.IsUpvoted.ShouldBeTrue();
    result.IsDownvoted.ShouldBeFalse();
  }

  [Fact]
  public async Task SelectPostResponseFromFullPost_ShouldSetIsDownvotedTrueWhenUserDownvoted()
  {
    // Arrange
    var authorProfile = TestDataBuilder.CreateProfile(id: "author-1", userId: "user-1");
    var voterProfile = TestDataBuilder.CreateProfile(id: "voter-1", userId: "user-2");
    var post = TestDataBuilder.CreatePost(
        authorId: authorProfile.Id,
        title: "Test Post",
        content: "Test Content",
        id: "post-1"
    );
    var downvote = TestDataBuilder.CreatePostVote(postId: post.Id, profileId: voterProfile.Id, isUpvote: false);

    _context.Profiles.AddRange(authorProfile, voterProfile);
    _context.Posts.Add(post);
    _context.PostVotes.Add(downvote);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Posts
        .FullyPopulatedPostQuery()
        .SelectPostResponseFromFullPost(userProfileId: voterProfile.Id)
        .FirstAsync();

    // Assert
    result.IsUpvoted.ShouldBeFalse();
    result.IsDownvoted.ShouldBeTrue();
  }

  [Fact]
  public async Task SelectPostResponseFromFullPost_ShouldSetBothVoteFlagsFalseWhenUserNotVoted()
  {
    // Arrange
    var authorProfile = TestDataBuilder.CreateProfile(id: "author-1", userId: "user-1");
    var nonVoterProfile = TestDataBuilder.CreateProfile(id: "non-voter-1", userId: "user-2");
    var post = TestDataBuilder.CreatePost(
        authorId: authorProfile.Id,
        title: "Test Post",
        content: "Test Content",
        id: "post-1"
    );

    _context.Profiles.AddRange(authorProfile, nonVoterProfile);
    _context.Posts.Add(post);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Posts
        .FullyPopulatedPostQuery()
        .SelectPostResponseFromFullPost(userProfileId: nonVoterProfile.Id)
        .FirstAsync();

    // Assert
    result.IsUpvoted.ShouldBeFalse();
    result.IsDownvoted.ShouldBeFalse();
  }

  [Fact]
  public async Task SelectPostResponseFromFullPost_ShouldHandleNullUserProfileIdGracefully()
  {
    // Arrange
    var authorProfile = TestDataBuilder.CreateProfile(id: "author-1", userId: "user-1");
    var voterProfile = TestDataBuilder.CreateProfile(id: "voter-1", userId: "user-2");
    var post = TestDataBuilder.CreatePost(
        authorId: authorProfile.Id,
        title: "Test Post",
        content: "Test Content",
        id: "post-1"
    );
    var upvote = TestDataBuilder.CreatePostVote(postId: post.Id, profileId: voterProfile.Id, isUpvote: true);

    _context.Profiles.AddRange(authorProfile, voterProfile);
    _context.Posts.Add(post);
    _context.PostVotes.Add(upvote);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Posts
        .FullyPopulatedPostQuery()
        .SelectPostResponseFromFullPost(userProfileId: null)
        .FirstAsync();

    // Assert - Anonymous user should see no vote flags
    result.IsUpvoted.ShouldBeFalse();
    result.IsDownvoted.ShouldBeFalse();
  }

  [Fact]
  public async Task SelectPostResponseFromFullPost_ShouldMapTagsCorrectly()
  {
    // Arrange
    var authorProfile = TestDataBuilder.CreateProfile(id: "author-1", userId: "user-1");
    var post = TestDataBuilder.CreatePost(
        authorId: authorProfile.Id,
        title: "Test Post",
        content: "Test Content",
        id: "post-1"
    );
    var tag1 = TestDataBuilder.CreateTag(englishName: "Technology", arabicName: "تكنولوجيا", id: "tag-1");
    var tag2 = TestDataBuilder.CreateTag(englishName: "Programming", arabicName: "برمجة", id: "tag-2");
    var postTag1 = TestDataBuilder.CreatePostTag(postId: post.Id, tagId: tag1.Id);
    var postTag2 = TestDataBuilder.CreatePostTag(postId: post.Id, tagId: tag2.Id);

    _context.Profiles.Add(authorProfile);
    _context.Posts.Add(post);
    _context.Tags.AddRange(tag1, tag2);
    _context.PostTags.AddRange(postTag1, postTag2);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Posts
        .FullyPopulatedPostQuery()
        .SelectPostResponseFromFullPost(userProfileId: null)
        .FirstAsync();

    // Assert
    result.Tags.ShouldNotBeEmpty();
    result.Tags.Count.ShouldBe(2);
    result.Tags.Any(t => t.EnglishName == "Technology").ShouldBeTrue();
    result.Tags.Any(t => t.EnglishName == "Programming").ShouldBeTrue();
  }

  [Fact]
  public async Task SelectPostResponseFromFullPost_ShouldMapMediasCorrectly()
  {
    // Arrange
    var authorProfile = TestDataBuilder.CreateProfile(id: "author-1", userId: "user-1");
    var post = TestDataBuilder.CreatePost(
        authorId: authorProfile.Id,
        title: "Test Post",
        content: "Test Content",
        id: "post-1"
    );
    var media1 = TestDataBuilder.CreatePostMedia(postId: post.Id, type: "image/jpeg", key: "image1.jpg");
    var media2 = TestDataBuilder.CreatePostMedia(postId: post.Id, type: "video/mp4", key: "video1.mp4");

    _context.Profiles.Add(authorProfile);
    _context.Posts.Add(post);
    _context.PostMedias.AddRange(
        (ExpertBridge.Core.Entities.Media.PostMedia.PostMedia)media1,
        (ExpertBridge.Core.Entities.Media.PostMedia.PostMedia)media2
    );
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Posts
        .FullyPopulatedPostQuery()
        .SelectPostResponseFromFullPost(userProfileId: null)
        .FirstAsync();

    // Assert
    result.Medias.ShouldNotBeEmpty();
    result.Medias.Count.ShouldBe(2);
    result.Medias.Any(m => m.Url.Contains("image1.jpg", StringComparison.Ordinal)).ShouldBeTrue();
    result.Medias.Any(m => m.Url.Contains("video1.mp4", StringComparison.Ordinal)).ShouldBeTrue();
  }

  [Fact]
  public async Task SelectPostResponseFromFullPost_ShouldProjectAllBasicFields()
  {
    // Arrange
    var authorProfile = TestDataBuilder.CreateProfile(
        id: "author-1",
        userId: "user-1",
        firstName: "John",
        lastName: "Doe"
    );
    var updatedDate = DateTime.UtcNow.AddDays(-1);
    var post = TestDataBuilder.CreatePost(
        authorId: authorProfile.Id,
        title: "My Test Post",
        content: "This is the content of my test post",
        language: "en",
        updatedAt: updatedDate,
        id: "post-1"
    );

    _context.Profiles.Add(authorProfile);
    _context.Posts.Add(post);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Posts
        .FullyPopulatedPostQuery()
        .SelectPostResponseFromFullPost(userProfileId: null)
        .FirstAsync();

    // Assert
    result.Id.ShouldBe("post-1");
    result.Title.ShouldBe("My Test Post");
    result.Content.ShouldBe("This is the content of my test post");
    result.Language.ShouldBe("en");
    result.CreatedAt.ShouldNotBeNull(); // CreatedAt is auto-stamped by DbContext
    result.LastModified.ShouldBe(updatedDate);
    result.Author.ShouldNotBeNull();
    result.Author.FirstName.ShouldBe("John");
    result.Author.LastName.ShouldBe("Doe");
  }

  [Fact]
  public async Task SelectPostResponseFromFullPost_ShouldHandleEmptyResultSet()
  {
    // Act
    var results = await _context.Posts
        .FullyPopulatedPostQuery()
        .SelectPostResponseFromFullPost(userProfileId: null)
        .ToListAsync();

    // Assert
    results.ShouldBeEmpty();
  }
}
