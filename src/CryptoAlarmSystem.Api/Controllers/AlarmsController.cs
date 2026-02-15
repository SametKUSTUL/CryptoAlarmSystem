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
    [ProducesResponseType(typeof(AlarmResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AlarmResponse>> CreateAlarm(
        [FromHeader(Name = "X-User-Id")] string? userId,
        [FromBody] CreateAlarmRequest request)
    {
        var result = await _alarmService.CreateAlarmAsync(userId!, request);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new 
            { 
                errorCode = result.ErrorCode.ToString(),
                errorMessage = result.ErrorMessage 
            });
        }
        
        return CreatedAtAction(nameof(GetAlarmLogs), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet("active")]
    [ValidateUserId]
    [ProducesResponseType(typeof(List<AlarmResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<AlarmResponse>>> GetActiveAlarms(
        [FromHeader(Name = "X-User-Id")] string? userId)
    {
        var alarms = await _alarmService.GetActiveAlarmsAsync(userId!);
        return Ok(alarms);
    }

    [HttpDelete("{id}")]
    [ValidateUserId]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(typeof(AlarmResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(typeof(List<AlarmResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<AlarmResponse>>> GetTriggeredAlarms(
        [FromHeader(Name = "X-User-Id")] string? userId)
    {
        var alarms = await _alarmService.GetTriggeredAlarmsAsync(userId!);
        return Ok(alarms);
    }

    [HttpGet("{id}/logs")]
    [ValidateUserId]
    [ProducesResponseType(typeof(List<NotificationLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<NotificationLogResponse>>> GetAlarmLogs(
        [FromHeader(Name = "X-User-Id")] string? userId,
        int id)
    {
        var logs = await _alarmService.GetAlarmLogsAsync(userId!, id);
        return Ok(logs);
    }

    [HttpGet("crypto-symbols")]
    [ProducesResponseType(typeof(List<CryptoSymbolResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CryptoSymbolResponse>>> GetCryptoSymbols()
    {
        var symbols = await _alarmService.GetCryptoSymbolsAsync();
        return Ok(symbols);
    }

    [HttpGet("notification-channels")]
    [ProducesResponseType(typeof(List<NotificationChannelDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<NotificationChannelDto>>> GetNotificationChannels()
    {
        var channels = await _alarmService.GetNotificationChannelsAsync();
        return Ok(channels);
    }

    [HttpGet("alarm-types")]
    [ProducesResponseType(typeof(List<AlarmTypeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AlarmTypeResponse>>> GetAlarmTypes()
    {
        var types = await _alarmService.GetAlarmTypesAsync();
        return Ok(types);
    }
}
