// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Queries;

/// <summary>
///     Unit tests for JobPostingQueries extension methods.
/// </summary>
public sealed class JobPostingQueriesTests
{
    [Fact]
    public async Task FullyPopulatedJobPostingQuery_ShouldIncludeAuthor()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Author", "User");
        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "Senior Developer",
            "Looking for senior dev",
            5000m);

        jobPosting.Author = author;
        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .FirstOrDefaultAsync(jp => jp.Id == jobPosting.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Author.ShouldNotBeNull();
        result.Author.Id.ShouldBe(author.Id);
    }

    [Fact]
    public async Task FullyPopulatedJobPostingQuery_ShouldIncludeVotes()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Author", "User");
        var voter = TestDataBuilder.CreateProfile("Voter", "User");
        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "Backend Engineer",
            "Need backend engineer",
            4000m);

        var vote = TestDataBuilder.CreateJobPostingVote(jobPosting.Id, voter.Id);

        jobPosting.Author = author;
        jobPosting.Votes.Add(vote);
        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .FirstOrDefaultAsync(jp => jp.Id == jobPosting.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Votes.ShouldNotBeEmpty();
        result.Votes.Count.ShouldBe(1);
        result.Votes.First().ProfileId.ShouldBe(voter.Id);
    }

    [Fact]
    public async Task FullyPopulatedJobPostingQuery_ShouldIncludeMedias()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Author", "User");
        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "UI Designer",
            "Need UI designer",
            3000m);

        var media = TestDataBuilder.CreateJobPostingMedia(jobPosting.Id, "media-key-1", "image/png");

        jobPosting.Author = author;
        jobPosting.Medias.Add(media);
        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .FirstOrDefaultAsync(jp => jp.Id == jobPosting.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Medias.ShouldNotBeEmpty();
        result.Medias.Count.ShouldBe(1);
        result.Medias.First().Key.ShouldBe("media-key-1");
    }

    [Fact]
    public async Task FullyPopulatedJobPostingQuery_ShouldIncludeComments()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Author", "User");
        var commenter = TestDataBuilder.CreateProfile("Commenter", "User");
        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "DevOps Engineer",
            "Need DevOps expert",
            6000m);

        var comment = TestDataBuilder.CreateComment(
            commenter.Id,
            "Interested in this position",
            jobPostingId: jobPosting.Id);

        jobPosting.Author = author;
        jobPosting.Comments.Add(comment);
        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .FirstOrDefaultAsync(jp => jp.Id == jobPosting.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Comments.ShouldNotBeEmpty();
        result.Comments.Count.ShouldBe(1);
    }

    [Fact]
    public async Task FullyPopulatedJobPostingQuery_ShouldIncludeJobApplications()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Hirer", "User");
        var applicant = TestDataBuilder.CreateProfile("Applicant", "User");
        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "Frontend Developer",
            "Need React developer",
            4500m);

        var application = TestDataBuilder.CreateJobApplication(
            jobPosting.Id,
            applicant.Id,
            4000m,
            "I am interested in this position");

        jobPosting.Author = author;
        jobPosting.JobApplications.Add(application);
        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .FirstOrDefaultAsync(jp => jp.Id == jobPosting.Id);

        // Assert
        result.ShouldNotBeNull();
        result.JobApplications.ShouldNotBeEmpty();
        result.JobApplications.Count.ShouldBe(1);
        result.JobApplications.First().ApplicantId.ShouldBe(applicant.Id);
    }

    [Fact]
    public async Task FullyPopulatedJobPostingQuery_ShouldIncludeJobPostingTagsWithTags()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Author", "User");
        var tag = TestDataBuilder.CreateTag("csharp");
        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "C# Developer",
            "Need C# expertise",
            5500m);

        var jobPostingTag = TestDataBuilder.CreateJobPostingTag(jobPosting.Id, tag.Id);
        jobPostingTag.Tag = tag;

        jobPosting.Author = author;
        jobPosting.JobPostingTags.Add(jobPostingTag);
        context.Tags.Add(tag);
        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .FirstOrDefaultAsync(jp => jp.Id == jobPosting.Id);

        // Assert
        result.ShouldNotBeNull();
        result.JobPostingTags.ShouldNotBeEmpty();
        result.JobPostingTags.Count.ShouldBe(1);
        result.JobPostingTags.First().Tag.ShouldNotBeNull();
        result.JobPostingTags.First().Tag.EnglishName.ShouldBe("csharp");
    }

    [Fact]
    public async Task FullyPopulatedJobPostingQuery_WithPredicate_ShouldFilterResults()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Author", "User");
        var jobPosting1 = TestDataBuilder.CreateJobPosting(
            author.Id,
            "Senior Developer",
            "Senior role",
            5000m);

        var jobPosting2 = TestDataBuilder.CreateJobPosting(
            author.Id,
            "Junior Developer",
            "Junior role",
            2000m);

        jobPosting1.Author = author;
        jobPosting2.Author = author;
        context.JobPostings.AddRange(jobPosting1, jobPosting2);
        await context.SaveChangesAsync();

        // Act
        var results = await context.JobPostings
            .FullyPopulatedJobPostingQuery(jp => jp.Budget > 3000m)
            .ToListAsync();

        // Assert
        results.ShouldNotBeEmpty();
        results.Count.ShouldBe(1);
        results.First().Title.ShouldBe("Senior Developer");
    }

    [Fact]
    public async Task SelectJopPostingResponseFromFullJobPosting_ShouldProjectAllBasicFields()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Author", "User");
        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "Full Stack Developer",
            "Looking for full stack developer with React and .NET experience",
            6000m);

        jobPosting.Author = author;
        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .Where(jp => jp.Id == jobPosting.Id)
            .SelectJopPostingResponseFromFullJobPosting(null)
            .FirstOrDefaultAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(jobPosting.Id);
        result.Title.ShouldBe("Full Stack Developer");
        result.Content.ShouldBe("Looking for full stack developer with React and .NET experience");
        result.Budget.ShouldBe(6000m);
        result.CreatedAt.ShouldNotBeNull();
        result.Author.ShouldNotBeNull();
        result.Author.Id.ShouldBe(author.Id);
    }

    [Fact]
    public async Task SelectJopPostingResponseFromFullJobPosting_ShouldCalculateUpvotesAndDownvotes()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Author", "User");
        var voter1 = TestDataBuilder.CreateProfile("Voter1", "User");
        var voter2 = TestDataBuilder.CreateProfile("Voter2", "User");
        var voter3 = TestDataBuilder.CreateProfile("Voter3", "User");

        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "Mobile Developer",
            "iOS and Android development",
            5000m);

        var upvote1 = TestDataBuilder.CreateJobPostingVote(jobPosting.Id, voter1.Id);
        var upvote2 = TestDataBuilder.CreateJobPostingVote(jobPosting.Id, voter2.Id);
        var downvote = TestDataBuilder.CreateJobPostingVote(jobPosting.Id, voter3.Id, false);

        jobPosting.Author = author;
        jobPosting.Votes.Add(upvote1);
        jobPosting.Votes.Add(upvote2);
        jobPosting.Votes.Add(downvote);

        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .Where(jp => jp.Id == jobPosting.Id)
            .SelectJopPostingResponseFromFullJobPosting(null)
            .FirstOrDefaultAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Upvotes.ShouldBe(2);
        result.Downvotes.ShouldBe(1);
    }

    [Fact]
    public async Task SelectJopPostingResponseFromFullJobPosting_ShouldSetIsUpvotedForCurrentUser()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Author", "User");
        var currentUser = TestDataBuilder.CreateProfile("CurrentUser", "User");

        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "Data Scientist",
            "ML and AI expertise needed",
            7000m);

        var upvote = TestDataBuilder.CreateJobPostingVote(jobPosting.Id, currentUser.Id);

        jobPosting.Author = author;
        jobPosting.Votes.Add(upvote);

        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .Where(jp => jp.Id == jobPosting.Id)
            .SelectJopPostingResponseFromFullJobPosting(currentUser.Id)
            .FirstOrDefaultAsync();

        // Assert
        result.ShouldNotBeNull();
        result.IsUpvoted.ShouldBeTrue();
        result.IsDownvoted.ShouldBeFalse();
    }

    [Fact]
    public async Task SelectJopPostingResponseFromFullJobPosting_ShouldSetIsDownvotedForCurrentUser()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Author", "User");
        var currentUser = TestDataBuilder.CreateProfile("CurrentUser", "User");

        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "Blockchain Developer",
            "Web3 and smart contracts",
            8000m);

        var downvote = TestDataBuilder.CreateJobPostingVote(jobPosting.Id, currentUser.Id, false);

        jobPosting.Author = author;
        jobPosting.Votes.Add(downvote);

        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .Where(jp => jp.Id == jobPosting.Id)
            .SelectJopPostingResponseFromFullJobPosting(currentUser.Id)
            .FirstOrDefaultAsync();

        // Assert
        result.ShouldNotBeNull();
        result.IsUpvoted.ShouldBeFalse();
        result.IsDownvoted.ShouldBeTrue();
    }

    [Fact]
    public async Task SelectJopPostingResponseFromFullJobPosting_ShouldSetVoteFlagsToFalseForAnonymousUser()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Author", "User");

        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "Game Developer",
            "Unity and Unreal Engine",
            6500m);

        jobPosting.Author = author;
        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act - null userProfileId for anonymous
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .Where(jp => jp.Id == jobPosting.Id)
            .SelectJopPostingResponseFromFullJobPosting(null)
            .FirstOrDefaultAsync();

        // Assert
        result.ShouldNotBeNull();
        result.IsUpvoted.ShouldBeFalse();
        result.IsDownvoted.ShouldBeFalse();
    }

    [Fact]
    public async Task SelectJopPostingResponseFromFullJobPosting_ShouldCalculateCommentsCountExcludingDeleted()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Author", "User");
        var commenter = TestDataBuilder.CreateProfile("Commenter", "User");

        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "Security Engineer",
            "Cybersecurity expert needed",
            7500m);

        var comment1 = TestDataBuilder.CreateComment(
            commenter.Id,
            "Great opportunity",
            jobPostingId: jobPosting.Id);

        var comment2 = TestDataBuilder.CreateComment(
            commenter.Id,
            "What's the timeline?",
            jobPostingId: jobPosting.Id);

        var deletedComment = TestDataBuilder.CreateComment(
            commenter.Id,
            "Deleted comment",
            jobPostingId: jobPosting.Id);
        deletedComment.IsDeleted = true;

        jobPosting.Author = author;
        jobPosting.Comments.Add(comment1);
        jobPosting.Comments.Add(comment2);
        jobPosting.Comments.Add(deletedComment);

        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .Where(jp => jp.Id == jobPosting.Id)
            .SelectJopPostingResponseFromFullJobPosting(null)
            .FirstOrDefaultAsync();

        // Assert
        result.ShouldNotBeNull();
        // Should count only non-deleted comments (2), not the deleted one
        result.Comments.ShouldBe(2);
    }

    [Fact]
    public async Task SelectJopPostingResponseFromFullJobPosting_ShouldMapTags()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Author", "User");
        var tag1 = TestDataBuilder.CreateTag("react");
        var tag2 = TestDataBuilder.CreateTag("nodejs");

        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "Full Stack JavaScript Developer",
            "MERN stack development",
            5500m);

        var jobPostingTag1 = TestDataBuilder.CreateJobPostingTag(jobPosting.Id, tag1.Id);
        var jobPostingTag2 = TestDataBuilder.CreateJobPostingTag(jobPosting.Id, tag2.Id);
        jobPostingTag1.Tag = tag1;
        jobPostingTag2.Tag = tag2;

        jobPosting.Author = author;
        jobPosting.JobPostingTags.Add(jobPostingTag1);
        jobPosting.JobPostingTags.Add(jobPostingTag2);

        context.Tags.AddRange(tag1, tag2);
        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .Where(jp => jp.Id == jobPosting.Id)
            .SelectJopPostingResponseFromFullJobPosting(null)
            .FirstOrDefaultAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Tags.ShouldNotBeEmpty();
        result.Tags.Count.ShouldBe(2);
        result.Tags.Select(t => t.EnglishName).ShouldContain("react");
        result.Tags.Select(t => t.EnglishName).ShouldContain("nodejs");
    }

    [Fact]
    public async Task SelectJopPostingResponseFromFullJobPosting_ShouldMapMedias()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Author", "User");
        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "Video Editor",
            "Professional video editing",
            4000m);

        var media1 = TestDataBuilder.CreateJobPostingMedia(jobPosting.Id, "media-1", "image/jpeg", "sample1.jpg");
        var media2 = TestDataBuilder.CreateJobPostingMedia(jobPosting.Id, "media-2", "image/png", "sample2.png");

        jobPosting.Author = author;
        jobPosting.Medias.Add(media1);
        jobPosting.Medias.Add(media2);

        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .Where(jp => jp.Id == jobPosting.Id)
            .SelectJopPostingResponseFromFullJobPosting(null)
            .FirstOrDefaultAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Medias.ShouldNotBeEmpty();
        result.Medias.Count.ShouldBe(2);
    }

    [Fact]
    public async Task SelectJopPostingResponseFromFullJobPosting_ShouldSetIsAppliedForWhenUserHasApplied()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Hirer", "User");
        var applicant = TestDataBuilder.CreateProfile("Applicant", "User");

        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "Python Developer",
            "Django and Flask expertise",
            5000m);

        var application = TestDataBuilder.CreateJobApplication(
            jobPosting.Id,
            applicant.Id,
            4500m,
            "I have 5 years of Python experience");

        jobPosting.Author = author;
        jobPosting.JobApplications.Add(application);

        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .Where(jp => jp.Id == jobPosting.Id)
            .SelectJopPostingResponseFromFullJobPosting(applicant.Id)
            .FirstOrDefaultAsync();

        // Assert
        result.ShouldNotBeNull();
        result.IsAppliedFor.ShouldBeTrue();
    }

    [Fact]
    public async Task SelectJopPostingResponseFromFullJobPosting_ShouldSetIsAppliedForToFalseWhenUserHasNotApplied()
    {
        // Arrange
        await using var context = InMemoryDbContextFixture.Create();
        var author = TestDataBuilder.CreateProfile("Hirer", "User");
        var currentUser = TestDataBuilder.CreateProfile("CurrentUser", "User");

        var jobPosting = TestDataBuilder.CreateJobPosting(
            author.Id,
            "Ruby Developer",
            "Rails expertise required",
            4800m);

        jobPosting.Author = author;
        context.JobPostings.Add(jobPosting);
        await context.SaveChangesAsync();

        // Act
        var result = await context.JobPostings
            .FullyPopulatedJobPostingQuery()
            .Where(jp => jp.Id == jobPosting.Id)
            .SelectJopPostingResponseFromFullJobPosting(currentUser.Id)
            .FirstOrDefaultAsync();

        // Assert
        result.ShouldNotBeNull();
        result.IsAppliedFor.ShouldBeFalse();
    }
}
