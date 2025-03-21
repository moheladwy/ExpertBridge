// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities;
using ExpertBridge.Api.Core.Entities.Tags;
using FluentValidation.TestHelper;

namespace ExpertBridge.UnitTests.ValidationTests.Entities;

public class TagValidatorTests
{
    private readonly TagEntityValidator _tagValidator = new();
    private readonly Tag _tag = new()
    {
        Id = Guid.NewGuid().ToString(),
        Name = "Test Tag",
        Description = "Test Description"
    };

    [Fact]
    public void ValidateTag_WhenTagIsValid_ShouldReturnTrue()
    {
        // No need to arrange anything since the tag is already valid
        // Act
        var result = _tagValidator.TestValidate(_tag);
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidateTag_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var tagWithNullId = _tag;
        tagWithNullId.Id = null;

        var tagWithEmptyId = _tag;
        tagWithEmptyId.Id = string.Empty;

        var tagWithLongId = _tag;
        tagWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _tagValidator.TestValidate(tagWithNullId);
        var resultOfEmptyId = _tagValidator.TestValidate(tagWithEmptyId);
        var resultOfLongId = _tagValidator.TestValidate(tagWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidateTag_WhenNameIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var tagWithNullName = _tag;
        tagWithNullName.Name = null;

        var tagWithEmptyName = _tag;
        tagWithEmptyName.Name = string.Empty;

        var tagWithLongName = _tag;
        tagWithLongName.Name = new string('a', TagEntityConstraints.MaxNameLength + 1);

        // Act
        var resultOfNullName = _tagValidator.TestValidate(tagWithNullName);
        var resultOfEmptyName = _tagValidator.TestValidate(tagWithEmptyName);
        var resultOfLongName = _tagValidator.TestValidate(tagWithLongName);

        // Assert
        resultOfNullName.ShouldHaveValidationErrorFor(x => x.Name);
        resultOfEmptyName.ShouldHaveValidationErrorFor(x => x.Name);
        resultOfLongName.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ValidateTag_WhenDescriptionIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var tagWithNullDescription = _tag;
        tagWithNullDescription.Description = null;

        var tagWithEmptyDescription = _tag;
        tagWithEmptyDescription.Description = string.Empty;

        var tagWithLongDescription = _tag;
        tagWithLongDescription.Description = new string('a', TagEntityConstraints.MaxDescriptionLength + 1);

        // Act
        var resultOfNullDescription = _tagValidator.TestValidate(tagWithNullDescription);
        var resultOfEmptyDescription = _tagValidator.TestValidate(tagWithEmptyDescription);
        var resultOfLongDescription = _tagValidator.TestValidate(tagWithLongDescription);

        // Assert
        resultOfNullDescription.ShouldHaveValidationErrorFor(x => x.Description);
        resultOfEmptyDescription.ShouldHaveValidationErrorFor(x => x.Description);
        resultOfLongDescription.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
