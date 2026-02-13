using CryptoAlarmSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAlarmSystem.Infrastructure.Data.Configurations;

public class AlarmNotificationChannelConfiguration : IEntityTypeConfiguration<AlarmNotificationChannel>
{
    public void Configure(EntityTypeBuilder<AlarmNotificationChannel> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Alarm)
            .WithMany(x => x.AlarmNotificationChannels)
            .HasForeignKey(x => x.AlarmId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.NotificationChannel)
            .WithMany(x => x.AlarmNotificationChannels)
            .HasForeignKey(x => x.NotificationChannelId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => new { x.AlarmId, x.NotificationChannelId }).IsUnique();
    }
}
