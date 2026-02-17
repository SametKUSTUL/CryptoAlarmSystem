using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Domain.Common;

namespace CryptoAlarmSystem.Application.Interfaces;

public interface IAlarmService
{
    Task<Result<AlarmResponse>> CreateAlarmAsync(string userId, CreateAlarmRequest request);
    Task<(List<AlarmResponse> Alarms, int TotalCount)> GetActiveAlarmsAsync(string userId, int pageNumber, int pageSize);
    Task<bool> DeleteAlarmAsync(string userId, int alarmId);
    Task<AlarmResponse?> UpdateAlarmChannelsAsync(string userId, int alarmId, UpdateAlarmChannelsRequest request);
    Task<(List<AlarmResponse> Alarms, int TotalCount)> GetTriggeredAlarmsAsync(string userId, int pageNumber, int pageSize);
    Task<(List<NotificationLogResponse> Logs, int TotalCount)> GetAlarmLogsAsync(string userId, int alarmId, int pageNumber, int pageSize);
    Task<List<CryptoSymbolResponse>> GetCryptoSymbolsAsync();
    Task<List<NotificationChannelDto>> GetNotificationChannelsAsync();
    Task<List<AlarmTypeResponse>> GetAlarmTypesAsync();
}
