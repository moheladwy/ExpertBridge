namespace ExpertBridge.UnitTests.ValidationTests.Entities;

public class ChatValidatorTests
{
    private readonly ChatEntityValidator _chatEntityValidator = new();

    private readonly Chat _validChat = new()
    {
        Id = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow.AddDays(-1), EndedAt = DateTime.MaxValue
    };

    [Fact]
    public void ValidateChat_WhenChatIsValid_ShouldReturnTrue()
    {
        // No need to arrange anything since the chat is already valid
        // Act
        var result = _chatEntityValidator.TestValidate(_validChat);
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidateChat_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var chatWithNullId = _validChat;
        chatWithNullId.Id = null;

        var chatWithEmptyId = _validChat;
        chatWithEmptyId.Id = string.Empty;

        var chatWithLongId = _validChat;
        chatWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _chatEntityValidator.TestValidate(chatWithNullId);
        var resultOfEmptyId = _chatEntityValidator.TestValidate(chatWithEmptyId);
        var resultOfLongId = _chatEntityValidator.TestValidate(chatWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidateChat_WhenCreatedAtIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var chatWithFutureCreatedAt = _validChat;
        chatWithFutureCreatedAt.CreatedAt = DateTime.MaxValue;

        // Act
        var resultOfFutureCreatedAt = _chatEntityValidator.TestValidate(chatWithFutureCreatedAt);

        // Assert
        resultOfFutureCreatedAt.ShouldHaveValidationErrorFor(x => x.CreatedAt);
    }

    [Fact]
    public void ValidateChat_WhenEndedAtIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var chatWithInvalidEndedAt = _validChat;
        chatWithInvalidEndedAt.EndedAt = _validChat.CreatedAt.Value.AddSeconds(-1);

        // Act
        var resultOfInvalidEndedAt = _chatEntityValidator.TestValidate(chatWithInvalidEndedAt);

        // Assert
        resultOfInvalidEndedAt.ShouldHaveValidationErrorFor(x => x.EndedAt);
    }
}
