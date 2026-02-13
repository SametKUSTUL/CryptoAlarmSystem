using CryptoAlarmSystem.Application.Models;

namespace CryptoAlarmSystem.Application.Strategies;

public interface INotificationStrategy
{
    Task SendAsync(NotificationMessage message);
}
