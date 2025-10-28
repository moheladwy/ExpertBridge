// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Queries;
using ExpertBridge.Core.Entities.Tags;

namespace ExpertBridge.Tests.Unit.Contract.Queries;

/// <summary>
/// Unit tests for TagQueries extension methods.
/// </summary>
/// <remarks>
/// Tests cover SelectTagResponseFromTag projection method.
/// Uses in-memory EF Core database for realistic query execution.
/// </remarks>
public sealed class TagQueriesTests : IDisposable
{
    private readonly ExpertBridgeDbContext _context;

    public TagQueriesTests()
    {
        _context = InMemoryDbContextFixture.Create();
    }

    #region SelectTagResponseFromTag Tests

    [Fact]
    public async Task SelectTagResponseFromTag_Should_Project_EnglishName_When_Tag_Exists()
    {
        // Arrange
        var tag = TestDataBuilder.CreateTag("Technology", "تكنولوجيا", "tag1");
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Tags
            .SelectTagResponseFromTag()
            .FirstAsync();

        // Assert
        result.ShouldNotBeNull();
        result.EnglishName.ShouldBe("Technology");
        result.ArabicName.ShouldBe("تكنولوجيا");
    }

    [Fact]
    public async Task SelectTagResponseFromTag_Should_Handle_NullArabicName_Gracefully()
    {
        // Arrange
        var tag = TestDataBuilder.CreateTag("Science", null, "tag2");
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Tags
            .SelectTagResponseFromTag()
            .FirstAsync();

        // Assert
        result.EnglishName.ShouldBe("Science");
        result.ArabicName.ShouldBeNull();
    }

    [Fact]
    public async Task SelectTagResponseFromTag_Should_Project_MultipleTags_Correctly()
    {
        // Arrange
        var tags = new[]
        {
            TestDataBuilder.CreateTag("AI", "ذكاء اصطناعي", "tag1"),
            TestDataBuilder.CreateTag("ML", "تعلم آلي", "tag2"),
            TestDataBuilder.CreateTag("Data", null, "tag3")
        };
        _context.Tags.AddRange(tags);
        await _context.SaveChangesAsync();

        // Act
        var results = await _context.Tags
            .SelectTagResponseFromTag()
            .ToListAsync();

        // Assert
        results.Count.ShouldBe(3);
        results.ShouldAllBe(r => !string.IsNullOrEmpty(r.EnglishName));
        results[0].EnglishName.ShouldBe("AI");
        results[1].EnglishName.ShouldBe("ML");
        results[2].EnglishName.ShouldBe("Data");
    }

    [Fact]
    public async Task SelectTagResponseFromTag_Should_Return_Empty_When_NoTags()
    {
        // Arrange - no data added

        // Act
        var results = await _context.Tags
            .SelectTagResponseFromTag()
            .ToListAsync();

        // Assert
        results.ShouldBeEmpty();
    }

    [Fact]
    public async Task SelectTagResponseFromTag_Should_Project_Description_Correctly()
    {
        // Arrange
        var tag = new Tag
        {
            Id = "tag1",
            EnglishName = "Programming",
            ArabicName = "برمجة",
            Description = "Software development and coding"
        };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Tags
            .SelectTagResponseFromTag()
            .FirstAsync();

        // Assert
        result.Description.ShouldBe("Software development and coding");
    }

    [Fact]
    public async Task SelectTagResponseFromTag_Should_Project_TagId_Correctly()
    {
        // Arrange
        var tagId = "unique-tag-id-123";
        var tag = TestDataBuilder.CreateTag("Testing", id: tagId);
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Tags
            .SelectTagResponseFromTag()
            .FirstAsync();

        // Assert
        result.TagId.ShouldBe(tagId);
    }

    #endregion

    public void Dispose()
    {
        _context.Dispose();
    }
}
