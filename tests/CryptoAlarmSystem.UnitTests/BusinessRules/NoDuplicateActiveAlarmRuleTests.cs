using CryptoAlarmSystem.Application.BusinessRules.CreateAlarm;
using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Domain.Common;
using CryptoAlarmSystem.Domain.Entities;
using CryptoAlarmSystem.Domain.Enums;
using CryptoAlarmSystem.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CryptoAlarmSystem.UnitTests.BusinessRules;

public class NoDuplicateActiveAlarmRuleTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly NoDuplicateActiveAlarmRule _rule;

    public NoDuplicateActiveAlarmRuleTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _rule = new NoDuplicateActiveAlarmRule(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.Alarms.Add(new Alarm
        {
            Id = 1,
            UserId = "user123",
            CryptoSymbolId = 1,
            AlarmTypeId = 1,
            TargetPrice = 45000m,
            Status = AlarmStatus.Active,
            CreatedAt = DateTime.UtcNow
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task ValidateAsync_NoDuplicateAlarm_ShouldReturnSuccess()
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: 1,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: 50000m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );

        var result = await _rule.ValidateAsync(request, "user123");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_DuplicateActiveAlarm_ShouldReturnFailure()
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: 1,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: 45000m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );

        var result = await _rule.ValidateAsync(request, "user123");

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCode.DuplicateAlarm);
        result.ErrorMessage.Should().Contain("already have an active alarm");
    }

    [Fact]
    public async Task ValidateAsync_NullUserId_ShouldReturnFailure()
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: 1,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: 45000m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );

        var result = await _rule.ValidateAsync(request, null);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCode.UserIdCannotBeNullOrEmpty);
    }

    [Fact]
    public async Task ValidateAsync_DifferentUser_ShouldReturnSuccess()
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: 1,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: 45000m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );

        var result = await _rule.ValidateAsync(request, "user456");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_DifferentAlarmType_ShouldReturnSuccess()
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: 1,
            AlarmTypeId: AlarmTypes.Below,
            TargetPrice: 45000m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );

        var result = await _rule.ValidateAsync(request, "user123");

        result.IsSuccess.Should().BeTrue();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
