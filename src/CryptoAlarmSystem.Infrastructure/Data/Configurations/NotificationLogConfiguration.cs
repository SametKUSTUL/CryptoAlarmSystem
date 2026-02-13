using CryptoAlarmSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAlarmSystem.Infrastructure.Data.Configurations;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.TargetPrice).HasPrecision(18, 8);
        builder.Property(x => x.TriggeredPrice).HasPrecision(18, 8);
        
        builder.HasOne(x => x.Alarm)
            .WithMany(x => x.NotificationLogs)
            .HasForeignKey(x => x.AlarmId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.NotificationChannel)
            .WithMany(x => x.NotificationLogs)
            .HasForeignKey(x => x.NotificationChannelId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.CryptoSymbol)
            .WithMany(x => x.NotificationLogs)
            .HasForeignKey(x => x.CryptoSymbolId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.AlarmType)
            .WithMany(x => x.NotificationLogs)
            .HasForeignKey(x => x.AlarmTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.AlarmId);
        builder.HasIndex(x => x.SentAt);
    }
}
