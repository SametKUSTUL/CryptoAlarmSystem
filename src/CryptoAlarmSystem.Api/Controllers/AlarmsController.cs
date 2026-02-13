using CryptoAlarmSystem.Api.Filters;
using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptoAlarmSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlarmsController : ControllerBase
{
    private readonly IAlarmService _alarmService;

    public AlarmsController(IAlarmService alarmService)
    {
        _alarmService = alarmService;
    }

    [HttpPost]
    [ValidateUserId]
    public async Task<ActionResult<AlarmResponse>> CreateAlarm(
        [FromHeader(Name = "X-User-Id")] string? userId,
        [FromBody] CreateAlarmRequest request)
    {
        var alarm = await _alarmService.CreateAlarmAsync(userId!, request);
        return CreatedAtAction(nameof(GetAlarmLogs), new { id = alarm.Id }, alarm);
    }

    [HttpGet("active")]
    [ValidateUserId]
    public async Task<ActionResult<List<AlarmResponse>>> GetActiveAlarms(
        [FromHeader(Name = "X-User-Id")] string? userId)
    {
        var alarms = await _alarmService.GetActiveAlarmsAsync(userId!);
        return Ok(alarms);
    }

    [HttpDelete("{id}")]
    [ValidateUserId]
    public async Task<IActionResult> DeleteAlarm(
        [FromHeader(Name = "X-User-Id")] string? userId,
        int id)
    {
        var deleted = await _alarmService.DeleteAlarmAsync(userId!, id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPatch("{id}/channels")]
    [ValidateUserId]
    public async Task<ActionResult<AlarmResponse>> UpdateAlarmChannels(
        [FromHeader(Name = "X-User-Id")] string? userId,
        int id,
        [FromBody] UpdateAlarmChannelsRequest request)
    {
        var alarm = await _alarmService.UpdateAlarmChannelsAsync(userId!, id, request);
        if (alarm == null)
            return NotFound();

        return Ok(alarm);
    }

    [HttpGet("triggered")]
    [ValidateUserId]
    public async Task<ActionResult<List<AlarmResponse>>> GetTriggeredAlarms(
        [FromHeader(Name = "X-User-Id")] string? userId)
    {
        var alarms = await _alarmService.GetTriggeredAlarmsAsync(userId!);
        return Ok(alarms);
    }

    [HttpGet("{id}/logs")]
    [ValidateUserId]
    public async Task<ActionResult<List<NotificationLogResponse>>> GetAlarmLogs(
        [FromHeader(Name = "X-User-Id")] string? userId,
        int id)
    {
        var logs = await _alarmService.GetAlarmLogsAsync(userId!, id);
        return Ok(logs);
    }

    [HttpGet("crypto-symbols")]
    public async Task<ActionResult<List<CryptoSymbolResponse>>> GetCryptoSymbols()
    {
        var symbols = await _alarmService.GetCryptoSymbolsAsync();
        return Ok(symbols);
    }

    [HttpGet("notification-channels")]
    public async Task<ActionResult<List<NotificationChannelDto>>> GetNotificationChannels()
    {
        var channels = await _alarmService.GetNotificationChannelsAsync();
        return Ok(channels);
    }

    [HttpGet("alarm-types")]
    public async Task<ActionResult<List<AlarmTypeResponse>>> GetAlarmTypes()
    {
        var types = await _alarmService.GetAlarmTypesAsync();
        return Ok(types);
    }
}
