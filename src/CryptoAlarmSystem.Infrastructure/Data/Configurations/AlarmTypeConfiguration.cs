using CryptoAlarmSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAlarmSystem.Infrastructure.Data.Configurations;

public class AlarmTypeConfiguration : IEntityTypeConfiguration<AlarmType>
{
    public void Configure(EntityTypeBuilder<AlarmType> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
