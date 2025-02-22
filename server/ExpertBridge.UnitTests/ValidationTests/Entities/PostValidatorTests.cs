using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.Post;
using FluentValidation.TestHelper;

namespace ExpertBridge.UnitTests.ValidationTests.Entities;

public class PostValidatorTests
{
    private readonly PostEntityValidator _postEntityValidator = new();
    private readonly Post _validPost = new()
    {
        Id = Guid.NewGuid().ToString(),
        AuthorId = Guid.NewGuid().ToString(),
        Title = "Post Title",
        Content = "Post Content",
        CreatedAt = DateTime.UtcNow.AddDays(-1),
        LastModified = DateTime.MaxValue,
        isDeleted = false
    };

    [Fact]
    public void ValidatePost_WhenPostIsValid_ShouldReturnTrue()
    {
        // No need to arrange anything since the post is already valid
        // Act
        var result = _postEntityValidator.TestValidate(_validPost);
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidatePost_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var postWithNullId = _validPost;
        postWithNullId.Id = null;

        var postWithEmptyId = _validPost;
        postWithEmptyId.Id = string.Empty;

        var postWithLongId = _validPost;
        postWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _postEntityValidator.TestValidate(postWithNullId);
        var resultOfEmptyId = _postEntityValidator.TestValidate(postWithEmptyId);
        var resultOfLongId = _postEntityValidator.TestValidate(postWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidatePost_WhenTitleIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var postWithNullTitle = _validPost;
        postWithNullTitle.Title = null;

        var postWithEmptyTitle = _validPost;
        postWithEmptyTitle.Title = string.Empty;

        var postWithLongTitle = _validPost;
        postWithLongTitle.Title = new string('a', PostEntityConstraints.MaxTitleLength + 1);

        // Act
        var resultOfNullTitle = _postEntityValidator.TestValidate(postWithNullTitle);
        var resultOfEmptyTitle = _postEntityValidator.TestValidate(postWithEmptyTitle);
        var resultOfLongTitle = _postEntityValidator.TestValidate(postWithLongTitle);
    }

    [Fact]
    public void ValidatePost_WhenContentIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var postWithNullContent = _validPost;
        postWithNullContent.Content = null;

        var postWithEmptyContent = _validPost;
        postWithEmptyContent.Content = string.Empty;

        var postWithLongContent = _validPost;
        postWithLongContent.Content = new string('a', PostEntityConstraints.MaxContentLength + 1);

        // Act
        var resultOfNullContent = _postEntityValidator.TestValidate(postWithNullContent);
        var resultOfEmptyContent = _postEntityValidator.TestValidate(postWithEmptyContent);
        var resultOfLongContent = _postEntityValidator.TestValidate(postWithLongContent);
    }

    [Fact]
    public void ValidatePost_WhenAuthorIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var postWithNullAuthorId = _validPost;
        postWithNullAuthorId.AuthorId = null;

        var postWithEmptyAuthorId = _validPost;
        postWithEmptyAuthorId.AuthorId = string.Empty;

        var postWithLongAuthorId = _validPost;
        postWithLongAuthorId.AuthorId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullAuthorId = _postEntityValidator.TestValidate(postWithNullAuthorId);
        var resultOfEmptyAuthorId = _postEntityValidator.TestValidate(postWithEmptyAuthorId);
        var resultOfLongAuthorId = _postEntityValidator.TestValidate(postWithLongAuthorId);

        // Assert
        resultOfNullAuthorId.ShouldHaveValidationErrorFor(x => x.AuthorId);
        resultOfEmptyAuthorId.ShouldHaveValidationErrorFor(x => x.AuthorId);
        resultOfLongAuthorId.ShouldHaveValidationErrorFor(x => x.AuthorId);
    }

    [Fact]
    public void ValidatePost_WhenCreatedAtIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var postWithInvalidCreatedAt = _validPost;
        postWithInvalidCreatedAt.CreatedAt = DateTime.UtcNow.AddDays(1);

        // Act
        var resultOfInvalidCreatedAt = _postEntityValidator.TestValidate(postWithInvalidCreatedAt);

        // Assert
        resultOfInvalidCreatedAt.ShouldHaveValidationErrorFor(x => x.CreatedAt);
    }

    [Fact]
    public void ValidatePost_WhenLastModifiedIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var postWithInvalidLastModified = _validPost;
        postWithInvalidLastModified.LastModified = _validPost.CreatedAt.AddDays(-1);

        // Act
        var resultOfInvalidLastModified = _postEntityValidator.TestValidate(postWithInvalidLastModified);

        // Assert
        resultOfInvalidLastModified.ShouldHaveValidationErrorFor(x => x.LastModified);
    }

}
