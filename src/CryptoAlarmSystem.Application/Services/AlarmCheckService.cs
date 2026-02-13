using CryptoAlarmSystem.Application.Interfaces;
using CryptoAlarmSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CryptoAlarmSystem.Application.Services;

public class AlarmCheckService : IAlarmCheckService
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly ILogger<AlarmCheckService> _logger;

    public AlarmCheckService(
        AppDbContext context, 
        INotificationService notificationService, 
        ILogger<AlarmCheckService> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task CheckAndTriggerAlarmsAsync(int symbolId, decimal currentPrice)
    {
        var activeAlarms = await _context.Alarms
            .Include(a => a.CryptoSymbol)
            .Include(a => a.AlarmType)
            .Where(a => a.CryptoSymbolId == symbolId && !a.IsTriggered)
            .ToListAsync();

        foreach (var alarm in activeAlarms)
        {
            if (ShouldTriggerAlarm(alarm.AlarmType.Code, currentPrice, alarm.TargetPrice))
            {
                await TriggerAlarmAsync(alarm, currentPrice);
            }
        }

        await _context.SaveChangesAsync();
    }

    private bool ShouldTriggerAlarm(string alarmTypeCode, decimal currentPrice, decimal targetPrice)
    {
        return alarmTypeCode switch
        {
            "ABOVE" => currentPrice >= targetPrice,
            "BELOW" => currentPrice <= targetPrice,
            _ => false
        };
    }

    private async Task TriggerAlarmAsync(Domain.Entities.Alarm alarm, decimal currentPrice)
    {
        alarm.IsTriggered = true;
        alarm.TriggeredPrice = currentPrice;
        alarm.TriggeredAt = DateTime.UtcNow;

        await _notificationService.SendNotificationsAsync(alarm, currentPrice);

        _logger.LogWarning(
            "🔔 Alarm triggered! User={UserId}, Symbol={Symbol}, Type={Type}, Target={Target}, Current={Current}",
            alarm.UserId, alarm.CryptoSymbol.Code, alarm.AlarmType.Code, alarm.TargetPrice, currentPrice);
    }
}
