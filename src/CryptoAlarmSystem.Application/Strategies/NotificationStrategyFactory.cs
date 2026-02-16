using CryptoAlarmSystem.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoAlarmSystem.Application.Strategies;

public class NotificationStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public INotificationStrategy GetStrategy(int channelCode)
    {
        if (!Enum.IsDefined(typeof(NotificationChannels), channelCode))
        {
            throw new ArgumentException($"Invalid channel code: {channelCode}", nameof(channelCode));
        }
        return (NotificationChannels)channelCode switch
        {
            NotificationChannels.Email => _serviceProvider.GetRequiredService<EmailNotificationStrategy>(),
            NotificationChannels.Sms=> _serviceProvider.GetRequiredService<SmsNotificationStrategy>(),
            NotificationChannels.PushNotification => _serviceProvider.GetRequiredService<PushNotificationStrategy>(),
            _ => throw new NotSupportedException($"Channel {channelCode} not supported")
        };
    }
}
