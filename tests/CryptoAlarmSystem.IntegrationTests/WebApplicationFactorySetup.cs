using CryptoAlarmSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CryptoAlarmSystem.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly string DatabaseName = "TestDatabase_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            // Add InMemory database for testing - use same database name for all tests
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(DatabaseName);
            });
        });

        builder.UseEnvironment("Testing");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                using var scope = Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureDeleted();
            }
            catch (ObjectDisposedException)
            {
                // Ignore if already disposed
            }
        }
        base.Dispose(disposing);
    }

    public void SeedDatabase()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        context.Database.EnsureCreated();
        
        // Clear all data
        context.Alarms.RemoveRange(context.Alarms);
        context.NotificationLogs.RemoveRange(context.NotificationLogs);
        context.CryptoSymbols.RemoveRange(context.CryptoSymbols);
        context.AlarmTypes.RemoveRange(context.AlarmTypes);
        context.NotificationChannels.RemoveRange(context.NotificationChannels);
        context.SaveChanges();

        // Seed CryptoSymbols
        context.CryptoSymbols.AddRange(
            new Domain.Entities.CryptoSymbol { Id = 1, Code = "BTC", Name = "Bitcoin" },
            new Domain.Entities.CryptoSymbol { Id = 2, Code = "ETH", Name = "Ethereum" },
            new Domain.Entities.CryptoSymbol { Id = 3, Code = "SOL", Name = "Solana" }
        );

        // Seed AlarmTypes
        context.AlarmTypes.AddRange(
            new Domain.Entities.AlarmType { Id = 1, Code = "ABOVE", Name = "Fiyat Üzerine Çıkarsa" },
            new Domain.Entities.AlarmType { Id = 2, Code = "BELOW", Name = "Fiyat Altına Düşerse" }
        );

        // Seed NotificationChannels
        context.NotificationChannels.AddRange(
            new Domain.Entities.NotificationChannel { Id = 1, Code = "EMAIL", Name = "Email" },
            new Domain.Entities.NotificationChannel { Id = 2, Code = "SMS", Name = "SMS" },
            new Domain.Entities.NotificationChannel { Id = 3, Code = "PUSH", Name = "Push Notification" }
        );

        context.SaveChanges();
    }
}
