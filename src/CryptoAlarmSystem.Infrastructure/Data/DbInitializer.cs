using CryptoAlarmSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoAlarmSystem.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
        
        // PostgreSQL timezone'u Türkiye saatine ayarla
        if (context.Database.IsNpgsql())
        {
            await context.Database.ExecuteSqlRawAsync("SET TIME ZONE 'Europe/Istanbul';");
        }
        
        var existingSymbols = await context.CryptoSymbols.Select(s => s.Code).ToListAsync();
        var newSymbols = new[]
        {
            new CryptoSymbol { Code = "BTC", Name = "Bitcoin" },
            new CryptoSymbol { Code = "ETH", Name = "Ethereum" },
            new CryptoSymbol { Code = "SOL", Name = "Solana" },
            new CryptoSymbol { Code = "DOGE", Name = "Dogecoin" },
            new CryptoSymbol { Code = "LTC", Name = "Litecoin" },
            new CryptoSymbol { Code = "XRP", Name = "Ripple" },
            new CryptoSymbol { Code = "BNB", Name = "Binance Coin" },
            new CryptoSymbol { Code = "USDT", Name = "Tether" },
            new CryptoSymbol { Code = "ADA", Name = "Cardano" }
        };
        
        var symbolsToAdd = newSymbols.Where(s => !existingSymbols.Contains(s.Code)).ToArray();
        if (symbolsToAdd.Any())
        {
            await context.CryptoSymbols.AddRangeAsync(symbolsToAdd);
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
