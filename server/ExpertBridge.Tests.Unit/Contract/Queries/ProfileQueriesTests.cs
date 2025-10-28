// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Queries;
using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.CommentVotes;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkills;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Skills;
using ExpertBridge.Core.Entities.Users;

namespace ExpertBridge.Tests.Unit.Contract.Queries;

/// <summary>
/// Unit tests for ProfileQueries extension methods.
/// </summary>
/// <remarks>
/// Tests cover FullyPopulatedProfileQuery, SelectProfileResponseFromProfile, 
/// SelectAuthorResponseFromProfile, and SelectApplicantResponseFromProfile.
/// Uses in-memory EF Core database for realistic query execution.
/// </remarks>
public sealed class ProfileQueriesTests : IDisposable
{
  private readonly ExpertBridgeDbContext _context;

  public ProfileQueriesTests()
  {
    _context = InMemoryDbContextFixture.Create();
  }

  #region FullyPopulatedProfileQuery Tests

  [Fact]
  public async Task FullyPopulatedProfileQuery_Should_IncludeUser()
  {
    // Arrange
    var user = new User
    {
      Id = "user1",
      ProviderId = "provider1",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User",
      IsOnboarded = true
    };
    var profile = TestDataBuilder.CreateProfile("user1", id: "profile1");
    profile.User = user;

    _context.Users.Add(user);
    _context.Profiles.Add(profile);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Profiles
        .FullyPopulatedProfileQuery()
        .FirstAsync();

    // Assert
    result.ShouldNotBeNull();
    result.User.ShouldNotBeNull();
    result.User.Id.ShouldBe("user1");
    result.User.IsOnboarded.ShouldBeTrue();
  }

  [Fact]
  public async Task FullyPopulatedProfileQuery_Should_IncludeProfileSkillsWithSkills()
  {
    // Arrange
    var user = new User
    {
      Id = "user1",
      ProviderId = "provider1",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User",
    };
    var profile = TestDataBuilder.CreateProfile("user1", id: "profile1");
    profile.User = user;

    var skill1 = new Skill { Id = "skill1", Name = "C#", Description = "C# Programming" };
    var skill2 = new Skill { Id = "skill2", Name = "TypeScript", Description = "TypeScript" };

    var profileSkill1 = new ProfileSkill
    {
      ProfileId = "profile1",
      SkillId = "skill1",
      Profile = profile,
      Skill = skill1
    };
    var profileSkill2 = new ProfileSkill
    {
      ProfileId = "profile1",
      SkillId = "skill2",
      Profile = profile,
      Skill = skill2
    };

    _context.Users.Add(user);
    _context.Profiles.Add(profile);
    _context.Skills.AddRange(skill1, skill2);
    _context.ProfileSkills.AddRange(profileSkill1, profileSkill2);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Profiles
        .FullyPopulatedProfileQuery()
        .FirstAsync();

    // Assert
    result.ProfileSkills.ShouldNotBeNull();
    result.ProfileSkills.Count.ShouldBe(2);
    result.ProfileSkills.First().Skill.ShouldNotBeNull();
    result.ProfileSkills.First().Skill.Name.ShouldBe("C#");
  }

  [Fact]
  public async Task FullyPopulatedProfileQuery_Should_IncludeCommentsWithVotes()
  {
    // Arrange
    var user = new User
    {
      Id = "user1",
      ProviderId = "provider1",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User",
    };
    var profile = TestDataBuilder.CreateProfile("user1", id: "profile1");
    profile.User = user;

    var comment = TestDataBuilder.CreateComment("profile1", postId: "post1", id: "comment1");
    var vote1 = TestDataBuilder.CreateCommentVote("comment1", "profile2", isUpvote: true);
    var vote2 = TestDataBuilder.CreateCommentVote("comment1", "profile3", isUpvote: false);

    comment.Votes = new List<CommentVote> { vote1, vote2 };
    profile.Comments = new List<Comment> { comment };

    _context.Users.Add(user);
    _context.Profiles.Add(profile);
    _context.Comments.Add(comment);
    _context.CommentVotes.AddRange(vote1, vote2);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Profiles
        .FullyPopulatedProfileQuery()
        .FirstAsync();

    // Assert
    result.Comments.ShouldNotBeNull();
    result.Comments.Count.ShouldBe(1);
    result.Comments.First().Votes.ShouldNotBeNull();
    result.Comments.First().Votes.Count.ShouldBe(2);
  }

