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

    public async Task<Result> ValidateAsync(CreateAlarmRequest request)
    {
        // Request'ten userId'yi almak için bir context gerekiyor
        // Bu yüzden rule'a userId parametresi eklemeliyiz
        // Şimdilik bu sorunu çözmek için CreateAlarmRequest'i genişletelim
        
        return Result.Success();
    }
}
