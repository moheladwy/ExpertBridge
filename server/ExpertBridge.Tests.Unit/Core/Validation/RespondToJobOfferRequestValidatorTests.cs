// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Requests.RespondToJobOffer;
using FluentValidation.TestHelper;

namespace ExpertBridge.Tests.Unit.Core.Validation;

/// <summary>
///     Unit tests for RespondToJobOfferRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: Accept field validation (boolean, not null).
/// </remarks>
public sealed class RespondToJobOfferRequestValidatorTests
{
  private readonly RespondToJobOfferRequestValidator _validator;

  public RespondToJobOfferRequestValidatorTests()
  {
    _validator = new RespondToJobOfferRequestValidator();
  }

  #region Happy Path Tests

  [Fact]
  public async Task Should_Pass_When_Accept_Is_True()
  {
    // Arrange
    var request = new RespondToJobOfferRequest
    {
      Accept = true
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_When_Accept_Is_False()
  {
    // Arrange
    var request = new RespondToJobOfferRequest
    {
      Accept = false
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region Sad Path Tests

  [Fact]
  public async Task Should_Pass_With_Explicit_True_Value()
  {
    // Arrange
    var request = new RespondToJobOfferRequest
    {
      Accept = bool.Parse("true")
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public async Task Should_Pass_With_Explicit_False_Value()
  {
    // Arrange
    var request = new RespondToJobOfferRequest
    {
      Accept = bool.Parse("false")
    };

    // Act
    var result = await _validator.TestValidateAsync(request);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion
}
