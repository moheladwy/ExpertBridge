namespace ExpertBridge.UnitTests.ValidationTests.Entities;

public class CommentValidatorTests
{
    private readonly CommentEntityValidator _commentEntityValidator = new();

    private readonly Comment _validComment = new()
    {
        Id = Guid.NewGuid().ToString(),
        AuthorId = Guid.NewGuid().ToString(),
        Content = "Comment Content",
        CreatedAt = DateTime.UtcNow.AddDays(-1),
        LastModified = DateTime.UtcNow.AddMinutes(-4),
        PostId = Guid.NewGuid().ToString()
    };

    [Fact]
    public void ValidateComment_WhenCommentIsValid_ShouldReturnTrue()
    {
        // Arrange
        var commentWithoutParentId = _validComment;
        var commentWithParentId = _validComment;
        commentWithParentId.ParentCommentId = Guid.NewGuid().ToString();

        // Act
        var resultOfCommentWithoutParentId = _commentEntityValidator.TestValidate(commentWithoutParentId);
        var resultOfCommentWithParentId = _commentEntityValidator.TestValidate(commentWithParentId);

        // Assert
        resultOfCommentWithoutParentId.ShouldNotHaveAnyValidationErrors();
        resultOfCommentWithParentId.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidateComment_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var commentWithNullId = _validComment;
        commentWithNullId.Id = null;

        var commentWithEmptyId = _validComment;
        commentWithEmptyId.Id = string.Empty;

        var commentWithLongId = _validComment;
        commentWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _commentEntityValidator.TestValidate(commentWithNullId);
        var resultOfEmptyId = _commentEntityValidator.TestValidate(commentWithEmptyId);
        var resultOfLongId = _commentEntityValidator.TestValidate(commentWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidateComment_WhenAuthorIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var commentWithNullAuthorId = _validComment;
        commentWithNullAuthorId.AuthorId = null;

        var commentWithEmptyAuthorId = _validComment;
        commentWithEmptyAuthorId.AuthorId = string.Empty;

        var commentWithLongAuthorId = _validComment;
        commentWithLongAuthorId.AuthorId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullAuthorId = _commentEntityValidator.TestValidate(commentWithNullAuthorId);
        var resultOfEmptyAuthorId = _commentEntityValidator.TestValidate(commentWithEmptyAuthorId);
        var resultOfLongAuthorId = _commentEntityValidator.TestValidate(commentWithLongAuthorId);

        // Assert
        resultOfNullAuthorId.ShouldHaveValidationErrorFor(x => x.AuthorId);
        resultOfEmptyAuthorId.ShouldHaveValidationErrorFor(x => x.AuthorId);
        resultOfLongAuthorId.ShouldHaveValidationErrorFor(x => x.AuthorId);
    }

    [Fact]
    public void ValidateComment_WhenContentIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var commentWithNullContent = _validComment;
        commentWithNullContent.Content = null;

        var commentWithEmptyContent = _validComment;
        commentWithEmptyContent.Content = string.Empty;

        var commentWithLongContent = _validComment;
        commentWithLongContent.Content = new string('a', CommentEntityConstraints.MaxContentLength + 1);

        // Act
        var resultOfNullContent = _commentEntityValidator.TestValidate(commentWithNullContent);
        var resultOfEmptyContent = _commentEntityValidator.TestValidate(commentWithEmptyContent);
        var resultOfLongContent = _commentEntityValidator.TestValidate(commentWithLongContent);

        // Assert
        resultOfNullContent.ShouldHaveValidationErrorFor(x => x.Content);
        resultOfEmptyContent.ShouldHaveValidationErrorFor(x => x.Content);
        resultOfLongContent.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void ValidateComment_WhenCreatedAtIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var commentWithCreatedAtGreaterThanNow = _validComment;
        commentWithCreatedAtGreaterThanNow.CreatedAt = DateTime.UtcNow.AddDays(1);

        // Act
        var resultOfCreatedAtGreaterThanNow = _commentEntityValidator.TestValidate(commentWithCreatedAtGreaterThanNow);

        // Assert
        resultOfCreatedAtGreaterThanNow.ShouldHaveValidationErrorFor(x => x.CreatedAt);
    }

    [Fact]
    public void ValidateComment_WhenLastModifiedIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var commentWithLastModifiedGreaterThanNow = _validComment;
        commentWithLastModifiedGreaterThanNow.LastModified = DateTime.UtcNow.AddDays(1);

        var commentWithLastModifiedLessThanCreatedAt = _validComment;
        commentWithLastModifiedLessThanCreatedAt.LastModified = _validComment.CreatedAt.Value.AddDays(-1);

        // Act
        var resultOfLastModifiedGreaterThanNow =
            _commentEntityValidator.TestValidate(commentWithLastModifiedGreaterThanNow);
        var resultOfLastModifiedLessThanCreatedAt =
            _commentEntityValidator.TestValidate(commentWithLastModifiedLessThanCreatedAt);

        // Assert
        resultOfLastModifiedGreaterThanNow.ShouldHaveValidationErrorFor(x => x.LastModified);
        resultOfLastModifiedLessThanCreatedAt.ShouldHaveValidationErrorFor(x => x.LastModified);
    }
}
