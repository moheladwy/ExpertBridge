// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for OnboardUserRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: Tags collection validation (required, count limits, individual tag ID validation).
/// </remarks>
public sealed class OnboardUserRequestValidatorTests
{
    private readonly OnboardUserRequestValidator _validator;

    public OnboardUserRequestValidatorTests()
    {
        _validator = new OnboardUserRequestValidator();
    }

    #region Happy Path Tests

    [Fact]
    public async Task Should_Pass_When_Request_Has_Valid_Single_Tag()
    {
        // Arrange
        var request = new OnboardUserRequest { Tags = ["tag-id-1"] };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_When_Request_Has_Multiple_Valid_Tags()
    {
        // Arrange
        var request = new OnboardUserRequest { Tags = ["tag-1", "tag-2", "tag-3", "tag-4", "tag-5"] };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_When_Request_Has_Exactly_20_Tags()
    {
        // Arrange
        var tags = Enumerable.Range(1, 20).Select(i => $"tag-{i}").ToList();
        var request = new OnboardUserRequest { Tags = tags };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_When_Tag_ID_Has_Max_Length()
    {
        // Arrange
        var maxLengthTag = new string('a', GlobalEntitiesConstraints.MaxIdLength);
        var request = new OnboardUserRequest { Tags = [maxLengthTag] };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region Tags Collection Validation Tests

    [Fact]
    public async Task Should_Fail_When_Tags_Is_Null()
    {
        // Arrange
        var request = new OnboardUserRequest { Tags = null! };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags)
            .WithErrorMessage("Tags cannot be null");
    }

    [Fact]
    public async Task Should_Fail_When_Tags_Is_Empty()
    {
        // Arrange
        var request = new OnboardUserRequest { Tags = [] };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags)
            .WithErrorMessage("At least one tag must be selected");
    }

    [Fact]
    public async Task Should_Fail_When_Tags_Exceeds_20()
    {
        // Arrange
        var tags = Enumerable.Range(1, 21).Select(i => $"tag-{i}").ToList();
        var request = new OnboardUserRequest { Tags = tags };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags)
            .WithErrorMessage("Cannot select more than 20 tags");
    }

    #endregion

    #region Individual Tag Validation Tests

    [Fact]
    public async Task Should_Fail_When_Tag_Is_Null()
    {
        // Arrange
        var request = new OnboardUserRequest { Tags = [null!] };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Tags[0]")
            .WithErrorMessage("Tag ID cannot be null");
    }

    [Fact]
    public async Task Should_Fail_When_Tag_Is_Empty()
    {
        // Arrange
        var request = new OnboardUserRequest { Tags = [""] };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Tags[0]")
            .WithErrorMessage("Tag ID cannot be empty");
    }

    [Fact]
    public async Task Should_Fail_When_Tag_Exceeds_Max_Length()
    {
        // Arrange
        var tooLongTag = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);
        var request = new OnboardUserRequest { Tags = [tooLongTag] };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Tags[0]")
            .WithErrorMessage($"Tag ID cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
    }

    [Fact]
    public async Task Should_Fail_When_Multiple_Tags_Are_Invalid()
    {
        // Arrange
        var request = new OnboardUserRequest
        {
            Tags = ["", null!, new string('a', GlobalEntitiesConstraints.MaxIdLength + 1)]
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Tags[0]");
        result.ShouldHaveValidationErrorFor("Tags[1]");
        result.ShouldHaveValidationErrorFor("Tags[2]");
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task Should_Fail_When_Some_Tags_Valid_Some_Invalid()
    {
        // Arrange
        var request = new OnboardUserRequest { Tags = ["valid-tag-1", "", "valid-tag-2", null!] };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Tags[1]");
        result.ShouldHaveValidationErrorFor("Tags[3]");
    }

    [Fact]
    public async Task Should_Pass_When_Tag_Has_Special_Characters()
    {
        // Arrange
        var request = new OnboardUserRequest { Tags = ["tag-with-dashes", "tag_with_underscores", "tag.with.dots"] };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region Parameterized Tests

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public async Task Should_Pass_When_Tag_Count_Is_Valid(int count)
    {
        // Arrange
        var tags = Enumerable.Range(1, count).Select(i => $"tag-{i}").ToList();
        var request = new OnboardUserRequest { Tags = tags };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(21)]
    [InlineData(25)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task Should_Fail_When_Tag_Count_Exceeds_Maximum(int count)
    {
        // Arrange
        var tags = Enumerable.Range(1, count).Select(i => $"tag-{i}").ToList();
        var request = new OnboardUserRequest { Tags = tags };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags)
            .WithErrorMessage("Cannot select more than 20 tags");
    }

    #endregion
}
