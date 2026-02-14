using CryptoAlarmSystem.Application.Messaging;
using CryptoAlarmSystem.Application.Models;
using CryptoAlarmSystem.Application.Interfaces;
using CryptoAlarmSystem.Domain.Entities;
using CryptoAlarmSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CryptoAlarmSystem.Application.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly NotificationPublisher _publisher;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(AppDbContext context, NotificationPublisher publisher, ILogger<NotificationService> logger)
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task SendNotificationsAsync(Alarm alarm, decimal currentPrice)
    {
        var channels = await _context.AlarmNotificationChannels
            .Include(anc => anc.NotificationChannel)
            .Where(anc => anc.AlarmId == alarm.Id)
            .Select(anc => new NotificationChannelInfo(
                anc.NotificationChannel.Id,
                anc.NotificationChannel.Code,
                anc.NotificationChannel.Name
            ))
            .ToListAsync();

        var message = new NotificationMessage(
            alarm.Id,
            alarm.UserId,
            alarm.CryptoSymbol.Code,
            alarm.CryptoSymbol.Name,
            alarm.CryptoSymbolId,
            alarm.AlarmType.Code,
            alarm.AlarmTypeId,
            alarm.TargetPrice,
            currentPrice,
            channels
        );

        await _publisher.PublishAsync(message);
        _logger.LogInformation("Notification queued for AlarmId={AlarmId}", alarm.Id);
    }
}
