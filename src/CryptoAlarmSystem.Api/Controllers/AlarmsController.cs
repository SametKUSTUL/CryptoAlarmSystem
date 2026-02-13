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
    public async Task<ActionResult<AlarmResponse>> CreateAlarm(
        [FromHeader(Name = "X-User-Id")] string userId,
        [FromBody] CreateAlarmRequest request)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest("X-User-Id header is required");

        var alarm = await _alarmService.CreateAlarmAsync(userId, request);
        return CreatedAtAction(nameof(GetAlarmLogs), new { id = alarm.Id }, alarm);
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<AlarmResponse>>> GetActiveAlarms(
        [FromHeader(Name = "X-User-Id")] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest("X-User-Id header is required");

        var alarms = await _alarmService.GetActiveAlarmsAsync(userId);
        return Ok(alarms);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAlarm(
        [FromHeader(Name = "X-User-Id")] string userId,
        int id)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest("X-User-Id header is required");

        var deleted = await _alarmService.DeleteAlarmAsync(userId, id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPatch("{id}/channels")]
    public async Task<ActionResult<AlarmResponse>> UpdateAlarmChannels(
        [FromHeader(Name = "X-User-Id")] string userId,
        int id,
        [FromBody] UpdateAlarmChannelsRequest request)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest("X-User-Id header is required");

        var alarm = await _alarmService.UpdateAlarmChannelsAsync(userId, id, request);
        if (alarm == null)
            return NotFound();

        return Ok(alarm);
    }

    [HttpGet("triggered")]
    public async Task<ActionResult<List<AlarmResponse>>> GetTriggeredAlarms(
        [FromHeader(Name = "X-User-Id")] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest("X-User-Id header is required");

        var alarms = await _alarmService.GetTriggeredAlarmsAsync(userId);
        return Ok(alarms);
    }

    [HttpGet("{id}/logs")]
    public async Task<ActionResult<List<NotificationLogResponse>>> GetAlarmLogs(
        [FromHeader(Name = "X-User-Id")] string userId,
        int id)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest("X-User-Id header is required");

        var logs = await _alarmService.GetAlarmLogsAsync(userId, id);
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
