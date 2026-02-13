using CryptoAlarmSystem.Application.Models;
using Microsoft.Extensions.Logging;

namespace CryptoAlarmSystem.Application.Strategies;

public class PushNotificationStrategy : INotificationStrategy
{
    private readonly ILogger<PushNotificationStrategy> _logger;

    public PushNotificationStrategy(ILogger<PushNotificationStrategy> logger)
    {
        _logger = logger;
    }

    public async Task SendAsync(NotificationMessage message)
    {
        _logger.LogInformation(
            "🔔 PUSH sent: User={UserId}, Symbol={Symbol}, Type={Type}, Target={Target}, Current={Current}",
            message.UserId, message.CryptoSymbolCode, message.AlarmTypeCode, message.TargetPrice, message.TriggeredPrice);
        
        await Task.CompletedTask;
    }
}
