// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Queries;

/// <summary>
/// Tests for JobApplicationQueries extension methods.
/// </summary>
public sealed class JobApplicationQueriesTests : IDisposable
{
    private readonly ExpertBridgeDbContext _context;

    public JobApplicationQueriesTests()
    {
        _context = InMemoryDbContextFixture.Create();
    }
    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SelectJobApplicationResponseFromEntity_ShouldProjectBasicProperties()
    {
        // Arrange
        var applicant = TestDataBuilder.CreateProfile(
            userId: "user1",
            firstName: "John",
            lastName: "Doe",
            username: "johndoe",
            jobTitle: "Senior Developer",
            profilePictureUrl: "https://example.com/john.jpg"
        );

        var jobPosting = TestDataBuilder.CreateJobPosting(
            authorId: "author1",
            title: "Build a website",
            content: "Need a modern website",
            budget: 5000m
        );

        var application = TestDataBuilder.CreateJobApplication(
            jobPostingId: jobPosting.Id,
            applicantId: applicant.Id,
            offeredCost: 4500m,
            coverLetter: "I'm perfect for this job"
        );

        _context.Profiles.Add(applicant);
        _context.JobPostings.Add(jobPosting);
        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.JobApplications
            .Include(j => j.Applicant)
                .ThenInclude(a => a.Comments)
                    .ThenInclude(c => c.Votes)
            .Where(j => j.Id == application.Id)
            .SelectJobApplicationResponseFromEntity()
            .SingleAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(application.Id);
        result.ApplicantId.ShouldBe(applicant.Id);
        result.JobPostingId.ShouldBe(jobPosting.Id);
        result.OfferedCost.ShouldBe(4500m);
        result.CoverLetter.ShouldBe("I'm perfect for this job");
        result.AppliedAt.ShouldBeGreaterThan(DateTime.MinValue);
    }
    [Fact]
    public async Task SelectJobApplicationResponseFromEntity_ShouldProjectApplicantDetails()
    {
        // Arrange
        var applicant = TestDataBuilder.CreateProfile(
            userId: "user1",
            firstName: "Jane",
            lastName: "Smith",
            username: "janesmith",
            email: "jane@example.com",
            jobTitle: "Full Stack Developer",
            profilePictureUrl: "https://example.com/jane.jpg"
        );

        var jobPosting = TestDataBuilder.CreateJobPosting(
            authorId: "author1",
            title: "API Development",
            content: "Build REST API",
            budget: 3000m
        );

        var application = TestDataBuilder.CreateJobApplication(
            jobPostingId: jobPosting.Id,
            applicantId: applicant.Id,
            offeredCost: 2800m
        );

        _context.Profiles.Add(applicant);
        _context.JobPostings.Add(jobPosting);
        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.JobApplications
            .Include(j => j.Applicant)
                .ThenInclude(a => a.Comments)
                    .ThenInclude(c => c.Votes)
            .Where(j => j.Id == application.Id)
            .SelectJobApplicationResponseFromEntity()
            .SingleAsync();

        // Assert
        result.Applicant.ShouldNotBeNull();
        result.Applicant.Id.ShouldBe(applicant.Id);
        result.Applicant.UserId.ShouldBe("user1");
        result.Applicant.FirstName.ShouldBe("Jane");
        result.Applicant.LastName.ShouldBe("Smith");
        result.Applicant.Username.ShouldBe("janesmith");
        result.Applicant.JobTitle.ShouldBe("Full Stack Developer");
        result.Applicant.ProfilePictureUrl.ShouldBe("https://example.com/jane.jpg");
    }

