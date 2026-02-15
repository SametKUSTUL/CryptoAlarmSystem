using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Domain.Common;
using CryptoAlarmSystem.Domain.Enums;
using CryptoAlarmSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CryptoAlarmSystem.Application.BusinessRules.CreateAlarm;

public class CryptoSymbolExistsRule : IBusinessRule<CreateAlarmRequest>
{
    private readonly AppDbContext _context;

    public CryptoSymbolExistsRule(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> ValidateAsync(CreateAlarmRequest request, string? userId = null)
    {
        var exists = await _context.CryptoSymbols
            .AnyAsync(cs => cs.Id == request.CryptoSymbolId);

        if (!exists)
        {
            return Result.Failure(
                ErrorCode.CryptoSymbolNotFound,
                $"Crypto symbol with ID {request.CryptoSymbolId} not found."
            );
        }

        return Result.Success();
    }
}
