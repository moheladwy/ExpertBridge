// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Queries;

/// <summary>
/// Tests for JobOfferQueries extension methods.
/// </summary>
public sealed class JobOfferQueriesTests : IDisposable
{
    private readonly ExpertBridgeDbContext _context;

    public JobOfferQueriesTests()
    {
        _context = InMemoryDbContextFixture.Create();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SelectJobOfferResponseFromEntity_ShouldProjectBasicProperties()
    {
        // Arrange
        var author = TestDataBuilder.CreateProfile(
            userId: "author1",
            firstName: "John",
            lastName: "Doe",
            username: "johndoe"
        );

        var worker = TestDataBuilder.CreateProfile(
            userId: "worker1",
            firstName: "Jane",
            lastName: "Smith",
            username: "janesmith"
        );

        var offer = TestDataBuilder.CreateJobOffer(
            authorId: author.Id,
            workerId: worker.Id,
            title: "Build a Mobile App",
            description: "Need an experienced React Native developer",
            budget: 5000m,
            area: "Remote"
        );

        _context.Profiles.AddRange(author, worker);
        _context.JobOffers.Add(offer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.JobOffers
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .Where(j => j.Id == offer.Id)
            .SelectJobOfferResponseFromEntity()
            .SingleAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(offer.Id);
        result.Title.ShouldBe("Build a Mobile App");
        result.Description.ShouldBe("Need an experienced React Native developer");
        result.Budget.ShouldBe(5000m);
        result.Area.ShouldBe("Remote");
        result.CreatedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task SelectJobOfferResponseFromEntity_ShouldProjectAuthorDetails()
    {
        // Arrange
        var author = TestDataBuilder.CreateProfile(
            userId: "author1",
            firstName: "Alice",
            lastName: "Johnson",
            username: "alicejohnson",
            email: "alice@example.com",
            jobTitle: "Product Manager",
            profilePictureUrl: "https://example.com/alice.jpg"
        );

        var worker = TestDataBuilder.CreateProfile(
            userId: "worker1",
            username: "worker"
        );

        var offer = TestDataBuilder.CreateJobOffer(
            authorId: author.Id,
            workerId: worker.Id,
            title: "UX Design Project",
            description: "Design a new user interface",
            budget: 3000m,
            area: "New York"
        );

        _context.Profiles.AddRange(author, worker);
        _context.JobOffers.Add(offer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.JobOffers
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .Where(j => j.Id == offer.Id)
            .SelectJobOfferResponseFromEntity()
            .SingleAsync();

        // Assert
        result.Author.ShouldNotBeNull();
        result.Author.Id.ShouldBe(author.Id);
        result.Author.UserId.ShouldBe("author1");
        result.Author.FirstName.ShouldBe("Alice");
        result.Author.LastName.ShouldBe("Johnson");
        result.Author.Username.ShouldBe("alicejohnson");
        result.Author.JobTitle.ShouldBe("Product Manager");
        result.Author.ProfilePictureUrl.ShouldBe("https://example.com/alice.jpg");
    }
    [Fact]
    public async Task SelectJobOfferResponseFromEntity_ShouldProjectWorkerDetails()
    {
        // Arrange
        var author = TestDataBuilder.CreateProfile(
            userId: "author1",
            username: "author"
        );

        var worker = TestDataBuilder.CreateProfile(
            userId: "worker1",
            firstName: "Bob",
            lastName: "Williams",
            username: "bobwilliams",
            email: "bob@example.com",
            jobTitle: "Full Stack Developer",
            profilePictureUrl: "https://example.com/bob.jpg"
        );

        var offer = TestDataBuilder.CreateJobOffer(
            authorId: author.Id,
            workerId: worker.Id,
            title: "Backend Development",
            description: "Build REST API",
            budget: 4000m,
            area: "San Francisco"
        );

        _context.Profiles.AddRange(author, worker);
        _context.JobOffers.Add(offer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.JobOffers
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .Where(j => j.Id == offer.Id)
            .SelectJobOfferResponseFromEntity()
            .SingleAsync();

        // Assert
        result.Worker.ShouldNotBeNull();
        result.Worker.Id.ShouldBe(worker.Id);
        result.Worker.UserId.ShouldBe("worker1");
        result.Worker.FirstName.ShouldBe("Bob");
        result.Worker.LastName.ShouldBe("Williams");
        result.Worker.Username.ShouldBe("bobwilliams");
        result.Worker.JobTitle.ShouldBe("Full Stack Developer");
        result.Worker.ProfilePictureUrl.ShouldBe("https://example.com/bob.jpg");
    }
    [Fact]
    public async Task SelectJobOfferResponseFromEntity_ShouldProjectMultipleOffers()
    {
        // Arrange
        var author1 = TestDataBuilder.CreateProfile(userId: "author1", username: "author1");
        var author2 = TestDataBuilder.CreateProfile(userId: "author2", username: "author2");
        var worker = TestDataBuilder.CreateProfile(userId: "worker1", username: "worker1");

        var offer1 = TestDataBuilder.CreateJobOffer(
            authorId: author1.Id,
            workerId: worker.Id,
            title: "Project A",
            description: "Description A",
            budget: 1000m,
            area: "Area A"
        );

        var offer2 = TestDataBuilder.CreateJobOffer(
            authorId: author2.Id,
            workerId: worker.Id,
            title: "Project B",
            description: "Description B",
            budget: 2000m,
            area: "Area B"
        );

        _context.Profiles.AddRange(author1, author2, worker);
        _context.JobOffers.AddRange(offer1, offer2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.JobOffers
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .Where(j => j.WorkerId == worker.Id)
            .SelectJobOfferResponseFromEntity()
            .ToListAsync();

        // Assert
        results.ShouldNotBeNull();
        results.Count.ShouldBe(2);

        var firstOffer = results.Single(r => r.Title == "Project A");
        firstOffer.Budget.ShouldBe(1000m);
        firstOffer.Area.ShouldBe("Area A");
        firstOffer.Author.ShouldNotBeNull();
        firstOffer.Author.Username.ShouldBe("author1");

        var secondOffer = results.Single(r => r.Title == "Project B");
        secondOffer.Budget.ShouldBe(2000m);
        secondOffer.Area.ShouldBe("Area B");
        secondOffer.Author.ShouldNotBeNull();
        secondOffer.Author.Username.ShouldBe("author2");
    }

    [Fact]
    public async Task SelectJobOfferResponseFromEntity_ShouldFilterByAuthor()
    {
        // Arrange
        var author = TestDataBuilder.CreateProfile(userId: "author1", username: "author");
        var worker1 = TestDataBuilder.CreateProfile(userId: "worker1", username: "worker1");
        var worker2 = TestDataBuilder.CreateProfile(userId: "worker2", username: "worker2");

        var offer1 = TestDataBuilder.CreateJobOffer(
            authorId: author.Id,
            workerId: worker1.Id,
            title: "Offer 1",
            description: "First offer",
            budget: 1000m,
            area: "Area 1"
        );

        var offer2 = TestDataBuilder.CreateJobOffer(
            authorId: author.Id,
            workerId: worker2.Id,
            title: "Offer 2",
            description: "Second offer",
            budget: 2000m,
            area: "Area 2"
        );

        _context.Profiles.AddRange(author, worker1, worker2);
        _context.JobOffers.AddRange(offer1, offer2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.JobOffers
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .Where(j => j.AuthorId == author.Id)
            .SelectJobOfferResponseFromEntity()
            .ToListAsync();

        // Assert
        results.ShouldNotBeNull();
        results.Count.ShouldBe(2);
        results.All(r => r.Author?.Username == "author").ShouldBeTrue();
    }

    [Fact]
    public async Task SelectJobOfferResponseFromEntity_ShouldHandleNullProfilePicture()
    {
        // Arrange
        var author = TestDataBuilder.CreateProfile(
            userId: "author1",
            username: "author",
            profilePictureUrl: null
        );

        var worker = TestDataBuilder.CreateProfile(
            userId: "worker1",
            username: "worker"
        );

        var offer = TestDataBuilder.CreateJobOffer(
            authorId: author.Id,
            workerId: worker.Id,
            title: "Test Job",
            description: "Test Description",
            budget: 1000m,
            area: "Test Area"
        );

        _context.Profiles.AddRange(author, worker);
        _context.JobOffers.Add(offer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.JobOffers
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .Where(j => j.Id == offer.Id)
            .SelectJobOfferResponseFromEntity()
            .SingleAsync();

        // Assert
        result.Author.ShouldNotBeNull();
        result.Author.ProfilePictureUrl.ShouldBeNull();
    }

    [Fact]
    public async Task SelectJobOfferResponseFromEntity_ShouldReturnEmptyForNoOffers()
    {
        // Act
        var results = await _context.JobOffers
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .SelectJobOfferResponseFromEntity()
            .ToListAsync();

        // Assert
        results.ShouldNotBeNull();
        results.ShouldBeEmpty();
    }

    [Fact]
    public async Task SelectJobOfferResponseFromEntity_ShouldHandleLargeBudget()
    {
        // Arrange
        var author = TestDataBuilder.CreateProfile(userId: "author1", username: "author");
        var worker = TestDataBuilder.CreateProfile(userId: "worker1", username: "worker");

        var offer = TestDataBuilder.CreateJobOffer(
            authorId: author.Id,
            workerId: worker.Id,
            title: "Enterprise Project",
            description: "Large scale implementation",
            budget: 999999.99m,
            area: "Global"
        );

        _context.Profiles.AddRange(author, worker);
        _context.JobOffers.Add(offer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.JobOffers
            .Include(j => j.Author)
            .Include(j => j.Worker)
            .Where(j => j.Id == offer.Id)
            .SelectJobOfferResponseFromEntity()
            .SingleAsync();

        // Assert
        result.Budget.ShouldBe(999999.99m);
    }
}