  [Fact]
  public async Task FullyPopulatedProfileQuery_WithPredicate_Should_FilterCorrectly()
  {
    // Arrange
    var user1 = new User
    {
      Id = "user1",
      ProviderId = "provider1",
      Email = "test1@example.com",
      Username = "user1",
      FirstName = "Test",
      LastName = "User1"
    };
    var user2 = new User
    {
      Id = "user2",
      ProviderId = "provider2",
      Email = "test2@example.com",
      Username = "user2",
      FirstName = "Test",
      LastName = "User2"
    };

    var profile1 = TestDataBuilder.CreateProfile("user1", username: "john_doe", id: "profile1");
    var profile2 = TestDataBuilder.CreateProfile("user2", username: "jane_smith", id: "profile2");
    profile1.User = user1;
    profile2.User = user2;

    _context.Users.AddRange(user1, user2);
    _context.Profiles.AddRange(profile1, profile2);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Profiles
        .FullyPopulatedProfileQuery(p => p.Username == "john_doe")
        .ToListAsync();

    // Assert
    result.Count.ShouldBe(1);
    result.First().Username.ShouldBe("john_doe");
  }

  [Fact]
  public async Task FullyPopulatedProfileQuery_Should_UseAsNoTracking()
  {
    // Arrange
    var user = new User
    {
      Id = "user1",
      ProviderId = "provider1",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User",
    };
    var profile = TestDataBuilder.CreateProfile("user1", id: "profile1");
    profile.User = user;

    _context.Users.Add(user);
    _context.Profiles.Add(profile);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Profiles
        .FullyPopulatedProfileQuery()
        .FirstAsync();

    // Assert
    var entry = _context.Entry(result);
    entry.State.ShouldBe(EntityState.Detached);
  }

  #endregion

  #region SelectProfileResponseFromProfile Tests

  [Fact]
  public async Task SelectProfileResponseFromProfile_Should_ProjectAllBasicProperties()
  {
    // Arrange
    var user = new User
    {
      Id = "user1",
      ProviderId = "provider1",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User",
      IsOnboarded = true
    };
    var profile = TestDataBuilder.CreateProfile(
        userId: "user1",
        email: "test@example.com",
        username: "testuser",
        firstName: "John",
        lastName: "Doe",
        jobTitle: "Software Engineer",
        profilePictureUrl: "https://example.com/pic.jpg",
        id: "profile1"
    );
    profile.User = user;
    profile.Bio = "Experienced developer";
    profile.PhoneNumber = "+1234567890";
    profile.IsBanned = false;
    profile.Rating = 4.5;
    profile.RatingCount = 10;

    _context.Users.Add(user);
    _context.Profiles.Add(profile);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Profiles
        .SelectProfileResponseFromProfile()
        .FirstAsync();

    // Assert
    result.Id.ShouldBe("profile1");
    result.UserId.ShouldBe("user1");
    result.Email.ShouldBe("test@example.com");
    result.Username.ShouldBe("testuser");
    result.FirstName.ShouldBe("John");
    result.LastName.ShouldBe("Doe");
    result.JobTitle.ShouldBe("Software Engineer");
    result.ProfilePictureUrl.ShouldBe("https://example.com/pic.jpg");
    result.Bio.ShouldBe("Experienced developer");
    result.PhoneNumber.ShouldBe("+1234567890");
    result.IsBanned.ShouldBeFalse();
    result.Rating.ShouldBe(4.5);
    result.RatingCount.ShouldBe(10);
    result.IsOnboarded.ShouldBeTrue();
    result.CreatedAt.ShouldBeGreaterThan(DateTime.MinValue);
  }

  [Fact]
  public async Task SelectProfileResponseFromProfile_Should_CalculateReputationCorrectly()
  {
    // Arrange
    var user = new User
    {
      Id = "user1",
      ProviderId = "provider1",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };
    var profile = TestDataBuilder.CreateProfile("user1", id: "profile1");
    profile.User = user;

    var comment1 = TestDataBuilder.CreateComment("profile1", postId: "post1", id: "comment1");
    var comment2 = TestDataBuilder.CreateComment("profile1", postId: "post1", id: "comment2");

    // Comment1: 3 upvotes, 1 downvote
    var vote1 = TestDataBuilder.CreateCommentVote("comment1", "profile2", isUpvote: true);
    var vote2 = TestDataBuilder.CreateCommentVote("comment1", "profile3", isUpvote: true);
    var vote3 = TestDataBuilder.CreateCommentVote("comment1", "profile4", isUpvote: true);
    var vote4 = TestDataBuilder.CreateCommentVote("comment1", "profile5", isUpvote: false);

    // Comment2: 2 upvotes, 2 downvotes
    var vote5 = TestDataBuilder.CreateCommentVote("comment2", "profile6", isUpvote: true);
    var vote6 = TestDataBuilder.CreateCommentVote("comment2", "profile7", isUpvote: true);
    var vote7 = TestDataBuilder.CreateCommentVote("comment2", "profile8", isUpvote: false);
    var vote8 = TestDataBuilder.CreateCommentVote("comment2", "profile9", isUpvote: false);

    comment1.Votes = new List<CommentVote> { vote1, vote2, vote3, vote4 };
    comment2.Votes = new List<CommentVote> { vote5, vote6, vote7, vote8 };
    profile.Comments = new List<Comment> { comment1, comment2 };

    _context.Users.Add(user);
    _context.Profiles.Add(profile);
    _context.Comments.AddRange(comment1, comment2);
    _context.CommentVotes.AddRange(vote1, vote2, vote3, vote4, vote5, vote6, vote7, vote8);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Profiles
        .SelectProfileResponseFromProfile()
        .FirstAsync();

    // Assert
    // Total upvotes: 3 + 2 = 5
    // Total downvotes: 1 + 2 = 3
    // Reputation: 5 - 3 = 2
    result.CommentsUpvotes.ShouldBe(5);
    result.CommentsDownvotes.ShouldBe(3);
    result.Reputation.ShouldBe(2);
  }

