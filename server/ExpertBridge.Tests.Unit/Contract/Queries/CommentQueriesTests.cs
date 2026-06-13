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
            authorProfile.Id,
            "This is a test comment",
            "post-123",
            id: "comment-1"
        );

        _context.Profiles.Add(authorProfile);
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(null)
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
        var comment = TestDataBuilder.CreateComment(authorProfile.Id, postId: "post-123");

        _context.Profiles.Add(authorProfile);
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(null)
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
        var comment = TestDataBuilder.CreateComment(authorProfile.Id, postId: "post-123", id: "comment-1");

        // Create 3 upvotes and 2 downvotes
        var upvote1 = TestDataBuilder.CreateCommentVote("comment-1", "voter-1");
        var upvote2 = TestDataBuilder.CreateCommentVote("comment-1", "voter-2");
        var upvote3 = TestDataBuilder.CreateCommentVote("comment-1", "voter-3");
        var downvote1 = TestDataBuilder.CreateCommentVote("comment-1", "voter-4", false);
        var downvote2 = TestDataBuilder.CreateCommentVote("comment-1", "voter-5", false);

        comment.Votes = [upvote1, upvote2, upvote3, downvote1, downvote2];

        _context.Profiles.Add(authorProfile);
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(null)
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
        var comment = TestDataBuilder.CreateComment(authorProfile.Id, postId: "post-123", id: "comment-1");
        var userUpvote = TestDataBuilder.CreateCommentVote("comment-1", "current-user-profile");

        comment.Votes = [userUpvote];

        _context.Profiles.AddRange(authorProfile, currentUserProfile);
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment("current-user-profile")
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
        var comment = TestDataBuilder.CreateComment(authorProfile.Id, postId: "post-123", id: "comment-1");
        var userDownvote = TestDataBuilder.CreateCommentVote("comment-1", "current-user-profile", false);

        comment.Votes = [userDownvote];

        _context.Profiles.AddRange(authorProfile, currentUserProfile);
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment("current-user-profile")
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
            authorProfile.Id,
            postId: "post-123",
            id: "parent-comment",
            content: "Parent comment"
        );

        var reply1 = TestDataBuilder.CreateComment(
            replyAuthorProfile.Id,
            postId: "post-123",
            id: "reply-1",
            content: "Reply 1",
            parentCommentId: "parent-comment"
        );

        var reply2 = TestDataBuilder.CreateComment(
            replyAuthorProfile.Id,
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
            .SelectCommentResponseFromFullComment(null)
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
            authorProfile.Id,
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
            .SelectCommentResponseFromFullComment(null)
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
            authorProfile.Id,
            postId: "post-123",
            id: "parent-1",
            content: "Parent 1"
        );

        var parentComment2 = TestDataBuilder.CreateComment(
            authorProfile.Id,
            postId: "post-123",
            id: "parent-2",
            content: "Parent 2"
        );

        var reply = TestDataBuilder.CreateComment(
            authorProfile.Id,
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
            .SelectCommentResponseFromFullComment(null)
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
            authorProfile.Id,
            jobPostingId: "job-posting-456",
            content: "Comment on job posting"
        );

        _context.Profiles.Add(authorProfile);
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Comments
            .FullyPopulatedCommentQuery()
            .SelectCommentResponseFromFullComment(null)
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
            .SelectCommentResponseFromFullComment(null)
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
            authorProfile.Id,
            postId: "post-123",
            id: "comment-1",
            content: "Comment for post 123"
        );

        var comment2 = TestDataBuilder.CreateComment(
            authorProfile.Id,
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
            .SelectCommentResponseFromFullComment(null)
            .ToListAsync();

        // Assert
        results.Count.ShouldBe(1);
        results.First().PostId.ShouldBe("post-123");
        results.First().Content.ShouldBe("Comment for post 123");
    }

    // Mirrors the flat load performed by CommentService.GetCommentsByPostAsync (without the relational-only
    // AsSplitQuery, which the InMemory provider ignores). The resulting list is fed to BuildCommentTree.
    private Task<List<Comment>> LoadAllForPostAsync(string postId)
    {
        return _context.Comments
            .AsNoTracking()
            .Where(c => c.PostId == postId)
            .Include(c => c.Author)
            .Include(c => c.Votes)
            .Include(c => c.Medias)
            .ToListAsync();
    }

    [Fact]
    public async Task BuildCommentTree_ShouldNestRepliesAtAllDepths()
    {
        // Arrange — a depth-3 chain: root -> child -> grandchild (the old eager-Include path dropped depth >= 2).
        var author = TestDataBuilder.CreateProfile(id: "author-profile-1", userId: "author-user-1");
        var root = TestDataBuilder.CreateComment(author.Id, "root", "post-1", id: "root");
        var child = TestDataBuilder.CreateComment(author.Id, "child", "post-1", parentCommentId: "root", id: "child");
        var grandchild =
            TestDataBuilder.CreateComment(author.Id, "grandchild", "post-1", parentCommentId: "child", id: "grandchild");

        _context.Profiles.Add(author);
        _context.Comments.AddRange(root, child, grandchild);
        await _context.SaveChangesAsync();

        // Act
        var tree = (await LoadAllForPostAsync("post-1")).BuildCommentTree(null);

        // Assert
        tree.Count.ShouldBe(1);
        tree[0].Id.ShouldBe("root");
        tree[0].Replies.Count.ShouldBe(1);
        tree[0].Replies[0].Id.ShouldBe("child");
        tree[0].Replies[0].Replies.Count.ShouldBe(1);
        tree[0].Replies[0].Replies[0].Id.ShouldBe("grandchild");
        tree[0].Replies[0].Replies[0].Author.ShouldNotBeNull();
    }

    [Fact]
    public async Task BuildCommentTree_ShouldProjectVoteStateOnNestedReplies()
    {
        // Arrange — a vote cast by the current user on a depth-2 reply.
        var author = TestDataBuilder.CreateProfile(id: "author-profile-1", userId: "author-user-1");
        var currentUser = TestDataBuilder.CreateProfile(id: "current-user-profile", userId: "current-user");
        var root = TestDataBuilder.CreateComment(author.Id, "root", "post-1", id: "root");
        var reply = TestDataBuilder.CreateComment(author.Id, "reply", "post-1", parentCommentId: "root", id: "reply");
        reply.Votes = [TestDataBuilder.CreateCommentVote("reply", "current-user-profile")];

        _context.Profiles.AddRange(author, currentUser);
        _context.Comments.AddRange(root, reply);
        await _context.SaveChangesAsync();

        // Act
        var tree = (await LoadAllForPostAsync("post-1")).BuildCommentTree("current-user-profile");

        // Assert
        var projectedReply = tree[0].Replies[0];
        projectedReply.Upvotes.ShouldBe(1);
        projectedReply.Downvotes.ShouldBe(0);
        projectedReply.IsUpvoted.ShouldBeTrue();
        projectedReply.IsDownvoted.ShouldBeFalse();
    }

    [Fact]
    public void BuildCommentTree_ShouldOrderEachLevelByCreatedAtAscending()
    {
        // Arrange — built directly in memory (not via the DbContext, which would overwrite CreatedAt with UtcNow
        // on save) so explicit, out-of-order timestamps drive the ordering assertion. BuildCommentTree is a pure
        // function over a flat comment list, so no database is needed.
        var author = TestDataBuilder.CreateProfile(id: "author-profile-1", userId: "author-user-1");

        Comment Make(string id, string? parentId, DateTime createdAt)
        {
            var comment = TestDataBuilder.CreateComment(
                author.Id, id, "post-1", parentCommentId: parentId, id: id, createdAt: createdAt);
            comment.Author = author;
            return comment;
        }

        var newerRoot = Make("newer-root", null, new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc));
        var olderRoot = Make("older-root", null, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var laterReply = Make("later-reply", "older-root", new DateTime(2024, 2, 2, 0, 0, 0, DateTimeKind.Utc));
        var earlierReply = Make("earlier-reply", "older-root", new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc));

        var flat = new List<Comment> { newerRoot, olderRoot, laterReply, earlierReply };

        // Act
        var tree = flat.BuildCommentTree(null);

        // Assert
        tree.Select(c => c.Id).ShouldBe(["older-root", "newer-root"]);
        tree[0].Replies.Select(r => r.Id).ShouldBe(["earlier-reply", "later-reply"]);
    }

    [Fact]
    public void BuildCommentTree_ShouldReturnEmptyForNoComments()
    {
        // Act
        var tree = new List<Comment>().BuildCommentTree(null);

        // Assert
        tree.ShouldBeEmpty();
    }

    [Fact]
    public async Task BuildCommentTree_ShouldOmitRepliesWhoseParentIsSoftDeleted()
    {
        // Arrange — root-1 (with a reply) plus an unrelated root-2. After soft-deleting root-1 the global query
        // filter excludes it, so its reply becomes an orphan unreachable from any root.
        var author = TestDataBuilder.CreateProfile(id: "author-profile-1", userId: "author-user-1");
        var deletedRoot = TestDataBuilder.CreateComment(author.Id, "root-1", "post-1", id: "root-1");
        var orphanReply =
            TestDataBuilder.CreateComment(author.Id, "reply", "post-1", parentCommentId: "root-1", id: "reply-1");
        var survivingRoot = TestDataBuilder.CreateComment(author.Id, "root-2", "post-1", id: "root-2");

        _context.Profiles.Add(author);
        _context.Comments.AddRange(deletedRoot, orphanReply, survivingRoot);
        await _context.SaveChangesAsync();

        deletedRoot.IsDeleted = true;
        deletedRoot.DeletedAt = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc);
        await _context.SaveChangesAsync();

        // Act
        var tree = (await LoadAllForPostAsync("post-1")).BuildCommentTree(null);

        // Assert
        tree.Count.ShouldBe(1);
        tree[0].Id.ShouldBe("root-2");
        tree[0].Replies.ShouldBeEmpty();
    }
}
