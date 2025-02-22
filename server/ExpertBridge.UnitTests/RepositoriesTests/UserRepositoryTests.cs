using ExpertBridge.Application.Repositories.User;
using ExpertBridge.Core;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.User;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.UnitTests.RepositoriesTests;

public sealed class UserRepositoryTests
{
    private static DbContextOptions<ExpertBridgeDbContext> CreateNewContextOptions(SqliteConnection connection)
    {
        // Each test gets its own fresh copy of the database.
        return new DbContextOptionsBuilder<ExpertBridgeDbContext>()
            .UseSqlite(connection)
            .Options;
    }
    private readonly SqliteConnection _connection;
    private UserRepository _repository;
    private readonly User _validUser, _anotherValidUser;

    public UserRepositoryTests()
    {
        _validUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            ProviderId = Guid.NewGuid().ToString(),
            Email = "user1@example.com",
            Username = "user1",
            FirstName = "Test",
            LastName = "User",
            IsBanned = false,
            IsDeleted = false,
            CreatedAt = DateTime.Now
        };
        _anotherValidUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            ProviderId = Guid.NewGuid().ToString(),
            Email = "user2@example.com",
            Username = "user2",
            FirstName = "Test",
            LastName = "User",
            IsBanned = false,
            IsDeleted = false,
            CreatedAt = DateTime.Now
        };
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
    }

    [Fact]
    public async Task GetAllUsers_WhenUsersExist_ReturnUsers()
    {
        // Arrange: Create a new context with seeded data.
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _repository = new UserRepository(context);
        context.Users.Add(_validUser);
        context.Users.Add(_anotherValidUser);
        await context.SaveChangesAsync();

        // Act: Use the repository to get all users.
        var users = await _repository.GetAllAsync();

        // Assert: The users should be returned by the repository.
        Assert.NotNull(users);
        Assert.NotEmpty(users);
    }

    [Fact]
    public async Task GetAllUsers_WhenNoUsersExist_ReturnEmptyList()
    {
        // Arrange: Create a new context with no seeded data.
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _repository = new UserRepository(context);

        // Act: Use the repository to get all users.
        var users = await _repository.GetAllAsync();

        // Assert: The users should be returned by the repository.
        Assert.NotNull(users);
        Assert.Empty(users);
    }

    [Fact]
    public async Task GetUserById_WhenUserExists_ReturnUser()
    {
        // Arrange: Create a new context with seeded data.
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        context.Users.Add(_validUser);
        await context.SaveChangesAsync();
        _repository = new UserRepository(context);

        // Act: Use the repository to get the user by id.
        var userByIdAsNoTracking = await _repository.GetByIdAsNoTrackingAsync(_validUser.Id);
        var userById = await _repository.GetByIdAsync(_validUser.Id);

        // Assert: The user should be returned by the repository.
        Assert.NotNull(userByIdAsNoTracking);
        Assert.NotNull(userById);
    }

    [Fact]
    public async Task GetUserById_WhenUserDoesNotExist_ReturnNull()
    {
        // Arrange: Create a new context with seeded data.
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _repository = new UserRepository(context);
        var invalidUserId = Guid.NewGuid().ToString();

        // Act: Use the repository to get the user by id.
        var userByIdAsNoTracking = await _repository.GetByIdAsNoTrackingAsync(invalidUserId);
        var userById = await _repository.GetByIdAsync(invalidUserId);

        // Assert: The user should not be returned by the repository.
        Assert.Null(userByIdAsNoTracking);
        Assert.Null(userById);
    }

    [Fact]
    public async Task GetUserByPredicate_WhenUserExists_ReturnUser()
    {
        // Arrange: Create a new context with seeded data.
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _repository = new UserRepository(context);
        context.Users.Add(_validUser);
        await context.SaveChangesAsync();

        // Act: Use the repository to get the user by username.
        var userByUsernameAsNoTracking = await _repository.GetFirstAsNoTrackingAsync(x => x.Username == _validUser.Username);
        var userByUsername = await _repository.GetFirstAsync(x => x.Username == _validUser.Username);

        // Assert: The user should be returned by the repository.
        Assert.NotNull(userByUsernameAsNoTracking);
        Assert.NotNull(userByUsername);
    }

    [Fact]
    public async Task GetUserByPredicate_WhenUserDoesNotExist_ReturnNull()
    {
        // Arrange: Create a new context with seeded data.
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _repository = new UserRepository(context);

        // Act: Use the repository to get the user by username.
        var userByUsernameAsNoTracking = await _repository.GetFirstAsNoTrackingAsync(x => x.Username == _validUser.Username);
        var userByUsername = await _repository.GetFirstAsync(x => x.Username == _validUser.Username);

        // Assert: The user should not be returned by the repository.
        Assert.Null(userByUsernameAsNoTracking);
        Assert.Null(userByUsername);
    }

    [Fact]
    public async Task AddUser_WhenUserIsValidAndDoesNotExist_ShouldBeAdded()
    {
        // Arrange: Create a new context with no seeded data.
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _repository = new UserRepository(context);

        // Act: Use the repository to add a user.
        await _repository.AddAsync(_validUser);

        // Assert: The user should be added to the repository.
        var user = await context.Users.FindAsync(_validUser.Id);
        Assert.NotNull(user);
        Assert.Equal(_validUser.Id, user.Id);
    }

    [Fact]
    public async Task AddUser_WhenUserIsValidAndAlreadyExists_ShouldThrowException()
    {
        // Arrange: Create a new context with seeded data.
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        context.Users.Add(_validUser);
        await context.SaveChangesAsync();
        _repository = new UserRepository(context);

        // Act: Use the repository to add a user.
        // Assert: It throws an exception when trying to add an existing user.
        await Assert.ThrowsAsync<DbUpdateException>(async () => await _repository.AddAsync(_validUser));
    }

    [Fact]
    public async Task AddUser_WhenUserIsInvalid_ShouldThrowException()
    {
        // Arrange: Create a new context with no seeded data.
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _repository = new UserRepository(context);
        var invalidUser = new User();

        // Act: Use the repository to add a user.
        // Assert: It throws an exception when trying to add an invalid user.
        await Assert.ThrowsAsync<DbUpdateException>(async () => await _repository.AddAsync(invalidUser));
    }

    [Fact]
    public async Task UpdateUser_WhenUserIsValidAndExists_ShouldBeUpdated()
    {
        // Arrange: Create a new context with seeded data.
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        context.Users.Add(_validUser);
        await context.SaveChangesAsync();
        _repository = new UserRepository(context);

        // Act: Use the repository to update a user.
        _validUser.FirstName = "Updated";
        _validUser.LastName = "User";
        await _repository.UpdateAsync(_validUser);

        // Assert: The user should be updated in the repository.
        var user = await context.Users.FindAsync(_validUser.Id);
        Assert.NotNull(user);
        Assert.Equal(_validUser.FirstName, user.FirstName);
        Assert.Equal(_validUser.LastName, user.LastName);
    }

    [Fact]
    public async Task UpdateUser_WhenUserUpdatesIsInvalid_ShouldThrowException()
    {
        // Arrange: Create a new context with seeded data.
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        context.Users.Add(_validUser);
        await context.SaveChangesAsync();
        _repository = new UserRepository(context);

        // Act: Use the repository to update a user.
        _validUser.FirstName = null;
        _validUser.ProviderId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Assert: It throws an exception when trying to update an invalid user.
        await Assert.ThrowsAsync<DbUpdateException>(async () => await _repository.UpdateAsync(_validUser));
    }

    [Fact]
    public async Task UpdateUser_WhenUserDoesNotExist_ShouldThrowException()
    {
        // Arrange: Create a new context with no seeded data.
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _repository = new UserRepository(context);

        // Act: Use the repository to update a user.
        // Assert: It throws an exception when trying to update a non-existing user.
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await _repository.UpdateAsync(_validUser));
    }

    [Fact]
    public async Task DeleteUser_WhenUserExists_ShouldBeDeleted()
    {
        // Arrange: Create a new context with seeded data.
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        context.Users.Add(_validUser);
        await context.SaveChangesAsync();
        _repository = new UserRepository(context);

        // Act: Use the repository to delete a user.
        await _repository.DeleteAsync(_validUser.Id);

        // Assert: The user should be deleted from the repository.
        var user = await context.Users.FindAsync(_validUser.Id);
        Assert.True(user?.IsDeleted);
    }

    [Fact]
    public async Task DeleteUser_WhenUserDoesNotExist_ShouldThrowException()
    {
        // Arrange: Create a new context with no seeded data.
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _repository = new UserRepository(context);

        // Act: Use the repository to delete a user.
        // Assert: It throws an exception when trying to delete a non-existing user.
        await Assert.ThrowsAsync<UserNotFoundException>(async () => await _repository.DeleteAsync(_validUser.Id));
    }
}
