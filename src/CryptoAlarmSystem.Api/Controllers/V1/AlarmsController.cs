using Asp.Versioning;
using CryptoAlarmSystem.Api.Extensions;
using CryptoAlarmSystem.Api.Filters;
using CryptoAlarmSystem.Api.Models;
using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptoAlarmSystem.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
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
    [ProducesResponseType(typeof(PagedResponse<AlarmResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<AlarmResponse>>> GetActiveAlarms(
        [FromHeader(Name = "X-User-Id")] string? userId,
        [FromQuery] PaginationRequest pagination)
    {
        var allAlarms = await _alarmService.GetActiveAlarmsAsync(userId!);
        var totalRecords = allAlarms.Count;
        var pagedAlarms = allAlarms.ApplyPagination(pagination);
        
        var response = pagedAlarms.ToPagedResponse(pagination.PageNumber, pagination.PageSize, totalRecords);
        return Ok(response);
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
    [ProducesResponseType(typeof(PagedResponse<AlarmResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<AlarmResponse>>> GetTriggeredAlarms(
        [FromHeader(Name = "X-User-Id")] string? userId,
        [FromQuery] PaginationRequest pagination)
    {
        var allAlarms = await _alarmService.GetTriggeredAlarmsAsync(userId!);
        var totalRecords = allAlarms.Count;
        var pagedAlarms = allAlarms.ApplyPagination(pagination);
        
        var response = pagedAlarms.ToPagedResponse(pagination.PageNumber, pagination.PageSize, totalRecords);
        return Ok(response);
    }

    [HttpGet("{id}/logs")]
    [ValidateUserId]
    [ProducesResponseType(typeof(PagedResponse<NotificationLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<NotificationLogResponse>>> GetAlarmLogs(
        [FromHeader(Name = "X-User-Id")] string? userId,
        int id,
        [FromQuery] PaginationRequest pagination)
    {
        var allLogs = await _alarmService.GetAlarmLogsAsync(userId!, id);
        var totalRecords = allLogs.Count;
        var pagedLogs = allLogs.ApplyPagination(pagination);
        
        var response = pagedLogs.ToPagedResponse(pagination.PageNumber, pagination.PageSize, totalRecords);
        return Ok(response);
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
