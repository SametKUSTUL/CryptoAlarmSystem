using CryptoAlarmSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoAlarmSystem.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<CryptoSymbol> CryptoSymbols { get; set; }
    public DbSet<NotificationChannel> NotificationChannels { get; set; }
    public DbSet<AlarmType> AlarmTypes { get; set; }
    public DbSet<Alarm> Alarms { get; set; }
    public DbSet<AlarmNotificationChannel> AlarmNotificationChannels { get; set; }
    public DbSet<NotificationLog> NotificationLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
