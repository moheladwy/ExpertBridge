// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Queries;

/// <summary>
///     Unit tests for JobQueries extension methods that project Job entities to JobResponse DTOs.
/// </summary>
public sealed class JobQueriesTests : IDisposable
{
    private readonly ExpertBridgeDbContext _context;

    public JobQueriesTests()
    {
        _context = InMemoryDbContextFixture.Create();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SelectJobResponseFromFullJob_ShouldProjectBasicProperties()
    {
        // Arrange
        var author = TestDataBuilder.CreateProfile("user1", firstName: "John", lastName: "Doe");
        var worker = TestDataBuilder.CreateProfile("user2", firstName: "Jane", lastName: "Smith");
        var job = TestDataBuilder.CreateJob(
            author.Id,
            worker.Id,
            "Website Development",
            "Build a modern website",
            "New York",
            actualCost: 5000.00m,
            id: "job1"
        );

        await _context.Profiles.AddRangeAsync(author, worker);
        await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Jobs
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .SelectJobResponseFromFullJob()
            .FirstAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe("job1");
        result.Title.ShouldBe("Website Development");
        result.Description.ShouldBe("Build a modern website");
        result.Area.ShouldBe("New York");
        result.ActualCost.ShouldBe(5000.00m);
    }

    [Fact]
    public async Task SelectJobResponseFromFullJob_ShouldProjectAuthorDetails()
    {
        // Arrange
        var author = TestDataBuilder.CreateProfile(
            "user1",
            firstName: "John",
            lastName: "Doe",
            username: "johndoe",
            jobTitle: "Project Manager",
            profilePictureUrl: "https://example.com/john.jpg"
        );
        var worker = TestDataBuilder.CreateProfile("user2", firstName: "Jane", lastName: "Smith");
        var job = TestDataBuilder.CreateJob(author.Id, worker.Id);

        await _context.Profiles.AddRangeAsync(author, worker);
        await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Jobs
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .SelectJobResponseFromFullJob()
            .FirstAsync();

        // Assert
        result.Author.ShouldNotBeNull();
        result.Author.Id.ShouldBe(author.Id);
        result.Author.UserId.ShouldBe("user1");
        result.Author.FirstName.ShouldBe("John");
        result.Author.LastName.ShouldBe("Doe");
        result.Author.Username.ShouldBe("johndoe");
        result.Author.JobTitle.ShouldBe("Project Manager");
        result.Author.ProfilePictureUrl.ShouldBe("https://example.com/john.jpg");
    }

    [Fact]
    public async Task SelectJobResponseFromFullJob_ShouldProjectWorkerDetails()
    {
        // Arrange
        var author = TestDataBuilder.CreateProfile("user1", firstName: "John", lastName: "Doe");
        var worker = TestDataBuilder.CreateProfile(
            "user2",
            firstName: "Jane",
            lastName: "Smith",
            username: "janesmith",
            jobTitle: "Full Stack Developer",
            profilePictureUrl: "https://example.com/jane.jpg"
        );
        var job = TestDataBuilder.CreateJob(author.Id, worker.Id);

        await _context.Profiles.AddRangeAsync(author, worker);
        await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Jobs
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .SelectJobResponseFromFullJob()
            .FirstAsync();

        // Assert
        result.Worker.ShouldNotBeNull();
        result.Worker.Id.ShouldBe(worker.Id);
        result.Worker.UserId.ShouldBe("user2");
        result.Worker.FirstName.ShouldBe("Jane");
        result.Worker.LastName.ShouldBe("Smith");
        result.Worker.Username.ShouldBe("janesmith");
        result.Worker.JobTitle.ShouldBe("Full Stack Developer");
        result.Worker.ProfilePictureUrl.ShouldBe("https://example.com/jane.jpg");
    }

    [Fact]
    public async Task SelectJobResponseFromFullJob_ShouldProjectStatusAndFlags()
    {
        // Arrange
        var author = TestDataBuilder.CreateProfile("user1");
        var worker = TestDataBuilder.CreateProfile("user2");
        var job = TestDataBuilder.CreateJob(
            author.Id,
            worker.Id,
            isCompleted: true,
            isPaid: true,
            status: JobStatusEnum.Completed
        );

        await _context.Profiles.AddRangeAsync(author, worker);
        await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Jobs
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .SelectJobResponseFromFullJob()
            .FirstAsync();

        // Assert
        result.IsCompleted.ShouldBeTrue();
        result.IsPaid.ShouldBeTrue();
        result.Status.ShouldBe(JobStatusEnum.Completed);
    }

    [Fact]
    public async Task SelectJobResponseFromFullJob_ShouldProjectDates()
    {
        // Arrange
        var startedAt = DateTime.UtcNow.AddDays(-10);
        var endedAt = DateTime.UtcNow.AddDays(-2);
        var updatedAt = DateTime.UtcNow.AddDays(-1);

        var author = TestDataBuilder.CreateProfile("user1");
        var worker = TestDataBuilder.CreateProfile("user2");
        var job = TestDataBuilder.CreateJob(
            author.Id,
            worker.Id,
            startedAt: startedAt,
            endedAt: endedAt,
            updatedAt: updatedAt
        );

        await _context.Profiles.AddRangeAsync(author, worker);
        await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Jobs
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .SelectJobResponseFromFullJob()
            .FirstAsync();

        // Assert
        result.StartedAt.ShouldBe(startedAt);
        result.EndedAt.ShouldBe(endedAt);
        result.UpdatedAt.ShouldBe(updatedAt);
    }

    [Fact]
    public async Task SelectJobResponseFromFullJob_ShouldProjectMultipleJobs()
    {
        // Arrange
        var author = TestDataBuilder.CreateProfile("user1", firstName: "John");
        var worker1 = TestDataBuilder.CreateProfile("user2", firstName: "Jane");
        var worker2 = TestDataBuilder.CreateProfile("user3", firstName: "Bob");

        var job1 = TestDataBuilder.CreateJob(
            author.Id,
            worker1.Id,
            "Job 1",
            id: "job1"
        );
        var job2 = TestDataBuilder.CreateJob(
            author.Id,
            worker2.Id,
            "Job 2",
            id: "job2"
        );

        await _context.Profiles.AddRangeAsync(author, worker1, worker2);
        await _context.Jobs.AddRangeAsync(job1, job2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.Jobs
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .Where(j => j.AuthorId == author.Id)
            .OrderBy(j => j.Id)
            .SelectJobResponseFromFullJob()
            .ToListAsync();

        // Assert
        results.ShouldNotBeNull();
        results.Count.ShouldBe(2);
        results[0].Id.ShouldBe("job1");
        results[0].Title.ShouldBe("Job 1");
        results[0].Worker.FirstName.ShouldBe("Jane");
        results[1].Id.ShouldBe("job2");
        results[1].Title.ShouldBe("Job 2");
        results[1].Worker.FirstName.ShouldBe("Bob");
    }

    [Fact]
    public async Task SelectJobResponseFromFullJob_ShouldHandleNullProfilePictures()
    {
        // Arrange
        var author = TestDataBuilder.CreateProfile("user1", profilePictureUrl: null);
        var worker = TestDataBuilder.CreateProfile("user2", profilePictureUrl: null);
        var job = TestDataBuilder.CreateJob(author.Id, worker.Id);

        await _context.Profiles.AddRangeAsync(author, worker);
        await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Jobs
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .SelectJobResponseFromFullJob()
            .FirstAsync();

        // Assert
        result.Author.ProfilePictureUrl.ShouldBeNull();
        result.Worker.ProfilePictureUrl.ShouldBeNull();
    }

    [Fact]
    public async Task SelectJobResponseFromFullJob_ShouldReturnEmptyForNoJobs()
    {
        // Act
        var results = await _context.Jobs
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .SelectJobResponseFromFullJob()
            .ToListAsync();

        // Assert
        results.ShouldBeEmpty();
    }

    [Fact]
    public async Task SelectJobResponseFromFullJob_ShouldProjectChatId()
    {
        // Arrange
        var author = TestDataBuilder.CreateProfile("user1");
        var worker = TestDataBuilder.CreateProfile("user2");
        var job = TestDataBuilder.CreateJob(
            author.Id,
            worker.Id,
            chatId: "chat123"
        );

        await _context.Profiles.AddRangeAsync(author, worker);
        await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Jobs
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .SelectJobResponseFromFullJob()
            .FirstAsync();

        // Assert
        result.ChatId.ShouldBe("chat123");
    }
}
