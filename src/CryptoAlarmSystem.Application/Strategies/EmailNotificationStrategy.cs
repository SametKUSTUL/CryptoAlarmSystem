using CryptoAlarmSystem.Application.Models;
using Microsoft.Extensions.Logging;

namespace CryptoAlarmSystem.Application.Strategies;

public class EmailNotificationStrategy : INotificationStrategy
{
    private readonly ILogger<EmailNotificationStrategy> _logger;

    public EmailNotificationStrategy(ILogger<EmailNotificationStrategy> logger)
    {
        _logger = logger;
    }

    public async Task SendAsync(NotificationMessage message)
    {
        _logger.LogInformation(
            "📧 EMAIL sent: User={UserId}, Symbol={Symbol}, Type={Type}, Target={Target}, Current={Current}",
            message.UserId, message.CryptoSymbolCode, message.AlarmTypeCode, message.TargetPrice, message.TriggeredPrice);
        
        await Task.CompletedTask;
    }
}
