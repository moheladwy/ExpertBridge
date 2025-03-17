using ExpertBridge.Api.Core.Entities;
using ExpertBridge.Api.Core.Entities.Area;
using FluentValidation.TestHelper;

namespace ExpertBridge.UnitTests.ValidationTests.Entities;

public class AreaValidatorTests
{
    private readonly AreaEntityValidator _areaEntityValidator = new();
    private readonly Area _validArea = new()
    {
        Id = Guid.NewGuid().ToString(),
        ProfileId = Guid.NewGuid().ToString(),
        Governorate = Governorates.Alexandria,
        Region = "Region"
    };

    [Fact]
    public void ValidateArea_WhenAreaIsValid_ShouldReturnTrue()
    {
        // No need to arrange anything since the area is already valid
        // Act
        var result = _areaEntityValidator.TestValidate(_validArea);
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidateArea_WhenIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var areaWithNullId = _validArea;
        areaWithNullId.Id = null;

        var areaWithEmptyId = _validArea;
        areaWithEmptyId.Id = string.Empty;

        var areaWithLongId = _validArea;
        areaWithLongId.Id = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullId = _areaEntityValidator.TestValidate(areaWithNullId);
        var resultOfEmptyId = _areaEntityValidator.TestValidate(areaWithEmptyId);
        var resultOfLongId = _areaEntityValidator.TestValidate(areaWithLongId);

        // Assert
        resultOfNullId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfEmptyId.ShouldHaveValidationErrorFor(x => x.Id);
        resultOfLongId.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidateArea_WhenProfileIdIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var areaWithNullProfileId = _validArea;
        areaWithNullProfileId.ProfileId = null;

        var areaWithEmptyProfileId = _validArea;
        areaWithEmptyProfileId.ProfileId = string.Empty;

        var areaWithLongProfileId = _validArea;
        areaWithLongProfileId.ProfileId = new string('a', GlobalEntitiesConstraints.MaxIdLength + 1);

        // Act
        var resultOfNullProfileId = _areaEntityValidator.TestValidate(areaWithNullProfileId);
        var resultOfEmptyProfileId = _areaEntityValidator.TestValidate(areaWithEmptyProfileId);
        var resultOfLongProfileId = _areaEntityValidator.TestValidate(areaWithLongProfileId);

        // Assert
        resultOfNullProfileId.ShouldHaveValidationErrorFor(x => x.ProfileId);
        resultOfEmptyProfileId.ShouldHaveValidationErrorFor(x => x.ProfileId);
        resultOfLongProfileId.ShouldHaveValidationErrorFor(x => x.ProfileId);
    }

    [Fact]
    public void ValidateArea_WhenGovernorateIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var areaWithInvalidGovernorate = _validArea;
        areaWithInvalidGovernorate.Governorate = (Governorates) 100;

        // Act
        var resultOfInvalidGovernorate = _areaEntityValidator.TestValidate(areaWithInvalidGovernorate);

        // Assert
        resultOfInvalidGovernorate.ShouldHaveValidationErrorFor(x => x.Governorate);
    }

    [Fact]
    public void ValidateArea_WhenRegionIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var areaWithNullRegion = _validArea;
        areaWithNullRegion.Region = null;

        var areaWithEmptyRegion = _validArea;
        areaWithEmptyRegion.Region = string.Empty;

        var areaWithLongRegion = _validArea;
        areaWithLongRegion.Region = new string('a', AreaEntityConstraints.MaxRegionLength + 1);

        // Act
        var resultOfNullRegion = _areaEntityValidator.TestValidate(areaWithNullRegion);
        var resultOfEmptyRegion = _areaEntityValidator.TestValidate(areaWithEmptyRegion);
        var resultOfLongRegion = _areaEntityValidator.TestValidate(areaWithLongRegion);

        // Assert
        resultOfNullRegion.ShouldHaveValidationErrorFor(x => x.Region);
        resultOfEmptyRegion.ShouldHaveValidationErrorFor(x => x.Region);
        resultOfLongRegion.ShouldHaveValidationErrorFor(x => x.Region);
    }
}
