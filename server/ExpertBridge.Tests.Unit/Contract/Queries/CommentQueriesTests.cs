// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Queries;

/// <summary>
///     Tests for <see cref="CommentQueries" /> extension methods.
/// </summary>
public sealed class CommentQueriesTests : IDisposable
{
    private readonly ExpertBridgeDbContext _context;

    public CommentQueriesTests()
    {
        _context = InMemoryDbContextFixture.Create();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SelectCommentResponseFromFullComment_ShouldProjectBasicProperties()
    {
        // Arrange
        var authorProfile = TestDataBuilder.CreateProfile(id: "author-profile-1", userId: "author-user-1");
        var comment = TestDataBuilder.CreateComment(
            authorId: authorProfile.Id,
            content: "This is a test comment",
            postId: "post-123",
            id: "comment-1"
        );

        _context.Profiles.Add(authorProfile);
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(userProfileId: null)
            .FirstAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe("comment-1");
        result.Content.ShouldBe("This is a test comment");
        result.PostId.ShouldBe("post-123");
        result.AuthorId.ShouldBe(authorProfile.Id);
        result.ParentCommentId.ShouldBeNull();
        result.JobPostingId.ShouldBeNull();
    }

    [Fact]
    public async Task SelectCommentResponseFromFullComment_ShouldProjectAuthorDetails()
    {
        // Arrange
        var authorProfile = TestDataBuilder.CreateProfile(
            id: "author-profile-1",
            userId: "author-user-1",
            firstName: "John",
            lastName: "Doe",
            username: "johndoe",
            jobTitle: "Software Engineer",
            profilePictureUrl: "https://example.com/john.jpg"
        );
        var comment = TestDataBuilder.CreateComment(authorId: authorProfile.Id, postId: "post-123");

        _context.Profiles.Add(authorProfile);
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(userProfileId: null)
            .FirstAsync();

        // Assert
        result.Author.ShouldNotBeNull();
        result.Author.Id.ShouldBe(authorProfile.Id);
        result.Author.UserId.ShouldBe("author-user-1");
        result.Author.FirstName.ShouldBe("John");
        result.Author.LastName.ShouldBe("Doe");
        result.Author.Username.ShouldBe("johndoe");
        result.Author.JobTitle.ShouldBe("Software Engineer");
        result.Author.ProfilePictureUrl.ShouldBe("https://example.com/john.jpg");
    }

    [Fact]
    public async Task SelectCommentResponseFromFullComment_ShouldCalculateVoteCounts()
    {
        // Arrange
        var authorProfile = TestDataBuilder.CreateProfile(id: "author-profile-1", userId: "author-user-1");
        var comment = TestDataBuilder.CreateComment(authorId: authorProfile.Id, postId: "post-123", id: "comment-1");

        // Create 3 upvotes and 2 downvotes
        var upvote1 = TestDataBuilder.CreateCommentVote(commentId: "comment-1", profileId: "voter-1", isUpvote: true);
        var upvote2 = TestDataBuilder.CreateCommentVote(commentId: "comment-1", profileId: "voter-2", isUpvote: true);
        var upvote3 = TestDataBuilder.CreateCommentVote(commentId: "comment-1", profileId: "voter-3", isUpvote: true);
        var downvote1 = TestDataBuilder.CreateCommentVote(commentId: "comment-1", profileId: "voter-4", isUpvote: false);
        var downvote2 = TestDataBuilder.CreateCommentVote(commentId: "comment-1", profileId: "voter-5", isUpvote: false);

        comment.Votes = [upvote1, upvote2, upvote3, downvote1, downvote2];

        _context.Profiles.Add(authorProfile);
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(userProfileId: null)
            .FirstAsync();

        // Assert
        result.Upvotes.ShouldBe(3);
        result.Downvotes.ShouldBe(2);
    }

    [Fact]
    public async Task SelectCommentResponseFromFullComment_ShouldIndicateUserUpvoteState()
    {
        // Arrange
        var authorProfile = TestDataBuilder.CreateProfile(id: "author-profile-1", userId: "author-user-1");
        var currentUserProfile = TestDataBuilder.CreateProfile(id: "current-user-profile", userId: "current-user");
        var comment = TestDataBuilder.CreateComment(authorId: authorProfile.Id, postId: "post-123", id: "comment-1");
        var userUpvote = TestDataBuilder.CreateCommentVote(commentId: "comment-1", profileId: "current-user-profile", isUpvote: true);

        comment.Votes = [userUpvote];

        _context.Profiles.AddRange(authorProfile, currentUserProfile);
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(userProfileId: "current-user-profile")
            .FirstAsync();

        // Assert
        result.IsUpvoted.ShouldBeTrue();
        result.IsDownvoted.ShouldBeFalse();
    }

    [Fact]
    public async Task SelectCommentResponseFromFullComment_ShouldIndicateUserDownvoteState()
    {
        // Arrange
        var authorProfile = TestDataBuilder.CreateProfile(id: "author-profile-1", userId: "author-user-1");
        var currentUserProfile = TestDataBuilder.CreateProfile(id: "current-user-profile", userId: "current-user");
        var comment = TestDataBuilder.CreateComment(authorId: authorProfile.Id, postId: "post-123", id: "comment-1");
        var userDownvote = TestDataBuilder.CreateCommentVote(commentId: "comment-1", profileId: "current-user-profile", isUpvote: false);

        comment.Votes = [userDownvote];

        _context.Profiles.AddRange(authorProfile, currentUserProfile);
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(userProfileId: "current-user-profile")
            .FirstAsync();

        // Assert
        result.IsUpvoted.ShouldBeFalse();
        result.IsDownvoted.ShouldBeTrue();
    }

    [Fact]
    public async Task SelectCommentResponseFromFullComment_ShouldProjectNestedReplies()
    {
        // Arrange
        var authorProfile = TestDataBuilder.CreateProfile(id: "author-profile-1", userId: "author-user-1");
        var replyAuthorProfile = TestDataBuilder.CreateProfile(id: "reply-author-profile", userId: "reply-author-user");

        var parentComment = TestDataBuilder.CreateComment(
            authorId: authorProfile.Id,
            postId: "post-123",
            id: "parent-comment",
            content: "Parent comment"
        );

        var reply1 = TestDataBuilder.CreateComment(
            authorId: replyAuthorProfile.Id,
            postId: "post-123",
            id: "reply-1",
            content: "Reply 1",
            parentCommentId: "parent-comment"
        );

        var reply2 = TestDataBuilder.CreateComment(
            authorId: replyAuthorProfile.Id,
            postId: "post-123",
            id: "reply-2",
            content: "Reply 2",
            parentCommentId: "parent-comment"
        );

        parentComment.Replies = [reply1, reply2];

        _context.Profiles.AddRange(authorProfile, replyAuthorProfile);
        _context.Comments.AddRange(parentComment, reply1, reply2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(userProfileId: null)
            .FirstAsync();

        // Assert
        result.Replies.ShouldNotBeNull();
        result.Replies.Count.ShouldBe(2);
        result.Replies.First().Content.ShouldBe("Reply 1");
        result.Replies.Last().Content.ShouldBe("Reply 2");
        result.Replies.First().ParentCommentId.ShouldBe("parent-comment");
        result.Replies.First().Author.ShouldNotBeNull();
        result.Replies.First().Author!.Id.ShouldBe(replyAuthorProfile.Id);
    }

    [Fact]
    public async Task SelectCommentResponseFromFullComment_ShouldProjectDates()
    {
        // Arrange
        var authorProfile = TestDataBuilder.CreateProfile(id: "author-profile-1", userId: "author-user-1");
        var comment = TestDataBuilder.CreateComment(
            authorId: authorProfile.Id,
            postId: "post-123"
        );

        _context.Profiles.Add(authorProfile);
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Set UpdatedAt after save to test projection
        comment.UpdatedAt = new DateTime(2024, 1, 16, 14, 45, 0, DateTimeKind.Utc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(userProfileId: null)
            .FirstAsync();

        // Assert
        result.CreatedAt.ShouldNotBe(default);
        result.LastModified.ShouldBe(new DateTime(2024, 1, 16, 14, 45, 0, DateTimeKind.Utc));
    }
    [Fact]
    public async Task FullyPopulatedCommentQuery_ShouldFilterTopLevelCommentsOnly()
    {
        // Arrange
        var authorProfile = TestDataBuilder.CreateProfile(id: "author-profile-1", userId: "author-user-1");

        var parentComment1 = TestDataBuilder.CreateComment(
            authorId: authorProfile.Id,
            postId: "post-123",
            id: "parent-1",
            content: "Parent 1"
        );

        var parentComment2 = TestDataBuilder.CreateComment(
            authorId: authorProfile.Id,
            postId: "post-123",
            id: "parent-2",
            content: "Parent 2"
        );

        var reply = TestDataBuilder.CreateComment(
            authorId: authorProfile.Id,
            postId: "post-123",
            id: "reply-1",
            content: "Reply to parent 1",
            parentCommentId: "parent-1"
        );

        _context.Profiles.Add(authorProfile);
        _context.Comments.AddRange(parentComment1, parentComment2, reply);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(userProfileId: null)
            .ToListAsync();

        // Assert
        results.Count.ShouldBe(2); // Only parent comments, reply is nested
        results.ShouldAllBe(c => c.ParentCommentId == null);
    }

    [Fact]
    public async Task SelectCommentResponseFromFullComment_ShouldHandleJobPostingComments()
    {
        // Arrange
        var authorProfile = TestDataBuilder.CreateProfile(id: "author-profile-1", userId: "author-user-1");
        var comment = TestDataBuilder.CreateComment(
            authorId: authorProfile.Id,
            jobPostingId: "job-posting-456",
            content: "Comment on job posting"
        );

        _context.Profiles.Add(authorProfile);
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(userProfileId: null)
            .FirstAsync();

        // Assert
        result.JobPostingId.ShouldBe("job-posting-456");
        result.PostId.ShouldBeNull();
    }

    [Fact]
    public async Task SelectCommentResponseFromFullComment_ShouldReturnEmptyForNoComments()
    {
        // Act
        var results = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(userProfileId: null)
            .ToListAsync();

        // Assert
        results.ShouldBeEmpty();
    }

    [Fact]
    public async Task FullyPopulatedCommentQuery_WithPredicate_ShouldFilterCorrectly()
    {
        // Arrange
        var authorProfile = TestDataBuilder.CreateProfile(id: "author-profile-1", userId: "author-user-1");

        var comment1 = TestDataBuilder.CreateComment(
            authorId: authorProfile.Id,
            postId: "post-123",
            id: "comment-1",
            content: "Comment for post 123"
        );

        var comment2 = TestDataBuilder.CreateComment(
            authorId: authorProfile.Id,
            postId: "post-456",
            id: "comment-2",
            content: "Comment for post 456"
        );

        _context.Profiles.Add(authorProfile);
        _context.Comments.AddRange(comment1, comment2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.Comments
            .FullyPopulatedCommentQuery(c => c.PostId == "post-123")
            .SelectCommentResponseFromFullComment(userProfileId: null)
            .ToListAsync();

        // Assert
        results.Count.ShouldBe(1);
        results.First().PostId.ShouldBe("post-123");
        results.First().Content.ShouldBe("Comment for post 123");
    }
}
