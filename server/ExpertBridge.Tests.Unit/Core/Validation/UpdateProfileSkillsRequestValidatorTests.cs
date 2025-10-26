// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Requests.UpdateProfileSkills;
using FluentValidation.TestHelper;

namespace ExpertBridge.Tests.Unit.Core.Validation;

/// <summary>
///     Unit tests for UpdateProfileSkillsRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: Skills collection validation (required, individual skill ID validation, empty list allowed).
/// </remarks>
public sealed class UpdateProfileSkillsRequestValidatorTests
{
  private readonly UpdateProfileSkillsRequestValidator _validator;

  public UpdateProfileSkillsRequestValidatorTests()
  {
    _validator = new UpdateProfileSkillsRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_When_Request_Has_Valid_Single_Skill()
  {
    // Arrange
    var request = new UpdateProfileSkillsRequest
    {
      Skills = ["skill-id-1"]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Request_Has_Multiple_Valid_Skills()
  {
    // Arrange
    var request = new UpdateProfileSkillsRequest
    {
      Skills = ["skill-1", "skill-2", "skill-3", "skill-4", "skill-5"]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Skills_Is_Empty()
  {
    // Arrange
    var request = new UpdateProfileSkillsRequest
    {
      Skills = []
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Skill_ID_Has_Max_Length()
  {
    // Arrange
    var maxLengthSkill = new string('a', GlobalEntitiesConstraints.MaxIdLength);
    var request = new UpdateProfileSkillsRequest
    {
      Skills = [maxLengthSkill]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Skills_Has_Many_Items()
  {
    // Arrange
    var skills = Enumerable.Range(1, 100).Select(i => $"skill-{i}").ToList();
    var request = new UpdateProfileSkillsRequest
    {
      Skills = skills
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region Skills Collection Validation Tests

  [Fact]
  public async Task Should_Fail_When_Skills_Is_Null()
  {
    // Arrange
    var request = new UpdateProfileSkillsRequest
    {
      Skills = null!
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Skills)
        .WithErrorMessage("Skills cannot be null");
  }

  #endregion

  #region Individual Skill Validation Tests

  [Fact]
  public async Task Should_Fail_When_Skill_Is_Null()
  {
    // Arrange
    var request = new UpdateProfileSkillsRequest
    {
      Skills = [null!]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Skills[0]")
        .WithErrorMessage("Skill ID cannot be null");
  }

  [Fact]
  public async Task Should_Fail_When_Skill_Is_Empty()
  {
    // Arrange
    var request = new UpdateProfileSkillsRequest
    {
      Skills = [""]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Skills[0]")
        .WithErrorMessage("Skill ID cannot be empty");
  }

  [Fact]
  public async Task Should_Fail_When_Skill_Exceeds_Max_Length()
  {
    // Arrange
    var tooLongSkill = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);
    var request = new UpdateProfileSkillsRequest
    {
      Skills = [tooLongSkill]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Skills[0]")
        .WithErrorMessage($"Skill ID cannot be longer than {GlobalEntitiesConstraints.MaxIdLength} characters");
  }

  [Fact]
  public async Task Should_Fail_When_Multiple_Skills_Are_Invalid()
  {
    // Arrange
    var request = new UpdateProfileSkillsRequest
    {
      Skills = ["", null!, new string('a', GlobalEntitiesConstraints.MaxIdLength + 1)]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Skills[0]");
    result.ShouldHaveValidationErrorFor("Skills[1]");
    result.ShouldHaveValidationErrorFor("Skills[2]");
  }

  #endregion

  #region Edge Case Tests

  [Fact]
  public async Task Should_Fail_When_Some_Skills_Valid_Some_Invalid()
  {
    // Arrange
    var request = new UpdateProfileSkillsRequest
    {
      Skills = ["valid-skill-1", "", "valid-skill-2", null!]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldHaveValidationErrorFor("Skills[1]");
    result.ShouldHaveValidationErrorFor("Skills[3]");
  }

  [Fact]
  public async Task Should_Pass_When_Skill_Has_Special_Characters()
  {
    // Arrange
    var request = new UpdateProfileSkillsRequest
    {
      Skills = ["skill-with-dashes", "skill_with_underscores", "skill.with.dots"]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Skill_Has_Numbers()
  {
    // Arrange
    var request = new UpdateProfileSkillsRequest
    {
      Skills = ["skill123", "123skill", "skill-123-test"]
    };

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
  [InlineData(50)]
  [InlineData(100)]
  public async Task Should_Pass_When_Skill_Count_Is_Any_Valid_Number(int count)
  {
    // Arrange
    var skills = Enumerable.Range(1, count).Select(i => $"skill-{i}").ToList();
    var request = new UpdateProfileSkillsRequest
    {
      Skills = skills
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Theory]
  [InlineData(1, "a")]
  [InlineData(10, "aaaaaaaaaa")]
  [InlineData(100, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
  public async Task Should_Pass_When_Skill_Length_Is_Within_Limit(int length, string skillId)
  {
    // Arrange
    var request = new UpdateProfileSkillsRequest
    {
      Skills = [skillId]
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion
}
