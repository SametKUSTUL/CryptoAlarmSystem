using CryptoAlarmSystem.Application.Models;
using Microsoft.Extensions.Logging;

namespace CryptoAlarmSystem.Application.Strategies;

public class SmsNotificationStrategy : INotificationStrategy
{
    private readonly ILogger<SmsNotificationStrategy> _logger;

    public SmsNotificationStrategy(ILogger<SmsNotificationStrategy> logger)
    {
        _logger = logger;
    }

    public async Task SendAsync(NotificationMessage message)
    {
        _logger.LogInformation(
            "📱 SMS sent: User={UserId}, Symbol={Symbol}, Type={Type}, Target={Target}, Current={Current}",
            message.UserId, message.CryptoSymbolCode, message.AlarmTypeCode, message.TargetPrice, message.TriggeredPrice);
        
        await Task.CompletedTask;
    }
}
