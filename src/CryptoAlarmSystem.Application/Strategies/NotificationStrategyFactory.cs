using Microsoft.Extensions.DependencyInjection;

namespace CryptoAlarmSystem.Application.Strategies;

public class NotificationStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public INotificationStrategy GetStrategy(string channelCode)
    {
        return channelCode switch
        {
            "EMAIL" => _serviceProvider.GetRequiredService<EmailNotificationStrategy>(),
            "SMS" => _serviceProvider.GetRequiredService<SmsNotificationStrategy>(),
            "PUSH" => _serviceProvider.GetRequiredService<PushNotificationStrategy>(),
            _ => throw new NotSupportedException($"Channel {channelCode} not supported")
        };
    }
}
