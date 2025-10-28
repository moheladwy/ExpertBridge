// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Queries;

/// <summary>
/// Unit tests for NotificationQueries extension methods.
/// </summary>
/// <remarks>
/// Tests cover SelectNotificationResopnse projection method for real-time notifications via SignalR.
/// Uses in-memory EF Core database for realistic query execution.
/// </remarks>
public sealed class NotificationQueriesTests : IDisposable
{
  private readonly ExpertBridgeDbContext _context;

  public NotificationQueriesTests()
  {
    _context = InMemoryDbContextFixture.Create();
  }

  #region SelectNotificationResopnse Tests

  [Fact]
  public async Task SelectNotificationResopnse_Should_Project_AllBasicProperties()
  {
    // Arrange
    var notification = TestDataBuilder.CreateNotification(
        recipientId: "user1",
        message: "You have a new comment",
        id: "notif1");
    _context.Set<Notification>().Add(notification);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Set<Notification>()
        .SelectNotificationResopnse()
        .FirstAsync();

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe("notif1");
    result.RecipientId.ShouldBe("user1");
    result.Message.ShouldBe("You have a new comment");
    result.IsRead.ShouldBeFalse();
  }

  [Fact]
  public async Task SelectNotificationResopnse_Should_Project_ReadNotification()
  {
    // Arrange
    var notification = TestDataBuilder.CreateNotification(
        recipientId: "user1",
        message: "Read notification",
        isRead: true);
    _context.Set<Notification>().Add(notification);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Set<Notification>()
        .SelectNotificationResopnse()
        .FirstAsync();

    // Assert
    result.IsRead.ShouldBeTrue();
  }

  [Fact]
  public async Task SelectNotificationResopnse_Should_Project_ActionUrl()
  {
    // Arrange
    var notification = TestDataBuilder.CreateNotification(
        recipientId: "user1",
        actionUrl: "/posts/123");
    _context.Set<Notification>().Add(notification);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Set<Notification>()
        .SelectNotificationResopnse()
        .FirstAsync();

    // Assert
    result.ActionUrl.ShouldBe("/posts/123");
  }

  [Fact]
  public async Task SelectNotificationResopnse_Should_Project_IconUrls()
  {
    // Arrange
    var notification = TestDataBuilder.CreateNotification(
        recipientId: "user1",
        iconUrl: "/avatars/user123.jpg",
        iconActionUrl: "/profiles/user123");
    _context.Set<Notification>().Add(notification);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Set<Notification>()
        .SelectNotificationResopnse()
        .FirstAsync();

    // Assert
    result.IconUrl.ShouldBe("/avatars/user123.jpg");
    result.IconActionUrl.ShouldBe("/profiles/user123");
  }

  [Fact]
  public async Task SelectNotificationResopnse_Should_Handle_NullOptionalFields()
  {
    // Arrange
    var notification = TestDataBuilder.CreateNotification(
        recipientId: "user1",
        message: "Simple notification");
    _context.Set<Notification>().Add(notification);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Set<Notification>()
        .SelectNotificationResopnse()
        .FirstAsync();

    // Assert
    result.ActionUrl.ShouldBeNull();
    result.IconUrl.ShouldBeNull();
    result.IconActionUrl.ShouldBeNull();
  }

  [Fact]
  public async Task SelectNotificationResopnse_Should_UseUtcNow_When_CreatedAtIsNull()
  {
    // Arrange
    var notification = new Notification
    {
      Id = "notif1",
      RecipientId = "user1",
      Message = "Test notification",
      CreatedAt = null // Explicitly null
    };
    _context.Set<Notification>().Add(notification);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Set<Notification>()
        .SelectNotificationResopnse()
        .FirstAsync();

    // Assert
    // Should use DateTime.UtcNow as fallback (within reasonable time range)
    result.CreatedAt.ShouldBeInRange(
        DateTime.UtcNow.AddMinutes(-1),
        DateTime.UtcNow.AddMinutes(1));
  }

  [Fact]
  public async Task SelectNotificationResopnse_Should_Project_MultipleNotifications()
  {
    // Arrange
    var notif1 = TestDataBuilder.CreateNotification("user1", "Notification 1", false, id: "n1");
    var notif2 = TestDataBuilder.CreateNotification("user1", "Notification 2", true, id: "n2");
    var notif3 = TestDataBuilder.CreateNotification("user2", "Notification 3", false, id: "n3");

    _context.Set<Notification>().AddRange(notif1, notif2, notif3);
    await _context.SaveChangesAsync();

    // Act
    var results = await _context.Set<Notification>()
        .SelectNotificationResopnse()
        .ToListAsync();

    // Assert
    results.Count.ShouldBe(3);
    results.Count(n => n.IsRead).ShouldBe(1);
    results.Count(n => !n.IsRead).ShouldBe(2);
  }

  [Fact]
  public async Task SelectNotificationResopnse_Should_Return_Empty_When_NoNotifications()
  {
    // Arrange - no data added

    // Act
    var results = await _context.Set<Notification>()
        .SelectNotificationResopnse()
        .ToListAsync();

    // Assert
    results.ShouldBeEmpty();
  }

  [Fact]
  public async Task SelectNotificationResopnse_Should_Filter_ByRecipient()
  {
    // Arrange
    var notif1 = TestDataBuilder.CreateNotification("user1", "For user1");
    var notif2 = TestDataBuilder.CreateNotification("user2", "For user2");
    var notif3 = TestDataBuilder.CreateNotification("user1", "Also for user1");

    _context.Set<Notification>().AddRange(notif1, notif2, notif3);
    await _context.SaveChangesAsync();

    // Act
    var results = await _context.Set<Notification>()
        .Where(n => n.RecipientId == "user1")
        .SelectNotificationResopnse()
        .ToListAsync();

    // Assert
    results.Count.ShouldBe(2);
    results.ShouldAllBe(n => n.RecipientId == "user1");
  }

  [Fact]
  public async Task SelectNotificationResopnse_Should_Filter_UnreadNotifications()
  {
    // Arrange
    var notif1 = TestDataBuilder.CreateNotification("user1", "Unread 1", isRead: false);
    var notif2 = TestDataBuilder.CreateNotification("user1", "Read", isRead: true);
    var notif3 = TestDataBuilder.CreateNotification("user1", "Unread 2", isRead: false);

    _context.Set<Notification>().AddRange(notif1, notif2, notif3);
    await _context.SaveChangesAsync();

    // Act
    var results = await _context.Set<Notification>()
        .Where(n => !n.IsRead)
        .SelectNotificationResopnse()
        .ToListAsync();

    // Assert
    results.Count.ShouldBe(2);
    results.ShouldAllBe(n => !n.IsRead);
  }

  #endregion

  public void Dispose()
  {
    _context.Dispose();
  }
}
