using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Domain.Common;
using CryptoAlarmSystem.Domain.Enums;
using CryptoAlarmSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CryptoAlarmSystem.Application.BusinessRules.CreateAlarm;

public class NoDuplicateActiveAlarmRule : IBusinessRule<CreateAlarmRequest>
{
    private readonly AppDbContext _context;

    public NoDuplicateActiveAlarmRule(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> ValidateAsync(CreateAlarmRequest request, string? userId = null)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return Result.Failure(
                ErrorCode.UserIdCannotBeNullOrEmpty,
                "User Id Cannot Be Null Or Empty.");
        }

        var duplicateExists = await _context.Alarms.AnyAsync(a =>
            a.UserId == userId
            && a.CryptoSymbolId == request.CryptoSymbolId
            && a.AlarmTypeId == (int)request.AlarmTypeId
            && a.TargetPrice == request.TargetPrice
            && a.Status == AlarmStatus.Active);

        if (duplicateExists)
        {
            return Result.Failure(
                ErrorCode.DuplicateAlarm,
                "You already have an active alarm for this crypto symbol, alarm type, and target price.");
        }

        return Result.Success();
    }
}
