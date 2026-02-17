using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Application.Interfaces;
using CryptoAlarmSystem.Application.Workflows;
using CryptoAlarmSystem.Domain.Common;
using CryptoAlarmSystem.Domain.Entities;
using CryptoAlarmSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CryptoAlarmSystem.Application.Services;

public class AlarmService : IAlarmService
{
    private readonly AppDbContext _context;
    private readonly IWorkflow<CreateAlarmRequest> _createAlarmWorkflow;

    public AlarmService(AppDbContext context, IWorkflow<CreateAlarmRequest> createAlarmWorkflow)
    {
        _context = context;
        _createAlarmWorkflow = createAlarmWorkflow;
    }

    public async Task<Result<AlarmResponse>> CreateAlarmAsync(string userId, CreateAlarmRequest request)
    {
        // Business workflow'u çalıştır
        var workflowResult = await _createAlarmWorkflow.ExecuteAsync(request, userId);
        if (!workflowResult.IsSuccess)
        {
            return Result<AlarmResponse>.Failure(workflowResult.ErrorCode, workflowResult.ErrorMessage);
        }

        // Workflow başarılı, alarm'ı oluştur
        var alarm = new Alarm
        {
            UserId = userId,
            CryptoSymbolId = request.CryptoSymbolId,
            AlarmTypeId = (int)request.AlarmTypeId,
            TargetPrice = request.TargetPrice,
            Status = Domain.Enums.AlarmStatus.Active,
            CreatedAt = DateTime.UtcNow,
            AlarmNotificationChannels = request.NotificationChannelIds.Select(channelId => new AlarmNotificationChannel
            {
                NotificationChannelId = (int)channelId
            }).ToList()
        };

        _context.Alarms.Add(alarm);
        await _context.SaveChangesAsync();

        var alarmResponse = await GetAlarmByIdAsync(alarm.Id);
        return Result<AlarmResponse>.Success(alarmResponse);
    }

    public async Task<(List<AlarmResponse> Alarms, int TotalCount)> GetActiveAlarmsAsync(string userId, int pageNumber, int pageSize)
    {
        var totalCount = await _context.Alarms
            .Where(a => a.UserId == userId 
                && a.Status == Domain.Enums.AlarmStatus.Active)
            .CountAsync();
        
        var alarms = await _context.Alarms
            .Include(a => a.CryptoSymbol)
            .Include(a => a.AlarmType)
            .Include(a => a.AlarmNotificationChannels)
                .ThenInclude(anc => anc.NotificationChannel)
            .Where(a => a.UserId == userId 
                && a.Status == Domain.Enums.AlarmStatus.Active)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (alarms.Select(MapToAlarmResponse).ToList(), totalCount);
    }

