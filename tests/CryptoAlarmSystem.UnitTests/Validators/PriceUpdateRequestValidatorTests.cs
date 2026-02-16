using CryptoAlarmSystem.Api.Controllers.V1;
using CryptoAlarmSystem.Api.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace CryptoAlarmSystem.UnitTests.Validators;

public class PriceUpdateRequestValidatorTests
{
    private readonly PriceUpdateRequestValidator _validator;

    public PriceUpdateRequestValidatorTests()
    {
        _validator = new PriceUpdateRequestValidator();
    }

    [Fact]
    public void Validate_ValidRequest_ShouldNotHaveValidationError()
    {
        // Arrange
        var request = new PriceUpdateRequest(CryptoSymbolId: 1, Price: 45000m);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidCryptoSymbolId_ShouldHaveValidationError(int symbolId)
    {
        // Arrange
        var request = new PriceUpdateRequest(CryptoSymbolId: symbolId, Price: 45000m);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CryptoSymbolId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Validate_InvalidPrice_ShouldHaveValidationError(decimal price)
    {
        // Arrange
        var request = new PriceUpdateRequest(CryptoSymbolId: 1, Price: price);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }
}
