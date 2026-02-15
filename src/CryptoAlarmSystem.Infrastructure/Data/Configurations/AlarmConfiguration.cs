using CryptoAlarmSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAlarmSystem.Infrastructure.Data.Configurations;

public class AlarmConfiguration : IEntityTypeConfiguration<Alarm>
{
    public void Configure(EntityTypeBuilder<Alarm> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.TargetPrice).HasPrecision(18, 8);
        builder.Property(x => x.TriggeredPrice).HasPrecision(18, 8);
        builder.Property(x => x.Status).IsRequired();

        // DateTime alanlarını timestamp without time zone olarak ayarla
        builder.Property(x => x.CreatedAt)
            .HasColumnType("timestamp without time zone");
        
        builder.Property(x => x.TriggeredAt)
            .HasColumnType("timestamp without time zone");

        builder.HasOne(x => x.CryptoSymbol)
            .WithMany(x => x.Alarms)
            .HasForeignKey(x => x.CryptoSymbolId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AlarmType)
            .WithMany(x => x.Alarms)
            .HasForeignKey(x => x.AlarmTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.Status });
    }
}