    public async Task<bool> DeleteAlarmAsync(string userId, int alarmId)
    {
        var alarm = await _context.Alarms
            .FirstOrDefaultAsync(a => a.Id == alarmId 
                && a.UserId == userId 
                && a.Status == Domain.Enums.AlarmStatus.Active);

        if (alarm == null)
            return false;

        alarm.Status = Domain.Enums.AlarmStatus.Deleted;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<AlarmResponse?> UpdateAlarmChannelsAsync(string userId, int alarmId, UpdateAlarmChannelsRequest request)
    {
        var alarm = await _context.Alarms
            .Include(a => a.AlarmNotificationChannels)
            .FirstOrDefaultAsync(a => a.Id == alarmId 
                && a.UserId == userId 
                && a.Status == Domain.Enums.AlarmStatus.Active);

        if (alarm == null)
            return null;

        _context.AlarmNotificationChannels.RemoveRange(alarm.AlarmNotificationChannels);

        var newChannels = request.NotificationChannelIds.Select(channelId => new AlarmNotificationChannel
        {
            AlarmId = alarmId,
            NotificationChannelId = (int)channelId
        }).ToList();

        _context.AlarmNotificationChannels.AddRange(newChannels);
        await _context.SaveChangesAsync();

        return await GetAlarmByIdAsync(alarmId);
    }

    public async Task<(List<AlarmResponse> Alarms, int TotalCount)> GetTriggeredAlarmsAsync(string userId, int pageNumber, int pageSize)
    {
     
        var totalCount = await _context.Alarms
            .Where(a => a.UserId == userId 
                && a.Status == Domain.Enums.AlarmStatus.Triggered)
            .CountAsync();

    
        var alarms = await _context.Alarms
            .Include(a => a.CryptoSymbol)
            .Include(a => a.AlarmType)
            .Include(a => a.AlarmNotificationChannels)
                .ThenInclude(anc => anc.NotificationChannel)
            .Include(a => a.NotificationLogs)
            .Where(a => a.UserId == userId 
                && a.Status == Domain.Enums.AlarmStatus.Triggered)
            .OrderByDescending(a => a.TriggeredAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (alarms.Select(alarm => MapToTriggeredAlarmResponse(alarm)).ToList(), totalCount);
    }

    public async Task<(List<NotificationLogResponse> Logs, int TotalCount)> GetAlarmLogsAsync(string userId, int alarmId, int pageNumber, int pageSize)
    {
        var alarm = await _context.Alarms
            .FirstOrDefaultAsync(a => a.Id == alarmId 
                && a.UserId == userId
                && a.Status == Domain.Enums.AlarmStatus.Triggered);

        if (alarm == null)
            return (new List<NotificationLogResponse>(), 0);

        // Önce toplam kayıt sayısını al
        var totalCount = await _context.NotificationLogs
            .Where(nl => nl.AlarmId == alarmId)
            .CountAsync();

        // Sonra sayfalanmış veriyi al
        var logs = await _context.NotificationLogs
            .Include(nl => nl.CryptoSymbol)
            .Include(nl => nl.AlarmType)
            .Include(nl => nl.NotificationChannel)
            .Where(nl => nl.AlarmId == alarmId)
            .OrderByDescending(nl => nl.SentAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(nl => new NotificationLogResponse(
                nl.Id,
                nl.AlarmId,
                nl.UserId,
                nl.CryptoSymbol.Code,
                nl.AlarmType.Code,
                nl.NotificationChannel.Code,
                nl.TargetPrice,
                nl.TriggeredPrice,
                nl.SentAt
            ))
            .ToListAsync();

        return (logs, totalCount);
    }

    public async Task<List<CryptoSymbolResponse>> GetCryptoSymbolsAsync()
    {
        return await _context.CryptoSymbols
            .Select(cs => new CryptoSymbolResponse(cs.Id, cs.Code, cs.Name))
            .ToListAsync();
    }

    public async Task<List<NotificationChannelDto>> GetNotificationChannelsAsync()
    {
        return await _context.NotificationChannels
            .Select(nc => new NotificationChannelDto(nc.Id, nc.Code, nc.Name, null))
            .ToListAsync();
    }

    public async Task<List<AlarmTypeResponse>> GetAlarmTypesAsync()
    {
        return await _context.AlarmTypes
            .Select(at => new AlarmTypeResponse(at.Id, at.Code, at.Name))
            .ToListAsync();
    }

    private async Task<AlarmResponse> GetAlarmByIdAsync(int alarmId)
    {
        var alarm = await _context.Alarms
            .Include(a => a.CryptoSymbol)
            .Include(a => a.AlarmType)
            .Include(a => a.AlarmNotificationChannels)
                .ThenInclude(anc => anc.NotificationChannel)
            .FirstAsync(a => a.Id == alarmId);

        return MapToAlarmResponse(alarm);
    }

    private static AlarmResponse MapToAlarmResponse(Alarm alarm)
    {
        return new AlarmResponse(
            alarm.Id,
            alarm.UserId,
            alarm.CryptoSymbolId,
            alarm.CryptoSymbol.Code,
            alarm.CryptoSymbol.Name,
            alarm.AlarmTypeId,
            alarm.AlarmType.Code,
            alarm.AlarmType.Name,
            alarm.TargetPrice,
            alarm.Status.ToString(),
            alarm.TriggeredPrice,
            alarm.TriggeredAt,
            alarm.CreatedAt,
            alarm.AlarmNotificationChannels.Select(anc => new NotificationChannelDto(
                anc.NotificationChannel.Id,
                anc.NotificationChannel.Code,
                anc.NotificationChannel.Name,
                null
            )).ToList()
        );
    }

    private static AlarmResponse MapToTriggeredAlarmResponse(Alarm alarm)
    {
        return new AlarmResponse(
            alarm.Id,
            alarm.UserId,
            alarm.CryptoSymbolId,
            alarm.CryptoSymbol.Code,
            alarm.CryptoSymbol.Name,
            alarm.AlarmTypeId,
            alarm.AlarmType.Code,
            alarm.AlarmType.Name,
            alarm.TargetPrice,
            alarm.Status.ToString(),
            alarm.TriggeredPrice,
            alarm.TriggeredAt,
            alarm.CreatedAt,
            alarm.AlarmNotificationChannels.Select(anc => new NotificationChannelDto(
                anc.NotificationChannel.Id,
                anc.NotificationChannel.Code,
                anc.NotificationChannel.Name,
                alarm.NotificationLogs
                    .Where(nl => nl.NotificationChannelId == anc.NotificationChannelId)
                    .OrderByDescending(nl => nl.SentAt)
                    .Select(nl => nl.SentAt)
                    .FirstOrDefault()
            )).ToList()
        );
    }
}
