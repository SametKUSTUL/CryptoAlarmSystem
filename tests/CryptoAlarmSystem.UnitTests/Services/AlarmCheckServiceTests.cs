using CryptoAlarmSystem.Application.Interfaces;
using CryptoAlarmSystem.Application.Services;
using CryptoAlarmSystem.Domain.Entities;
using CryptoAlarmSystem.Domain.Enums;
using CryptoAlarmSystem.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CryptoAlarmSystem.UnitTests.Services;

public class AlarmCheckServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<ILogger<AlarmCheckService>> _loggerMock;
    private readonly AlarmCheckService _service;

    public AlarmCheckServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _notificationServiceMock = new Mock<INotificationService>();
        _loggerMock = new Mock<ILogger<AlarmCheckService>>();
        _service = new AlarmCheckService(_context, _notificationServiceMock.Object, _loggerMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var cryptoSymbol = new CryptoSymbol { Id = 1, Code = "BTC", Name = "Bitcoin" };
        var alarmTypeAbove = new AlarmType { Id = 1, Code = "ABOVE", Name = "Fiyat Üzerine Çıkarsa" };
        var alarmTypeBelow = new AlarmType { Id = 2, Code = "BELOW", Name = "Fiyat Altına Düşerse" };

        _context.CryptoSymbols.Add(cryptoSymbol);
        _context.AlarmTypes.AddRange(alarmTypeAbove, alarmTypeBelow);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CheckAndTriggerAlarmsAsync_NoActiveAlarms_ShouldNotTriggerAny()
    {
        // Arrange
        var symbolId = 1;
        var currentPrice = 45000m;

        // Act
        await _service.CheckAndTriggerAlarmsAsync(symbolId, currentPrice);

        // Assert
        _notificationServiceMock.Verify(
            x => x.SendNotificationsAsync(It.IsAny<Alarm>(), It.IsAny<decimal>()),
            Times.Never);
    }

    [Fact]
    public async Task CheckAndTriggerAlarmsAsync_PriceAboveTarget_ShouldTriggerAboveAlarm()
    {
        // Arrange
        var alarm = new Alarm
        {
            UserId = "user123",
            CryptoSymbolId = 1,
            AlarmTypeId = 1,
            TargetPrice = 44000m,
            Status = AlarmStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        _context.Alarms.Add(alarm);
        await _context.SaveChangesAsync();

        var currentPrice = 45000m;

        // Act
        await _service.CheckAndTriggerAlarmsAsync(1, currentPrice);

        // Assert
        var triggeredAlarm = await _context.Alarms.FirstAsync(a => a.Id == alarm.Id);
        triggeredAlarm.Status.Should().Be(AlarmStatus.Triggered);
        triggeredAlarm.TriggeredPrice.Should().Be(currentPrice);
        triggeredAlarm.TriggeredAt.Should().NotBeNull();

        _notificationServiceMock.Verify(
            x => x.SendNotificationsAsync(It.IsAny<Alarm>(), currentPrice),
            Times.Once);
    }

    [Fact]
    public async Task CheckAndTriggerAlarmsAsync_PriceBelowTarget_ShouldTriggerBelowAlarm()
    {
        // Arrange
        var alarm = new Alarm
        {
            UserId = "user123",
            CryptoSymbolId = 1,
            AlarmTypeId = 2,
            TargetPrice = 46000m,
            Status = AlarmStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        _context.Alarms.Add(alarm);
        await _context.SaveChangesAsync();

        var currentPrice = 45000m;

        // Act
        await _service.CheckAndTriggerAlarmsAsync(1, currentPrice);

        // Assert
        var triggeredAlarm = await _context.Alarms.FirstAsync(a => a.Id == alarm.Id);
        triggeredAlarm.Status.Should().Be(AlarmStatus.Triggered);
        triggeredAlarm.TriggeredPrice.Should().Be(currentPrice);
    }

    [Fact]
    public async Task CheckAndTriggerAlarmsAsync_PriceNotMeetingCondition_ShouldNotTrigger()
    {
        // Arrange
        var alarm = new Alarm
        {
            UserId = "user123",
            CryptoSymbolId = 1,
            AlarmTypeId = 1,
            TargetPrice = 50000m, // Hedef 50k
            Status = AlarmStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        _context.Alarms.Add(alarm);
        await _context.SaveChangesAsync();

        var currentPrice = 45000m; // Mevcut 45k (henüz tetiklenmemeli)

        // Act
        await _service.CheckAndTriggerAlarmsAsync(1, currentPrice);

        // Assert
        var unchangedAlarm = await _context.Alarms.FirstAsync(a => a.Id == alarm.Id);
        unchangedAlarm.Status.Should().Be(AlarmStatus.Active);
        unchangedAlarm.TriggeredPrice.Should().BeNull();

        _notificationServiceMock.Verify(
            x => x.SendNotificationsAsync(It.IsAny<Alarm>(), It.IsAny<decimal>()),
            Times.Never);
    }

    [Fact]
    public async Task CheckAndTriggerAlarmsAsync_MultipleAlarms_ShouldTriggerOnlyMatching()
    {
        // Arrange
        var alarm1 = new Alarm
        {
            UserId = "user123",
            CryptoSymbolId = 1,
            AlarmTypeId = 1,
            TargetPrice = 44000m, // Tetiklenmeli
            Status = AlarmStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var alarm2 = new Alarm
        {
            UserId = "user456",
            CryptoSymbolId = 1,
            AlarmTypeId = 1,
            TargetPrice = 50000m, // Tetiklenmemeli
            Status = AlarmStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _context.Alarms.AddRange(alarm1, alarm2);
        await _context.SaveChangesAsync();

        var currentPrice = 45000m;

        // Act
        await _service.CheckAndTriggerAlarmsAsync(1, currentPrice);

        // Assert
        var triggeredAlarm = await _context.Alarms.FirstAsync(a => a.Id == alarm1.Id);
        triggeredAlarm.Status.Should().Be(AlarmStatus.Triggered);

        var activeAlarm = await _context.Alarms.FirstAsync(a => a.Id == alarm2.Id);
        activeAlarm.Status.Should().Be(AlarmStatus.Active);

        _notificationServiceMock.Verify(
            x => x.SendNotificationsAsync(It.IsAny<Alarm>(), currentPrice),
            Times.Once);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
