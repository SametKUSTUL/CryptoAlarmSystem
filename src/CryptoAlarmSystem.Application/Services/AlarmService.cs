using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Application.Interfaces;
using CryptoAlarmSystem.Domain.Entities;
using CryptoAlarmSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CryptoAlarmSystem.Application.Services;

public class AlarmService : IAlarmService
{
    private readonly AppDbContext _context;

    public AlarmService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AlarmResponse> CreateAlarmAsync(string userId, CreateAlarmRequest request)
    {
        var alarm = new Alarm
        {
            UserId = userId,
            CryptoSymbolId = request.CryptoSymbolId,
            AlarmTypeId = request.AlarmTypeId,
            TargetPrice = request.TargetPrice,
            IsTriggered = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Alarms.Add(alarm);
        await _context.SaveChangesAsync();

        var alarmChannels = request.NotificationChannelIds.Select(channelId => new AlarmNotificationChannel
        {
            AlarmId = alarm.Id,
            NotificationChannelId = channelId
        }).ToList();

        _context.AlarmNotificationChannels.AddRange(alarmChannels);
        await _context.SaveChangesAsync();

        return await GetAlarmByIdAsync(alarm.Id);
    }

    public async Task<List<AlarmResponse>> GetActiveAlarmsAsync(string userId)
    {
        var alarms = await _context.Alarms
            .Include(a => a.CryptoSymbol)
            .Include(a => a.AlarmType)
            .Include(a => a.AlarmNotificationChannels)
                .ThenInclude(anc => anc.NotificationChannel)
            .Where(a => a.UserId == userId && !a.IsTriggered)
            .ToListAsync();

        return alarms.Select(MapToAlarmResponse).ToList();
    }

    public async Task<bool> DeleteAlarmAsync(string userId, int alarmId)
    {
        var alarm = await _context.Alarms
            .FirstOrDefaultAsync(a => a.Id == alarmId && a.UserId == userId && !a.IsTriggered);

        if (alarm == null)
            return false;

        _context.Alarms.Remove(alarm);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<AlarmResponse?> UpdateAlarmChannelsAsync(string userId, int alarmId, UpdateAlarmChannelsRequest request)
    {
        var alarm = await _context.Alarms
            .Include(a => a.AlarmNotificationChannels)
            .FirstOrDefaultAsync(a => a.Id == alarmId && a.UserId == userId && !a.IsTriggered);

        if (alarm == null)
            return null;

        _context.AlarmNotificationChannels.RemoveRange(alarm.AlarmNotificationChannels);

        var newChannels = request.NotificationChannelIds.Select(channelId => new AlarmNotificationChannel
        {
            AlarmId = alarmId,
            NotificationChannelId = channelId
        }).ToList();

        _context.AlarmNotificationChannels.AddRange(newChannels);
        await _context.SaveChangesAsync();

        return await GetAlarmByIdAsync(alarmId);
    }

    public async Task<List<AlarmResponse>> GetTriggeredAlarmsAsync(string userId)
    {
        var alarms = await _context.Alarms
            .Include(a => a.CryptoSymbol)
            .Include(a => a.AlarmType)
            .Include(a => a.AlarmNotificationChannels)
                .ThenInclude(anc => anc.NotificationChannel)
            .Where(a => a.UserId == userId && a.IsTriggered)
            .ToListAsync();

        return alarms.Select(MapToAlarmResponse).ToList();
    }

    public async Task<List<NotificationLogResponse>> GetAlarmLogsAsync(string userId, int alarmId)
    {
        var alarm = await _context.Alarms
            .FirstOrDefaultAsync(a => a.Id == alarmId && a.UserId == userId);

        if (alarm == null)
            return new List<NotificationLogResponse>();

        return await _context.NotificationLogs
            .Include(nl => nl.CryptoSymbol)
            .Include(nl => nl.AlarmType)
            .Include(nl => nl.NotificationChannel)
            .Where(nl => nl.AlarmId == alarmId)
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
            .Select(nc => new NotificationChannelDto(nc.Id, nc.Code, nc.Name))
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
            alarm.IsTriggered,
            alarm.TriggeredPrice,
            alarm.TriggeredAt,
            alarm.CreatedAt,
            alarm.AlarmNotificationChannels.Select(anc => new NotificationChannelDto(
                anc.NotificationChannel.Id,
                anc.NotificationChannel.Code,
                anc.NotificationChannel.Name
            )).ToList()
        );
    }
}