    [Fact]
    public async Task SelectJobApplicationResponseFromEntity_ShouldCalculateReputationWithNoVotes()
    {
        // Arrange
        var applicant = TestDataBuilder.CreateProfile(
            userId: "user1",
            username: "noreputation"
        );

        var jobPosting = TestDataBuilder.CreateJobPosting(
            authorId: "author1",
            title: "Job",
            content: "Content",
            budget: 1000m
        );

        var application = TestDataBuilder.CreateJobApplication(
            jobPostingId: jobPosting.Id,
            applicantId: applicant.Id,
            offeredCost: 900m
        );

        _context.Profiles.Add(applicant);
        _context.JobPostings.Add(jobPosting);
        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.JobApplications
            .Include(j => j.Applicant)
                .ThenInclude(a => a.Comments)
                    .ThenInclude(c => c.Votes)
            .Where(j => j.Id == application.Id)
            .SelectJobApplicationResponseFromEntity()
            .SingleAsync();

        // Assert
        result.Applicant.ShouldNotBeNull();
        result.Applicant.Reputation.ShouldBe(0);
    }
    [Fact]
    public async Task SelectJobApplicationResponseFromEntity_ShouldCalculateReputationWithOnlyUpvotes()
    {
        // Arrange
        var applicant = TestDataBuilder.CreateProfile(
            userId: "user1",
            username: "popular"
        );

        var comment1 = TestDataBuilder.CreateComment(
            authorId: applicant.Id,
            content: "Great comment",
            postId: "post1"
        );

        var comment2 = TestDataBuilder.CreateComment(
            authorId: applicant.Id,
            content: "Another great comment",
            postId: "post2"
        );

        var upvote1 = TestDataBuilder.CreateCommentVote(comment1.Id, "voter1", isUpvote: true);
        var upvote2 = TestDataBuilder.CreateCommentVote(comment1.Id, "voter2", isUpvote: true);
        var upvote3 = TestDataBuilder.CreateCommentVote(comment2.Id, "voter3", isUpvote: true);

        comment1.Votes = [upvote1, upvote2];
        comment2.Votes = [upvote3];
        applicant.Comments = [comment1, comment2];

        var jobPosting = TestDataBuilder.CreateJobPosting(
            authorId: "author1",
            title: "Job",
            content: "Content",
            budget: 2000m
        );

        var application = TestDataBuilder.CreateJobApplication(
            jobPostingId: jobPosting.Id,
            applicantId: applicant.Id,
            offeredCost: 1800m
        );

        _context.Profiles.Add(applicant);
        _context.Comments.AddRange(comment1, comment2);
        _context.CommentVotes.AddRange(upvote1, upvote2, upvote3);
        _context.JobPostings.Add(jobPosting);
        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.JobApplications
            .Include(j => j.Applicant)
                .ThenInclude(a => a.Comments)
                    .ThenInclude(c => c.Votes)
            .Where(j => j.Id == application.Id)
            .SelectJobApplicationResponseFromEntity()
            .SingleAsync();

        // Assert
        result.Applicant.ShouldNotBeNull();
        result.Applicant.Reputation.ShouldBe(3); // 3 upvotes - 0 downvotes
    }
    [Fact]
    public async Task SelectJobApplicationResponseFromEntity_ShouldCalculateReputationWithMixedVotes()
    {
        // Arrange
        var applicant = TestDataBuilder.CreateProfile(
            userId: "user1",
            username: "controversial"
        );

        var comment = TestDataBuilder.CreateComment(
            authorId: applicant.Id,
            content: "Mixed reactions",
            postId: "post1"
        );

        var upvote1 = TestDataBuilder.CreateCommentVote(comment.Id, "voter1", isUpvote: true);
        var upvote2 = TestDataBuilder.CreateCommentVote(comment.Id, "voter2", isUpvote: true);
        var upvote3 = TestDataBuilder.CreateCommentVote(comment.Id, "voter3", isUpvote: true);
        var downvote1 = TestDataBuilder.CreateCommentVote(comment.Id, "voter4", isUpvote: false);
        var downvote2 = TestDataBuilder.CreateCommentVote(comment.Id, "voter5", isUpvote: false);

        comment.Votes = [upvote1, upvote2, upvote3, downvote1, downvote2];
        applicant.Comments = [comment];

        var jobPosting = TestDataBuilder.CreateJobPosting(
            authorId: "author1",
            title: "Job",
            content: "Content",
            budget: 1500m
        );

        var application = TestDataBuilder.CreateJobApplication(
            jobPostingId: jobPosting.Id,
            applicantId: applicant.Id,
            offeredCost: 1400m
        );

        _context.Profiles.Add(applicant);
        _context.Comments.Add(comment);
        _context.CommentVotes.AddRange(upvote1, upvote2, upvote3, downvote1, downvote2);
        _context.JobPostings.Add(jobPosting);
        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.JobApplications
            .Include(j => j.Applicant)
                .ThenInclude(a => a.Comments)
                    .ThenInclude(c => c.Votes)
            .Where(j => j.Id == application.Id)
            .SelectJobApplicationResponseFromEntity()
            .SingleAsync();

        // Assert
        result.Applicant.ShouldNotBeNull();
        result.Applicant.Reputation.ShouldBe(1); // 3 upvotes - 2 downvotes = 1
    }
    [Fact]
    public async Task SelectJobApplicationResponseFromEntity_ShouldCalculateNegativeReputation()
    {
        // Arrange
        var applicant = TestDataBuilder.CreateProfile(
            userId: "user1",
            username: "unpopular"
        );

        var comment = TestDataBuilder.CreateComment(
            authorId: applicant.Id,
            content: "Unpopular opinion",
            postId: "post1"
        );

        var upvote = TestDataBuilder.CreateCommentVote(comment.Id, "voter1", isUpvote: true);
        var downvote1 = TestDataBuilder.CreateCommentVote(comment.Id, "voter2", isUpvote: false);
        var downvote2 = TestDataBuilder.CreateCommentVote(comment.Id, "voter3", isUpvote: false);
        var downvote3 = TestDataBuilder.CreateCommentVote(comment.Id, "voter4", isUpvote: false);

        comment.Votes = [upvote, downvote1, downvote2, downvote3];
        applicant.Comments = [comment];

        var jobPosting = TestDataBuilder.CreateJobPosting(
            authorId: "author1",
            title: "Job",
            content: "Content",
            budget: 800m
        );

        var application = TestDataBuilder.CreateJobApplication(
            jobPostingId: jobPosting.Id,
            applicantId: applicant.Id,
            offeredCost: 750m
        );

        _context.Profiles.Add(applicant);
        _context.Comments.Add(comment);
        _context.CommentVotes.AddRange(upvote, downvote1, downvote2, downvote3);
        _context.JobPostings.Add(jobPosting);
        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.JobApplications
            .Include(j => j.Applicant)
                .ThenInclude(a => a.Comments)
                    .ThenInclude(c => c.Votes)
            .Where(j => j.Id == application.Id)
            .SelectJobApplicationResponseFromEntity()
            .SingleAsync();

        // Assert
        result.Applicant.ShouldNotBeNull();
        result.Applicant.Reputation.ShouldBe(-2); // 1 upvote - 3 downvotes = -2
    }
    [Fact]
    public async Task SelectJobApplicationResponseFromEntity_ShouldProjectMultipleApplications()
    {
        // Arrange
        var applicant1 = TestDataBuilder.CreateProfile(userId: "user1", username: "applicant1");
        var applicant2 = TestDataBuilder.CreateProfile(userId: "user2", username: "applicant2");

        var jobPosting = TestDataBuilder.CreateJobPosting(
            authorId: "author1",
            title: "Big Project",
            content: "Complex requirements",
            budget: 10000m
        );

        var application1 = TestDataBuilder.CreateJobApplication(
            jobPostingId: jobPosting.Id,
            applicantId: applicant1.Id,
            offeredCost: 9500m,
            coverLetter: "First application"
        );

        var application2 = TestDataBuilder.CreateJobApplication(
            jobPostingId: jobPosting.Id,
            applicantId: applicant2.Id,
            offeredCost: 8500m,
            coverLetter: "Second application"
        );

        _context.Profiles.AddRange(applicant1, applicant2);
        _context.JobPostings.Add(jobPosting);
        _context.JobApplications.AddRange(application1, application2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.JobApplications
            .Include(j => j.Applicant)
                .ThenInclude(a => a.Comments)
                    .ThenInclude(c => c.Votes)
            .Where(j => j.JobPostingId == jobPosting.Id)
            .SelectJobApplicationResponseFromEntity()
            .ToListAsync();

        // Assert
        results.ShouldNotBeNull();
        results.Count.ShouldBe(2);

        var firstApp = results.Single(r => r.ApplicantId == applicant1.Id);
        firstApp.OfferedCost.ShouldBe(9500m);
        firstApp.CoverLetter.ShouldBe("First application");
        firstApp.Applicant.ShouldNotBeNull();
        firstApp.Applicant.Username.ShouldBe("applicant1");

        var secondApp = results.Single(r => r.ApplicantId == applicant2.Id);
        secondApp.OfferedCost.ShouldBe(8500m);
        secondApp.CoverLetter.ShouldBe("Second application");
        secondApp.Applicant.ShouldNotBeNull();
        secondApp.Applicant.Username.ShouldBe("applicant2");
    }
    [Fact]
    public async Task SelectJobApplicationResponseFromEntity_ShouldHandleNullCoverLetter()
    {
        // Arrange
        var applicant = TestDataBuilder.CreateProfile(
            userId: "user1",
            username: "nocoverletter"
        );

        var jobPosting = TestDataBuilder.CreateJobPosting(
            authorId: "author1",
            title: "Job",
            content: "Content",
            budget: 1000m
        );

        var application = TestDataBuilder.CreateJobApplication(
            jobPostingId: jobPosting.Id,
            applicantId: applicant.Id,
            offeredCost: 900m,
            coverLetter: null
        );

        _context.Profiles.Add(applicant);
        _context.JobPostings.Add(jobPosting);
        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.JobApplications
            .Include(j => j.Applicant)
                .ThenInclude(a => a.Comments)
                    .ThenInclude(c => c.Votes)
            .Where(j => j.Id == application.Id)
            .SelectJobApplicationResponseFromEntity()
            .SingleAsync();

        // Assert
        result.CoverLetter.ShouldBeNull();
    }

    [Fact]
    public async Task SelectJobApplicationResponseFromEntity_ShouldReturnEmptyForNoApplications()
    {
        // Act
        var results = await _context.JobApplications
            .Include(j => j.Applicant)
                .ThenInclude(a => a.Comments)
                    .ThenInclude(c => c.Votes)
            .SelectJobApplicationResponseFromEntity()
            .ToListAsync();

        // Assert
        results.ShouldNotBeNull();
        results.ShouldBeEmpty();
    }
}
