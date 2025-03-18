// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Application.Repositories.Profiles;
using ExpertBridge.Api.Application.Repositories.Users;
using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.Entities;
using ExpertBridge.Api.Core.Entities.Profiles;
using ExpertBridge.Api.Core.Entities.Users;
using ExpertBridge.Api.Data.DatabaseContexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.UnitTests.RepositoriesTests;

public class ProfileRepositoryTests
{
    private static DbContextOptions<ExpertBridgeDbContext> CreateNewContextOptions(SqliteConnection connection)
    {
        // Each test gets its own fresh copy of the database.
        return new DbContextOptionsBuilder<ExpertBridgeDbContext>()
            .UseSqlite(connection)
            .Options;
    }
    private readonly SqliteConnection _connection;
    private ProfilesRepository _profileRepository;
    private UsersRepository _userRepository;
    private readonly User _validUser;
    private readonly Profile _validProfile;

    public ProfileRepositoryTests()
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
            CreatedAt = DateTime.UtcNow
        };
        _validProfile = new Profile
        {
            Id = Guid.NewGuid().ToString(),
            UserId = _validUser.Id,
            JobTitle = "Software Developer",
            Bio = "I am a software developer",
            ProfilePictureUrl = "https://example.com/profile.jpg",
            Rating = 5,
            RatingCount = 1
        };
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
    }

    [Fact]
    private async Task GetByIdAsync_WhenProfileExists_ShouldReturnProfile()
    {
        // Arrange
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        await context.Users.AddAsync(_validUser);
        await context.Profiles.AddAsync(_validProfile);
        await context.SaveChangesAsync();
        _userRepository = new UsersRepository(context);
        _profileRepository = new ProfilesRepository(context, _userRepository);

        // Act
        var resultWithTracking = await _profileRepository.GetByIdAsync(_validProfile.Id);
        var resultWithoutTracking = await _profileRepository.GetByIdAsNoTrackingAsync(_validProfile.Id);

        // Assert
        Assert.NotNull(resultWithTracking);
        Assert.NotNull(resultWithoutTracking);
        Assert.Equal(_validProfile, resultWithTracking);
        Assert.Equal(_validProfile, resultWithoutTracking);
    }

    [Fact]
    private async Task GetByIdAsync_WhenProfileDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _userRepository = new UsersRepository(context);
        _profileRepository = new ProfilesRepository(context, _userRepository);

        // Act
        var resultWithTracking = await _profileRepository.GetByIdAsync(_validProfile.Id);
        var resultWithoutTracking = await _profileRepository.GetByIdAsNoTrackingAsync(_validProfile.Id);

        // Assert
        Assert.Null(resultWithTracking);
        Assert.Null(resultWithoutTracking);
    }

    [Fact]
    private async Task GetAllAsync_WhenProfilesExist_ShouldReturnProfiles()
    {
        // Arrange
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        await context.Users.AddAsync(_validUser);
        await context.Profiles.AddAsync(_validProfile);
        await context.SaveChangesAsync();
        _userRepository = new UsersRepository(context);
        _profileRepository = new ProfilesRepository(context, _userRepository);

        // Act
        var result = await _profileRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    private async Task GetAllAsync_WhenProfilesDoNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _userRepository = new UsersRepository(context);
        _profileRepository = new ProfilesRepository(context, _userRepository);

        // Act
        var result = await _profileRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    private async Task GetProfileByPredicateAsync_WhenProfileExists_ShouldReturnProfile()
    {
        // Arrange
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        await context.Users.AddAsync(_validUser);
        await context.Profiles.AddAsync(_validProfile);
        await context.SaveChangesAsync();
        _userRepository = new UsersRepository(context);
        _profileRepository = new ProfilesRepository(context, _userRepository);

        // Act
        var resultWithTracking = await _profileRepository.GetFirstAsync(p => p.Id == _validProfile.Id);
        var resultWithoutTracking = await _profileRepository.GetFirstAsNoTrackingAsync(p => p.Id == _validProfile.Id);

        // Assert
        Assert.NotNull(resultWithTracking);
        Assert.NotNull(resultWithoutTracking);
        Assert.Equal(_validProfile.Id, resultWithTracking.Id);
        Assert.Equal(_validProfile.Id, resultWithoutTracking.Id);
        Assert.Equal(resultWithTracking.Id, resultWithoutTracking.Id);
    }

    [Fact]
    private async Task GetProfileByPredicateAsync_WhenProfileDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _userRepository = new UsersRepository(context);
        _profileRepository = new ProfilesRepository(context, _userRepository);

        // Act
        var resultWithTracking = await _profileRepository.GetFirstAsync(p => p.Id == _validProfile.Id);
        var resultWithoutTracking = await _profileRepository.GetFirstAsNoTrackingAsync(p => p.Id == _validProfile.Id);

        // Assert
        Assert.Null(resultWithTracking);
        Assert.Null(resultWithoutTracking);
    }

    [Fact]
    private async Task AddAsync_WhenProfileIsValidAndTheUserExistsAndTheProfileDoesNotExist_ShouldAddProfile()
    {
        // Arrange
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _userRepository = new UsersRepository(context);
        _profileRepository = new ProfilesRepository(context, _userRepository);

        // Act
        await _userRepository.AddAsync(_validUser);
        await _profileRepository.AddAsync(_validProfile);
        var result = await context.Profiles.FindAsync(_validProfile.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_validProfile.Id, result.Id);
    }

    [Fact]
    private async Task AddAsync_WhenProfileIsValidAndTheUserExistsAndTheProfileExists_ShouldThrowException()
    {
        // Arrange
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        await context.Users.AddAsync(_validUser);
        await context.Profiles.AddAsync(_validProfile);
        await context.SaveChangesAsync();
        _userRepository = new UsersRepository(context);
        _profileRepository = new ProfilesRepository(context, _userRepository);

        // Act and Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _profileRepository.AddAsync(_validProfile));
    }

    [Fact]
    private async Task AddAsync_WhenProfileIsValidAndTheUserDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _userRepository = new UsersRepository(context);
        _profileRepository = new ProfilesRepository(context, _userRepository);

        // Act and Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _profileRepository.AddAsync(_validProfile));
    }

    [Fact]
    private async Task UpdateAsync_WhenProfileIsValidAndTheProfileExists_ShouldUpdateProfile()
    {
        // Arrange
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        await context.Users.AddAsync(_validUser);
        await context.Profiles.AddAsync(_validProfile);
        await context.SaveChangesAsync();
        _userRepository = new UsersRepository(context);
        _profileRepository = new ProfilesRepository(context, _userRepository);
        _validProfile.JobTitle = "Senior Software Engineer";
        _validProfile.Bio = "I am a senior software engineer";

        // Act
        await _profileRepository.UpdateAsync(_validProfile);
        var result = await context.Profiles.FindAsync(_validProfile.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_validProfile.JobTitle, result.JobTitle);
        Assert.Equal(_validProfile.Bio, result.Bio);
    }

    [Fact]
    private async Task UpdateAsync_WhenProfileIsValidAndTheProfileDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _userRepository = new UsersRepository(context);
        _profileRepository = new ProfilesRepository(context, _userRepository);

        // Act and Assert
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _profileRepository.UpdateAsync(_validProfile));
    }

    [Fact]
    private async Task UpdateAsync_WhenProfileIsNotValid_ShouldThrowException()
    {
        // Arrange
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _userRepository = new UsersRepository(context);
        _profileRepository = new ProfilesRepository(context, _userRepository);
        var invalidProfile = new Profile
        {
            Id = new string('n', GlobalEntitiesConstraints.MaxIdLength + 1),
            UserId = _validUser.Id
        };

        // Act and Assert
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _profileRepository.UpdateAsync(invalidProfile));
    }

    [Fact]
    private async Task DeleteAsync_WhenProfileExists_ShouldDeleteProfile()
    {
        // Arrange
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        await context.Users.AddAsync(_validUser);
        await context.Profiles.AddAsync(_validProfile);
        await context.SaveChangesAsync();
        _userRepository = new UsersRepository(context);
        _profileRepository = new ProfilesRepository(context, _userRepository);

        // Act
        await _profileRepository.DeleteAsync(_validProfile.Id);
        var result = await context.Users.FindAsync(_validUser.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsDeleted);
    }

    [Fact]
    private async Task DeleteAsync_WhenProfileDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var options = CreateNewContextOptions(_connection);
        await using var context = new ExpertBridgeDbContext(options);
        await context.Database.EnsureCreatedAsync();
        _userRepository = new UsersRepository(context);
        _profileRepository = new ProfilesRepository(context, _userRepository);

        // Act and Assert
        await Assert.ThrowsAsync<ProfileNotFoundException>(() => _profileRepository.DeleteAsync(_validProfile.Id));
    }
}
