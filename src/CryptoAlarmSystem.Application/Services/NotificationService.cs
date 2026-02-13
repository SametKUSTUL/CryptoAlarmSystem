using CryptoAlarmSystem.Application.Interfaces;
using CryptoAlarmSystem.Domain.Entities;
using CryptoAlarmSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CryptoAlarmSystem.Application.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(AppDbContext context, ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SendNotificationsAsync(Alarm alarm, decimal currentPrice)
    {
        var channels = await _context.AlarmNotificationChannels
            .Include(anc => anc.NotificationChannel)
            .Where(anc => anc.AlarmId == alarm.Id)
            .ToListAsync();

        foreach (var channel in channels)
        {
            await SendMockNotificationAsync(alarm, channel.NotificationChannel, currentPrice);
            await LogNotificationAsync(alarm, channel.NotificationChannel, currentPrice);
        }
    }

    private async Task SendMockNotificationAsync(Alarm alarm, NotificationChannel channel, decimal currentPrice)
    {
        // Mock HTTP request - sadece console'a log
        _logger.LogInformation(
            "📧 Notification sent via {Channel}: User={UserId}, Symbol={Symbol}, Type={Type}, Target={Target}, Current={Current}",
            channel.Code, alarm.UserId, alarm.CryptoSymbol.Code, alarm.AlarmType.Code, alarm.TargetPrice, currentPrice);
        
        await Task.CompletedTask;
    }

    private async Task LogNotificationAsync(Alarm alarm, NotificationChannel channel, decimal currentPrice)
    {
        var log = new NotificationLog
        {
            AlarmId = alarm.Id,
            NotificationChannelId = channel.Id,
            UserId = alarm.UserId,
            CryptoSymbolId = alarm.CryptoSymbolId,
            AlarmTypeId = alarm.AlarmTypeId,
            TargetPrice = alarm.TargetPrice,
            TriggeredPrice = currentPrice,
            SentAt = DateTime.UtcNow
        };

        _context.NotificationLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
