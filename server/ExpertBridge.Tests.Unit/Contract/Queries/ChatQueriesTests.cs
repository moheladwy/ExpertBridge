// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Queries;

/// <summary>
///     Unit tests for ChatQueries extension methods.
/// </summary>
/// <remarks>
///     Tests cover WhereProfileIsChatParticipant filtering for security and access control.
///     Uses in-memory EF Core database for realistic query execution.
/// </remarks>
public sealed class ChatQueriesTests : IDisposable
{
    private readonly ExpertBridgeDbContext _context;

    public ChatQueriesTests()
    {
        _context = InMemoryDbContextFixture.Create();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #region WhereProfileIsChatParticipant Tests

    [Fact]
    public async Task WhereProfileIsChatParticipant_Should_Return_ChatsWhereProfileIsHirer()
    {
        // Arrange
        var profileId = "profile1";
        var chat1 = TestDataBuilder.CreateChat(profileId, "worker1", "chat1");
        var chat2 = TestDataBuilder.CreateChat("hirer2", "worker2", "chat2");

        _context.Set<Chat>().AddRange(chat1, chat2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.Set<Chat>()
            .WhereProfileIsChatParticipant(profileId)
            .ToListAsync();

        // Assert
        results.ShouldHaveSingleItem();
        results.First().Id.ShouldBe("chat1");
        results.First().HirerId.ShouldBe(profileId);
    }

    [Fact]
    public async Task WhereProfileIsChatParticipant_Should_Return_ChatsWhereProfileIsWorker()
    {
        // Arrange
        var profileId = "profile1";
        var chat1 = TestDataBuilder.CreateChat("hirer1", profileId, "chat1");
        var chat2 = TestDataBuilder.CreateChat("hirer2", "worker2", "chat2");

        _context.Set<Chat>().AddRange(chat1, chat2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.Set<Chat>()
            .WhereProfileIsChatParticipant(profileId)
            .ToListAsync();

        // Assert
        results.ShouldHaveSingleItem();
        results.First().Id.ShouldBe("chat1");
        results.First().WorkerId.ShouldBe(profileId);
    }

    [Fact]
    public async Task WhereProfileIsChatParticipant_Should_Return_MultipleChatsForActiveUser()
    {
        // Arrange
        var profileId = "activeUser";
        var chat1 = TestDataBuilder.CreateChat(profileId, "worker1", "chat1");
        var chat2 = TestDataBuilder.CreateChat("hirer2", profileId, "chat2");
        var chat3 = TestDataBuilder.CreateChat(profileId, "worker3", "chat3");
        var chat4 = TestDataBuilder.CreateChat("hirer4", "worker4", "chat4");

        _context.Set<Chat>().AddRange(chat1, chat2, chat3, chat4);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.Set<Chat>()
            .WhereProfileIsChatParticipant(profileId)
            .ToListAsync();

        // Assert
        results.Count.ShouldBe(3);
        results.ShouldAllBe(c => c.HirerId == profileId || c.WorkerId == profileId);
    }

    [Fact]
    public async Task WhereProfileIsChatParticipant_Should_Return_Empty_WhenProfileNotParticipant()
    {
        // Arrange
        var chat1 = TestDataBuilder.CreateChat("hirer1", "worker1");
        var chat2 = TestDataBuilder.CreateChat("hirer2", "worker2");

        _context.Set<Chat>().AddRange(chat1, chat2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.Set<Chat>()
            .WhereProfileIsChatParticipant("nonParticipant")
            .ToListAsync();

        // Assert
        results.ShouldBeEmpty();
    }

    [Fact]
    public async Task WhereProfileIsChatParticipant_Should_ExcludeNonMatchingChats()
    {
        // Arrange
        var profileId = "profile1";
        var chat1 = TestDataBuilder.CreateChat(profileId, "worker1", "chat1");
        var chat2 = TestDataBuilder.CreateChat("hirer2", "worker2", "chat2");
        var chat3 = TestDataBuilder.CreateChat("hirer3", "worker3", "chat3");

        _context.Set<Chat>().AddRange(chat1, chat2, chat3);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.Set<Chat>()
            .WhereProfileIsChatParticipant(profileId)
            .ToListAsync();

        // Assert
        results.ShouldHaveSingleItem();
        results.ShouldNotContain(c => c.Id == "chat2");
        results.ShouldNotContain(c => c.Id == "chat3");
    }

    [Fact]
    public async Task WhereProfileIsChatParticipant_Should_WorkWithOtherFilters()
    {
        // Arrange
        var profileId = "profile1";
        var chat1 = TestDataBuilder.CreateChat(profileId, "worker1", "chat1");
        var chat2 = TestDataBuilder.CreateChat("hirer2", profileId, "chat2");
        var chat3 = TestDataBuilder.CreateChat(profileId, "worker3", "chat3");

        // Mark chat3 as deleted
        chat3.IsDeleted = true;

        _context.Set<Chat>().AddRange(chat1, chat2, chat3);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.Set<Chat>()
            .WhereProfileIsChatParticipant(profileId)
            .Where(c => !c.IsDeleted)
            .ToListAsync();

        // Assert
        results.Count.ShouldBe(2);
        results.ShouldAllBe(c => !c.IsDeleted);
        results.ShouldNotContain(c => c.Id == "chat3");
    }

    [Fact]
    public async Task WhereProfileIsChatParticipant_Should_Return_Empty_WhenNoChats()
    {
        // Arrange - no data added

        // Act
        var results = await _context.Set<Chat>()
            .WhereProfileIsChatParticipant("anyProfile")
            .ToListAsync();

        // Assert
        results.ShouldBeEmpty();
    }

    [Fact]
    public async Task WhereProfileIsChatParticipant_Should_HandleBothRolesForSameProfile()
    {
        // Arrange - Edge case: same profile as both hirer and worker in different chats
        var profileId = "multirole";
        var chat1 = TestDataBuilder.CreateChat(profileId, "other1", "chat1");
        var chat2 = TestDataBuilder.CreateChat("other2", profileId, "chat2");

        _context.Set<Chat>().AddRange(chat1, chat2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.Set<Chat>()
            .WhereProfileIsChatParticipant(profileId)
            .ToListAsync();

        // Assert
        results.Count.ShouldBe(2);
        results.ShouldContain(c => c.Id == "chat1" && c.HirerId == profileId);
        results.ShouldContain(c => c.Id == "chat2" && c.WorkerId == profileId);
    }

    #endregion
}
