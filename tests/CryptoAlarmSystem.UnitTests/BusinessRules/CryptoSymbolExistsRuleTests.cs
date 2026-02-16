using CryptoAlarmSystem.Application.BusinessRules.CreateAlarm;
using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Domain.Entities;
using CryptoAlarmSystem.Domain.Enums;
using CryptoAlarmSystem.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CryptoAlarmSystem.UnitTests.BusinessRules;

public class CryptoSymbolExistsRuleTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CryptoSymbolExistsRule _rule;

    public CryptoSymbolExistsRuleTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _rule = new CryptoSymbolExistsRule(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.CryptoSymbols.AddRange(
            new CryptoSymbol { Id = 1, Code = "BTC", Name = "Bitcoin" },
            new CryptoSymbol { Id = 2, Code = "ETH", Name = "Ethereum" },
            new CryptoSymbol { Id = 3, Code = "SOL", Name = "Solana" }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task ValidateAsync_ExistingSymbol_ShouldReturnSuccess()
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: 1,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: 45000m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_NonExistingSymbol_ShouldReturnFailure()
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: 999,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: 45000m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(CryptoAlarmSystem.Domain.Enums.ErrorCode.CryptoSymbolNotFound);
        result.ErrorMessage.Should().Contain("Crypto symbol with ID 999 not found");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task ValidateAsync_MultipleExistingSymbols_ShouldReturnSuccess(int symbolId)
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: symbolId,
            AlarmTypeId: AlarmTypes.Below,
            TargetPrice: 1000m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Sms }
        );

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeTrue();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
