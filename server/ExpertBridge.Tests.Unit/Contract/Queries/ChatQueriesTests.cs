// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Queries;
using ExpertBridge.Core.Entities.Chats;

namespace ExpertBridge.Tests.Unit.Contract.Queries;

/// <summary>
/// Unit tests for ChatQueries extension methods.
/// </summary>
/// <remarks>
/// Tests cover WhereProfileIsChatParticipant filtering for security and access control.
/// Uses in-memory EF Core database for realistic query execution.
/// </remarks>
public sealed class ChatQueriesTests : IDisposable
{
  private readonly ExpertBridgeDbContext _context;

  public ChatQueriesTests()
  {
    _context = InMemoryDbContextFixture.Create();
  }

  #region WhereProfileIsChatParticipant Tests

  [Fact]
  public async Task WhereProfileIsChatParticipant_Should_Return_ChatsWhereProfileIsHirer()
  {
    // Arrange
    var profileId = "profile1";
    var chat1 = TestDataBuilder.CreateChat(hirerId: profileId, workerId: "worker1", id: "chat1");
    var chat2 = TestDataBuilder.CreateChat(hirerId: "hirer2", workerId: "worker2", id: "chat2");

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
    var chat1 = TestDataBuilder.CreateChat(hirerId: "hirer1", workerId: profileId, id: "chat1");
    var chat2 = TestDataBuilder.CreateChat(hirerId: "hirer2", workerId: "worker2", id: "chat2");

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
    var chat1 = TestDataBuilder.CreateChat(hirerId: profileId, workerId: "worker1", id: "chat1");
    var chat2 = TestDataBuilder.CreateChat(hirerId: "hirer2", workerId: profileId, id: "chat2");
    var chat3 = TestDataBuilder.CreateChat(hirerId: profileId, workerId: "worker3", id: "chat3");
    var chat4 = TestDataBuilder.CreateChat(hirerId: "hirer4", workerId: "worker4", id: "chat4");

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
    var chat1 = TestDataBuilder.CreateChat(hirerId: "hirer1", workerId: "worker1");
    var chat2 = TestDataBuilder.CreateChat(hirerId: "hirer2", workerId: "worker2");

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
    var chat1 = TestDataBuilder.CreateChat(hirerId: profileId, workerId: "worker1", id: "chat1");
    var chat2 = TestDataBuilder.CreateChat(hirerId: "hirer2", workerId: "worker2", id: "chat2");
    var chat3 = TestDataBuilder.CreateChat(hirerId: "hirer3", workerId: "worker3", id: "chat3");

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
    var chat1 = TestDataBuilder.CreateChat(hirerId: profileId, workerId: "worker1", id: "chat1");
    var chat2 = TestDataBuilder.CreateChat(hirerId: "hirer2", workerId: profileId, id: "chat2");
    var chat3 = TestDataBuilder.CreateChat(hirerId: profileId, workerId: "worker3", id: "chat3");

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
    var chat1 = TestDataBuilder.CreateChat(hirerId: profileId, workerId: "other1", id: "chat1");
    var chat2 = TestDataBuilder.CreateChat(hirerId: "other2", workerId: profileId, id: "chat2");

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

  public void Dispose()
  {
    _context.Dispose();
  }
}