  [Fact]
  public async Task SelectProfileResponseFromProfile_Should_ExtractSkillNames()
  {
    // Arrange
    var user = new User
    {
      Id = "user1",
      ProviderId = "provider1",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };
    var profile = TestDataBuilder.CreateProfile("user1", id: "profile1");
    profile.User = user;

    var skill1 = new Skill { Id = "skill1", Name = "C#", Description = "C#" };
    var skill2 = new Skill { Id = "skill2", Name = "TypeScript", Description = "TS" };
    var skill3 = new Skill { Id = "skill3", Name = "PostgreSQL", Description = "DB" };

    var profileSkill1 = new ProfileSkill
    {
      ProfileId = "profile1",
      SkillId = "skill1",
      Profile = profile,
      Skill = skill1
    };
    var profileSkill2 = new ProfileSkill
    {
      ProfileId = "profile1",
      SkillId = "skill2",
      Profile = profile,
      Skill = skill2
    };
    var profileSkill3 = new ProfileSkill
    {
      ProfileId = "profile1",
      SkillId = "skill3",
      Profile = profile,
      Skill = skill3
    };

    _context.Users.Add(user);
    _context.Profiles.Add(profile);
    _context.Skills.AddRange(skill1, skill2, skill3);
    _context.ProfileSkills.AddRange(profileSkill1, profileSkill2, profileSkill3);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Profiles
        .SelectProfileResponseFromProfile()
        .FirstAsync();

    // Assert
    result.Skills.ShouldNotBeNull();
    result.Skills.Count.ShouldBe(3);
    result.Skills.ShouldContain("C#");
    result.Skills.ShouldContain("TypeScript");
    result.Skills.ShouldContain("PostgreSQL");
  }

  [Fact]
  public async Task SelectProfileResponseFromProfile_Should_HandleNoSkills()
  {
    // Arrange
    var user = new User
    {
      Id = "user1",
      ProviderId = "provider1",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };
    var profile = TestDataBuilder.CreateProfile("user1", id: "profile1");
    profile.User = user;

    _context.Users.Add(user);
    _context.Profiles.Add(profile);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Profiles
        .SelectProfileResponseFromProfile()
        .FirstAsync();

    // Assert
    result.Skills.ShouldNotBeNull();
    result.Skills.ShouldBeEmpty();
  }

  [Fact]
  public async Task SelectProfileResponseFromProfile_Should_HandleNoComments()
  {
    // Arrange
    var user = new User
    {
      Id = "user1",
      ProviderId = "provider1",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };
    var profile = TestDataBuilder.CreateProfile("user1", id: "profile1");
    profile.User = user;

    _context.Users.Add(user);
    _context.Profiles.Add(profile);
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Profiles
        .SelectProfileResponseFromProfile()
        .FirstAsync();

    // Assert
    result.CommentsUpvotes.ShouldBe(0);
    result.CommentsDownvotes.ShouldBe(0);
    result.Reputation.ShouldBe(0);
  }

  #endregion

  #region SelectAuthorResponseFromProfile Tests

  [Fact]
  public void SelectAuthorResponseFromProfile_Should_ProjectCorrectly()
  {
    // Arrange
    var user = new User
    {
      Id = "user1",
      ProviderId = "provider1",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };
    var profile = TestDataBuilder.CreateProfile(
        userId: "user1",
        username: "johndoe",
        firstName: "John",
        lastName: "Doe",
        jobTitle: "Senior Developer",
        profilePictureUrl: "https://example.com/pic.jpg",
        id: "profile1"
    );
    profile.User = user;

    // Act
    var result = profile.SelectAuthorResponseFromProfile();

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe("profile1");
    result.UserId.ShouldBe("user1");
    result.Username.ShouldBe("johndoe");
    result.FirstName.ShouldBe("John");
    result.LastName.ShouldBe("Doe");
    result.JobTitle.ShouldBe("Senior Developer");
    result.ProfilePictureUrl.ShouldBe("https://example.com/pic.jpg");
  }

