// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Queries;

/// <summary>
///     Unit tests for MessageQueries extension methods.
/// </summary>
/// <remarks>
///     Tests cover SelectMessageResponseFromFullMessage projection method for SignalR messaging.
///     Uses in-memory EF Core database for realistic query execution.
/// </remarks>
public sealed class MessageQueriesTests : IDisposable
{
    private readonly ExpertBridgeDbContext _context;

    public MessageQueriesTests()
    {
        _context = InMemoryDbContextFixture.Create();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #region SelectMessageResponseFromFullMessage Tests

    [Fact]
    public async Task SelectMessageResponseFromFullMessage_Should_Project_AllProperties()
    {
        // Arrange
        var message = TestDataBuilder.CreateMessage(
            "sender1",
            "chat1",
            "Hello, world!",
            id: "msg1");
        _context.Set<Message>().Add(message);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Set<Message>()
            .SelectMessageResponseFromFullMessage()
            .FirstAsync();

        // Assert
        result.ShouldNotBeNull();
        result.SenderId.ShouldBe("sender1");
        result.ChatId.ShouldBe("chat1");
        result.Content.ShouldBe("Hello, world!");
        result.CreatedAt.ShouldNotBe(default);
    }

    [Fact]
    public async Task SelectMessageResponseFromFullMessage_Should_Project_ConfirmationMessage()
    {
        // Arrange
        var confirmationMsg = TestDataBuilder.CreateMessage(
            "system",
            "chat1",
            "Job has been accepted",
            true);
        _context.Set<Message>().Add(confirmationMsg);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Set<Message>()
            .SelectMessageResponseFromFullMessage()
            .FirstAsync();

        // Assert
        result.IsConfirmationMessage.ShouldBeTrue();
        result.Content.ShouldBe("Job has been accepted");
    }

    [Fact]
    public async Task SelectMessageResponseFromFullMessage_Should_Project_RegularMessage()
    {
        // Arrange
        var regularMsg = TestDataBuilder.CreateMessage(
            "user1",
            "chat1",
            "Regular message",
            false);
        _context.Set<Message>().Add(regularMsg);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Set<Message>()
            .SelectMessageResponseFromFullMessage()
            .FirstAsync();

        // Assert
        result.IsConfirmationMessage.ShouldBeFalse();
    }

    [Fact]
    public async Task SelectMessageResponseFromFullMessage_Should_Project_MultipleMessages()
    {
        // Arrange
        var msg1 = TestDataBuilder.CreateMessage("sender1", "chat1", "Message 1", false, "m1");
        var msg2 = TestDataBuilder.CreateMessage("sender2", "chat1", "Message 2", false, "m2");
        var msg3 = TestDataBuilder.CreateMessage("sender1", "chat1", "Confirmation", true, "m3");

        _context.Set<Message>().AddRange(msg1, msg2, msg3);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.Set<Message>()
            .SelectMessageResponseFromFullMessage()
            .ToListAsync();

        // Assert
        results.Count.ShouldBe(3);
        results.Count(r => r.IsConfirmationMessage).ShouldBe(1);
        results.Count(r => !r.IsConfirmationMessage).ShouldBe(2);
    }

    [Fact]
    public async Task SelectMessageResponseFromFullMessage_Should_Return_Empty_When_NoMessages()
    {
        // Arrange - no data added

        // Act
        var results = await _context.Set<Message>()
            .SelectMessageResponseFromFullMessage()
            .ToListAsync();

        // Assert
        results.ShouldBeEmpty();
    }

    [Fact]
    public async Task SelectMessageResponseFromFullMessage_Should_Include_CreatedAt()
    {
        // Arrange
        var message = TestDataBuilder.CreateMessage(
            "sender1",
            "chat1",
            "Test message");
        _context.Set<Message>().Add(message);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Set<Message>()
            .SelectMessageResponseFromFullMessage()
            .FirstAsync();

        // Assert
        result.CreatedAt.ShouldNotBe(default);
        result.CreatedAt.ShouldBeInRange(
            DateTime.UtcNow.AddMinutes(-1),
            DateTime.UtcNow.AddMinutes(1));
    }

    [Fact]
    public async Task SelectMessageResponseFromFullMessage_Should_Filter_ByChatId()
    {
        // Arrange
        var msg1 = TestDataBuilder.CreateMessage("sender1", "chat1", "Message for chat1");
        var msg2 = TestDataBuilder.CreateMessage("sender2", "chat2", "Message for chat2");
        var msg3 = TestDataBuilder.CreateMessage("sender1", "chat1", "Another for chat1");

        _context.Set<Message>().AddRange(msg1, msg2, msg3);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.Set<Message>()
            .Where(m => m.ChatId == "chat1")
            .SelectMessageResponseFromFullMessage()
            .ToListAsync();

        // Assert
        results.Count.ShouldBe(2);
        results.ShouldAllBe(r => r.ChatId == "chat1");
    }

    #endregion
}
