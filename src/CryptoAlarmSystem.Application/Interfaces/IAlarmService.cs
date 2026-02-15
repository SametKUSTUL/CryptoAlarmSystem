using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Domain.Common;

namespace CryptoAlarmSystem.Application.Interfaces;

public interface IAlarmService
{
    Task<Result<AlarmResponse>> CreateAlarmAsync(string userId, CreateAlarmRequest request);
    Task<List<AlarmResponse>> GetActiveAlarmsAsync(string userId);
    Task<bool> DeleteAlarmAsync(string userId, int alarmId);
    Task<AlarmResponse?> UpdateAlarmChannelsAsync(string userId, int alarmId, UpdateAlarmChannelsRequest request);
    Task<List<AlarmResponse>> GetTriggeredAlarmsAsync(string userId);
    Task<List<NotificationLogResponse>> GetAlarmLogsAsync(string userId, int alarmId);
    Task<List<CryptoSymbolResponse>> GetCryptoSymbolsAsync();
    Task<List<NotificationChannelDto>> GetNotificationChannelsAsync();
    Task<List<AlarmTypeResponse>> GetAlarmTypesAsync();
}
