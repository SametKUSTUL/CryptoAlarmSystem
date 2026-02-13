using CryptoAlarmSystem.Application.Interfaces;
using CryptoAlarmSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CryptoAlarmSystem.Application.Services;

public class PriceMonitorBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PriceMonitorBackgroundService> _logger;
    private readonly Dictionary<string, decimal> _currentPrices = new();
    private readonly Dictionary<string, (decimal Min, decimal Max)> _priceRanges = new()
    {
        { "BTC", (40000m, 50000m) },
        { "ETH", (2000m, 3000m) },
        { "SOL", (80m, 120m) }
    };

    public PriceMonitorBackgroundService(IServiceProvider serviceProvider, ILogger<PriceMonitorBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Price Monitor Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await GeneratePricesAndCheckAlarmsAsync();
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Price Monitor Background Service");
            }
        }
    }

    private async Task GeneratePricesAndCheckAlarmsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var symbols = await context.CryptoSymbols.ToListAsync();

        foreach (var symbol in symbols)
        {
            var newPrice = GenerateRandomPrice(symbol.Code);
            _currentPrices[symbol.Code] = newPrice;

            _logger.LogInformation("💰 {Symbol} price: {Price:F2}", symbol.Code, newPrice);

            await CheckAndTriggerAlarmsAsync(context, notificationService, symbol.Id, newPrice);
        }
    }

    private decimal GenerateRandomPrice(string symbolCode)
    {
        if (!_priceRanges.TryGetValue(symbolCode, out var range))
            return 0;

        var random = new Random();
        var price = (decimal)(random.NextDouble() * (double)(range.Max - range.Min) + (double)range.Min);
        return Math.Round(price, 2);
    }

    private async Task CheckAndTriggerAlarmsAsync(AppDbContext context, INotificationService notificationService, int symbolId, decimal currentPrice)
    {
        var activeAlarms = await context.Alarms
            .Include(a => a.CryptoSymbol)
            .Include(a => a.AlarmType)
            .Where(a => a.CryptoSymbolId == symbolId && !a.IsTriggered)
            .ToListAsync();

        foreach (var alarm in activeAlarms)
        {
            var shouldTrigger = alarm.AlarmType.Code switch
            {
                "ABOVE" => currentPrice >= alarm.TargetPrice,
                "BELOW" => currentPrice <= alarm.TargetPrice,
                _ => false
            };

            if (shouldTrigger)
            {
                alarm.IsTriggered = true;
                alarm.TriggeredPrice = currentPrice;
                alarm.TriggeredAt = DateTime.UtcNow;

                await notificationService.SendNotificationsAsync(alarm, currentPrice);

                _logger.LogWarning(
                    "🔔 Alarm triggered! User={UserId}, Symbol={Symbol}, Type={Type}, Target={Target}, Current={Current}",
                    alarm.UserId, alarm.CryptoSymbol.Code, alarm.AlarmType.Code, alarm.TargetPrice, currentPrice);
            }
        }

        await context.SaveChangesAsync();
    }
}
