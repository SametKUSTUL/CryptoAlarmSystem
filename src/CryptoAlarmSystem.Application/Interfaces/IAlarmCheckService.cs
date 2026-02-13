namespace CryptoAlarmSystem.Application.Interfaces;

public interface IAlarmCheckService
{
    Task CheckAndTriggerAlarmsAsync(int symbolId, decimal currentPrice);
}
