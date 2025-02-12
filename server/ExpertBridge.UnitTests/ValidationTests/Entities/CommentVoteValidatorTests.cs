using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.Votes.CommentVote;
using FluentValidation.TestHelper;

namespace ExpertBridge.UnitTests.ValidationTests.Entities;

public class CommentVoteValidatorTests
{
    private readonly CommentVoteEntityValidator _commentVoteEntityValidator = new();
    private readonly CommentVote _validCommentVote = new()
    {
        Id = Guid.NewGuid().ToString(),
        CommentId = Guid.NewGuid().ToString(),
        ProfileId = Guid.NewGuid().ToString(),
        IsUpvote = true,
        CreatedAt = DateTime.Now.AddDays(-1)
    };

    [Fact]
    public void ValidateCommentVote_WhenCommentVoteIsValid_ShouldReturnTrue()
    {
        // No need to arrange anything since the comment vote is already valid
        // Act
        var result = _commentVoteEntityValidator.TestValidate(_validCommentVote);
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidateCommentVote_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var commentVoteWithNullId = _validCommentVote;
        commentVoteWithNullId.Id = null;

        var commentVoteWithEmptyId = _validCommentVote;
        commentVoteWithEmptyId.Id = string.Empty;

        var commentVoteWithLongId = _validCommentVote;
        commentVoteWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _commentVoteEntityValidator.TestValidate(commentVoteWithNullId);
        var resultOfEmptyId = _commentVoteEntityValidator.TestValidate(commentVoteWithEmptyId);
        var resultOfLongId = _commentVoteEntityValidator.TestValidate(commentVoteWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidateCommentVote_WhenCommentIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var commentVoteWithNullCommentId = _validCommentVote;
        commentVoteWithNullCommentId.CommentId = null;

        var commentVoteWithEmptyCommentId = _validCommentVote;
        commentVoteWithEmptyCommentId.CommentId = string.Empty;

        var commentVoteWithLongCommentId = _validCommentVote;
        commentVoteWithLongCommentId.CommentId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullCommentId = _commentVoteEntityValidator.TestValidate(commentVoteWithNullCommentId);
        var resultOfEmptyCommentId = _commentVoteEntityValidator.TestValidate(commentVoteWithEmptyCommentId);
        var resultOfLongCommentId = _commentVoteEntityValidator.TestValidate(commentVoteWithLongCommentId);

        // Assert
        resultOfNullCommentId.ShouldHaveValidationErrorFor(x => x.CommentId);
        resultOfEmptyCommentId.ShouldHaveValidationErrorFor(x => x.CommentId);
        resultOfLongCommentId.ShouldHaveValidationErrorFor(x => x.CommentId);
    }

    [Fact]
    public void ValidateCommentVote_WhenProfileIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var commentVoteWithNullProfileId = _validCommentVote;
        commentVoteWithNullProfileId.ProfileId = null;

        var commentVoteWithEmptyProfileId = _validCommentVote;
        commentVoteWithEmptyProfileId.ProfileId = string.Empty;

        var commentVoteWithLongProfileId = _validCommentVote;
        commentVoteWithLongProfileId.ProfileId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullProfileId = _commentVoteEntityValidator.TestValidate(commentVoteWithNullProfileId);
        var resultOfEmptyProfileId = _commentVoteEntityValidator.TestValidate(commentVoteWithEmptyProfileId);
        var resultOfLongProfileId = _commentVoteEntityValidator.TestValidate(commentVoteWithLongProfileId);

        // Assert
        resultOfNullProfileId.ShouldHaveValidationErrorFor(x => x.ProfileId);
        resultOfEmptyProfileId.ShouldHaveValidationErrorFor(x => x.ProfileId);
        resultOfLongProfileId.ShouldHaveValidationErrorFor(x => x.ProfileId);
    }

    [Fact]
    public void ValidateCommentVote_WhenCreatedAtIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var commentVoteWithFutureCreatedAt = _validCommentVote;
        commentVoteWithFutureCreatedAt.CreatedAt = DateTime.Now.AddDays(1);

        // Act
        var resultOfFutureCreatedAt = _commentVoteEntityValidator.TestValidate(commentVoteWithFutureCreatedAt);

        // Assert
        resultOfFutureCreatedAt.ShouldHaveValidationErrorFor(x => x.CreatedAt);
    }
}
