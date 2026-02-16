using CryptoAlarmSystem.Domain.Entities;
using CryptoAlarmSystem.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace CryptoAlarmSystem.UnitTests.Domain;

public class AlarmEntityTests
{
    [Fact]
    public void Alarm_ShouldInitializeWithDefaultValues()
    {
        // Act
        var alarm = new Alarm();

        // Assert
        alarm.Id.Should().Be(0);
        alarm.TriggeredPrice.Should().BeNull();
        alarm.TriggeredAt.Should().BeNull();
        alarm.AlarmNotificationChannels.Should().NotBeNull();
        alarm.NotificationLogs.Should().NotBeNull();
    }

    [Fact]
    public void Alarm_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var userId = "user123";
        var symbolId = 1;
        var alarmTypeId = 1;
        var targetPrice = 45000m;
        var createdAt = DateTime.UtcNow;

        // Act
        var alarm = new Alarm
        {
            UserId = userId,
            CryptoSymbolId = symbolId,
            AlarmTypeId = alarmTypeId,
            TargetPrice = targetPrice,
            CreatedAt = createdAt,
            Status = AlarmStatus.Active
        };

        // Assert
        alarm.UserId.Should().Be(userId);
        alarm.CryptoSymbolId.Should().Be(symbolId);
        alarm.AlarmTypeId.Should().Be(alarmTypeId);
        alarm.TargetPrice.Should().Be(targetPrice);
        alarm.CreatedAt.Should().Be(createdAt);
        alarm.Status.Should().Be(AlarmStatus.Active);
    }

    [Fact]
    public void Alarm_WhenTriggered_ShouldSetTriggeredProperties()
    {
        // Arrange
        var alarm = new Alarm
        {
            UserId = "user123",
            CryptoSymbolId = 1,
            AlarmTypeId = 1,
            TargetPrice = 45000m,
            Status = AlarmStatus.Active
        };

        var triggeredPrice = 45500m;
        var triggeredAt = DateTime.UtcNow;

        // Act
        alarm.Status = AlarmStatus.Triggered;
        alarm.TriggeredPrice = triggeredPrice;
        alarm.TriggeredAt = triggeredAt;

        // Assert
        alarm.Status.Should().Be(AlarmStatus.Triggered);
        alarm.TriggeredPrice.Should().Be(triggeredPrice);
        alarm.TriggeredAt.Should().Be(triggeredAt);
    }

    [Theory]
    [InlineData(AlarmStatus.Active)]
    [InlineData(AlarmStatus.Triggered)]
    [InlineData(AlarmStatus.Deleted)]
    public void Alarm_ShouldSupportAllStatusTypes(AlarmStatus status)
    {
        // Arrange
        var alarm = new Alarm();

        // Act
        alarm.Status = status;

        // Assert
        alarm.Status.Should().Be(status);
    }

    [Fact]
    public void Alarm_ShouldSupportNavigationProperties()
    {
        // Arrange
        var alarm = new Alarm();
        var cryptoSymbol = new CryptoSymbol { Id = 1, Code = "BTC", Name = "Bitcoin" };
        var alarmType = new AlarmType { Id = 1, Code = "ABOVE", Name = "Fiyat Üzerine Çıkarsa" };

        // Act
        alarm.CryptoSymbol = cryptoSymbol;
        alarm.AlarmType = alarmType;

        // Assert
        alarm.CryptoSymbol.Should().NotBeNull();
        alarm.CryptoSymbol.Code.Should().Be("BTC");
        alarm.AlarmType.Should().NotBeNull();
        alarm.AlarmType.Code.Should().Be("ABOVE");
    }
}