  [Fact]
  public void SelectAuthorResponseFromProfile_Should_ReturnNull_WhenProfileIsNull()
  {
    // Arrange
    Profile? profile = null;

    // Act
    var result = profile.SelectAuthorResponseFromProfile();

    // Assert
    result.ShouldBeNull();
  }

  #endregion

  #region SelectApplicantResponseFromProfile Tests

  [Fact]
  public void SelectApplicantResponseFromProfile_Should_ProjectCorrectly()
  {
    // Arrange
    var user = new User
    {
      Id = "user1",
      ProviderId = "provider1",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };
    var profile = TestDataBuilder.CreateProfile(
        userId: "user1",
        username: "johndoe",
        firstName: "John",
        lastName: "Doe",
        jobTitle: "Senior Developer",
        profilePictureUrl: "https://example.com/pic.jpg",
        id: "profile1"
    );
    profile.User = user;

    // Add comments with votes for reputation calculation
    var comment1 = TestDataBuilder.CreateComment("profile1", postId: "post1", id: "comment1");
    var vote1 = TestDataBuilder.CreateCommentVote("comment1", "profile2", isUpvote: true);
    var vote2 = TestDataBuilder.CreateCommentVote("comment1", "profile3", isUpvote: true);
    var vote3 = TestDataBuilder.CreateCommentVote("comment1", "profile4", isUpvote: false);

    comment1.Votes = new List<CommentVote> { vote1, vote2, vote3 };
    profile.Comments = new List<Comment> { comment1 };

    // Act
    var result = profile.SelectApplicantResponseFromProfile();

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe("profile1");
    result.UserId.ShouldBe("user1");
    result.Username.ShouldBe("johndoe");
    result.FirstName.ShouldBe("John");
    result.LastName.ShouldBe("Doe");
    result.JobTitle.ShouldBe("Senior Developer");
    result.ProfilePictureUrl.ShouldBe("https://example.com/pic.jpg");
    result.Reputation.ShouldBe(1); // 2 upvotes - 1 downvote = 1
  }

  [Fact]
  public void SelectApplicantResponseFromProfile_Should_ReturnNull_WhenProfileIsNull()
  {
    // Arrange
    Profile? profile = null;

    // Act
    var result = profile.SelectApplicantResponseFromProfile();

    // Assert
    result.ShouldBeNull();
  }

  [Fact]
  public void SelectApplicantResponseFromProfile_Should_CalculateZeroReputation_WhenNoComments()
  {
    // Arrange
    var user = new User
    {
      Id = "user1",
      ProviderId = "provider1",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };
    var profile = TestDataBuilder.CreateProfile("user1", id: "profile1");
    profile.User = user;
    profile.Comments = new List<Comment>();

    // Act
    var result = profile.SelectApplicantResponseFromProfile();

    // Assert
    result.ShouldNotBeNull();
    result.Reputation.ShouldBe(0);
  }

  [Fact]
  public void SelectApplicantResponseFromProfile_Should_CalculateNegativeReputation()
  {
    // Arrange
    var user = new User
    {
      Id = "user1",
      ProviderId = "provider1",
      Email = "test@example.com",
      Username = "testuser",
      FirstName = "Test",
      LastName = "User"
    };
    var profile = TestDataBuilder.CreateProfile("user1", id: "profile1");
    profile.User = user;

    var comment1 = TestDataBuilder.CreateComment("profile1", postId: "post1", id: "comment1");
    var vote1 = TestDataBuilder.CreateCommentVote("comment1", "profile2", isUpvote: true);
    var vote2 = TestDataBuilder.CreateCommentVote("comment1", "profile3", isUpvote: false);
    var vote3 = TestDataBuilder.CreateCommentVote("comment1", "profile4", isUpvote: false);
    var vote4 = TestDataBuilder.CreateCommentVote("comment1", "profile5", isUpvote: false);

    comment1.Votes = new List<CommentVote> { vote1, vote2, vote3, vote4 };
    profile.Comments = new List<Comment> { comment1 };

    // Act
    var result = profile.SelectApplicantResponseFromProfile();

    // Assert
    result.ShouldNotBeNull();
    result.Reputation.ShouldBe(-2); // 1 upvote - 3 downvotes = -2
  }

  #endregion

  public void Dispose()
  {
    _context.Dispose();
  }
}
