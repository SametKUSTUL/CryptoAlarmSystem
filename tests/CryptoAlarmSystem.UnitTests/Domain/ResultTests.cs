using CryptoAlarmSystem.Domain.Common;
using CryptoAlarmSystem.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace CryptoAlarmSystem.UnitTests.Domain;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.ErrorCode.Should().Be(ErrorCode.None);
        result.ErrorMessage.Should().BeEmpty();
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult()
    {
        // Arrange
        var errorCode = ErrorCode.CryptoSymbolNotFound;
        var errorMessage = "Symbol not found";

        // Act
        var result = Result.Failure(errorCode, errorMessage);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(errorCode);
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void GenericSuccess_ShouldCreateSuccessResultWithData()
    {
        // Arrange
        var data = "Test Data";

        // Act
        var result = Result<string>.Success(data);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(data);
        result.ErrorCode.Should().Be(ErrorCode.None);
        result.ErrorMessage.Should().BeEmpty();
    }

    [Fact]
    public void GenericFailure_ShouldCreateFailureResultWithoutData()
    {
        // Arrange
        var errorCode = ErrorCode.DuplicateAlarm;
        var errorMessage = "Duplicate alarm exists";

        // Act
        var result = Result<string>.Failure(errorCode, errorMessage);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorCode.Should().Be(errorCode);
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void GenericSuccess_WithComplexObject_ShouldStoreData()
    {
        // Arrange
        var alarm = new { Id = 1, UserId = "user123", TargetPrice = 45000m };

        // Act
        var result = Result<object>.Success(alarm);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().Be(alarm);
    }
}
