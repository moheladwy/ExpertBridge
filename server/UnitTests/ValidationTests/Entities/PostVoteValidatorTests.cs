using Core.Entities;
using Core.Entities.PostVotes;
using FluentValidation.TestHelper;

namespace UnitTests.ValidationTests.Entities;

public class PostVoteValidatorTests
{
    private readonly PostVoteEntityValidator _postVoteEntityValidator = new();

    private readonly PostVote _validPostVote = new()
    {
        Id = Guid.NewGuid().ToString(),
        ProfileId = Guid.NewGuid().ToString(),
        PostId = Guid.NewGuid().ToString(),
        CreatedAt = DateTime.UtcNow.AddDays(-1),
        IsUpvote = true
    };

    [Fact]
    public void ValidatePostVote_WhenPostVoteIsValid_ShouldReturnTrue()
    {
        // No need to arrange anything since the post vote is already valid
        // Act
        var result = _postVoteEntityValidator.TestValidate(_validPostVote);
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidatePostVote_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var postVoteWithNullId = _validPostVote;
        postVoteWithNullId.Id = null;

        var postVoteWithEmptyId = _validPostVote;
        postVoteWithEmptyId.Id = string.Empty;

        var postVoteWithLongId = _validPostVote;
        postVoteWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _postVoteEntityValidator.TestValidate(postVoteWithNullId);
        var resultOfEmptyId = _postVoteEntityValidator.TestValidate(postVoteWithEmptyId);
        var resultOfLongId = _postVoteEntityValidator.TestValidate(postVoteWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidatePostVote_WhenProfileIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var postVoteWithNullProfileId = _validPostVote;
        postVoteWithNullProfileId.ProfileId = null;

        var postVoteWithEmptyProfileId = _validPostVote;
        postVoteWithEmptyProfileId.ProfileId = string.Empty;

        var postVoteWithLongProfileId = _validPostVote;
        postVoteWithLongProfileId.ProfileId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullProfileId = _postVoteEntityValidator.TestValidate(postVoteWithNullProfileId);
        var resultOfEmptyProfileId = _postVoteEntityValidator.TestValidate(postVoteWithEmptyProfileId);
        var resultOfLongProfileId = _postVoteEntityValidator.TestValidate(postVoteWithLongProfileId);

        // Assert
        resultOfNullProfileId.ShouldHaveValidationErrorFor(x => x.ProfileId);
        resultOfEmptyProfileId.ShouldHaveValidationErrorFor(x => x.ProfileId);
        resultOfLongProfileId.ShouldHaveValidationErrorFor(x => x.ProfileId);
    }

    [Fact]
    public void ValidatePostVote_WhenPostIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var postVoteWithNullPostId = _validPostVote;
        postVoteWithNullPostId.PostId = null;

        var postVoteWithEmptyPostId = _validPostVote;
        postVoteWithEmptyPostId.PostId = string.Empty;

        var postVoteWithLongPostId = _validPostVote;
        postVoteWithLongPostId.PostId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullPostId = _postVoteEntityValidator.TestValidate(postVoteWithNullPostId);
        var resultOfEmptyPostId = _postVoteEntityValidator.TestValidate(postVoteWithEmptyPostId);
        var resultOfLongPostId = _postVoteEntityValidator.TestValidate(postVoteWithLongPostId);

        // Assert
        resultOfNullPostId.ShouldHaveValidationErrorFor(x => x.PostId);
        resultOfEmptyPostId.ShouldHaveValidationErrorFor(x => x.PostId);
        resultOfLongPostId.ShouldHaveValidationErrorFor(x => x.PostId);
    }

    [Fact]
    public void ValidatePostVote_WhenCreatedAtIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var postVoteWithNullCreatedAt = _validPostVote;
        postVoteWithNullCreatedAt.CreatedAt = DateTime.UtcNow.AddDays(1);

        // Act
        var resultOfNullCreatedAt = _postVoteEntityValidator.TestValidate(postVoteWithNullCreatedAt);

        // Assert
        resultOfNullCreatedAt.ShouldHaveValidationErrorFor(x => x.CreatedAt);
    }
}
