using CryptoAlarmSystem.Domain.Entities;

namespace CryptoAlarmSystem.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationsAsync(Alarm alarm, decimal currentPrice);
}
