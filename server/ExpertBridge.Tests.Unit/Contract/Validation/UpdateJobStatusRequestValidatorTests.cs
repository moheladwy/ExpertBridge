// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Tests.Unit.Contract.Validation;

/// <summary>
///     Unit tests for UpdateJobStatusRequestValidator using FluentValidation.TestHelper.
/// </summary>
/// <remarks>
///     Tests cover: Status field validation (required, max length).
/// </remarks>
public sealed class UpdateJobStatusRequestValidatorTests
{
    private readonly UpdateJobStatusRequestValidator _validator;

    public UpdateJobStatusRequestValidatorTests()
    {
        _validator = new UpdateJobStatusRequestValidator();
    }

    #region Happy Path Tests

    [Fact]
    public async Task Should_Pass_With_Valid_Status()
    {
        // Arrange
        var request = new UpdateJobStatusRequest { Status = "InProgress" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_With_Single_Character_Status()
    {
        // Arrange
        var request = new UpdateJobStatusRequest { Status = "A" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_With_Maximum_Length_Status()
    {
        // Arrange
        var request = new UpdateJobStatusRequest
        {
            Status = new string('a', 128) // Max enum length
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_With_Different_Status_Values()
    {
        // Arrange
        var statuses = new[] { "Pending", "Active", "Completed", "Cancelled", "OnHold" };

        foreach (var status in statuses)
        {
            var request = new UpdateJobStatusRequest { Status = status };

            // Act
            var result = await _validator.TestValidateAsync(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region Status Validation Tests

    [Fact]
    public async Task Should_Fail_When_Status_Is_Null()
    {
        // Arrange
        var request = new UpdateJobStatusRequest { Status = null! };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("Status cannot be null");
    }

    [Fact]
    public async Task Should_Fail_When_Status_Is_Empty()
    {
        // Arrange
        var request = new UpdateJobStatusRequest { Status = string.Empty };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("Status cannot be empty");
    }

    [Fact]
    public async Task Should_Fail_When_Status_Is_Whitespace()
    {
        // Arrange
        var request = new UpdateJobStatusRequest { Status = "   " };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("Status cannot be empty");
    }

    [Fact]
    public async Task Should_Fail_When_Status_Exceeds_MaxLength()
    {
        // Arrange
        var request = new UpdateJobStatusRequest
        {
            Status = new string('a', 129) // Max is 128
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("Status cannot be longer than 128 characters");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task Should_Pass_With_PascalCase_Status()
    {
        // Arrange
        var request = new UpdateJobStatusRequest { Status = "InProgress" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_With_Uppercase_Status()
    {
        // Arrange
        var request = new UpdateJobStatusRequest { Status = "COMPLETED" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_With_Lowercase_Status()
    {
        // Arrange
        var request = new UpdateJobStatusRequest { Status = "pending" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_With_Status_Containing_Numbers()
    {
        // Arrange
        var request = new UpdateJobStatusRequest { Status = "Status123" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Pass_With_Status_At_Exact_MaxLength()
    {
        // Arrange
        var request = new UpdateJobStatusRequest { Status = new string('x', 128) };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
