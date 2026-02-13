using CryptoAlarmSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoAlarmSystem.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
        
        if (!await context.CryptoSymbols.AnyAsync())
        {
            var symbols = new[]
            {
                new CryptoSymbol { Code = "BTC", Name = "Bitcoin" },
                new CryptoSymbol { Code = "ETH", Name = "Ethereum" },
                new CryptoSymbol { Code = "SOL", Name = "Solana" }
            };
            await context.CryptoSymbols.AddRangeAsync(symbols);
        }
        
        if (!await context.NotificationChannels.AnyAsync())
        {
            var channels = new[]
            {
                new NotificationChannel { Code = "EMAIL", Name = "Email" },
                new NotificationChannel { Code = "SMS", Name = "SMS" },
                new NotificationChannel { Code = "PUSH", Name = "Push Notification" }
            };
            await context.NotificationChannels.AddRangeAsync(channels);
        }
        
        if (!await context.AlarmTypes.AnyAsync())
        {
            var types = new[]
            {
                new AlarmType { Code = "ABOVE", Name = "Fiyat Üzerine Çıkarsa" },
                new AlarmType { Code = "BELOW", Name = "Fiyat Altına Düşerse" }
            };
            await context.AlarmTypes.AddRangeAsync(types);
        }
        
        await context.SaveChangesAsync();
    }
}
