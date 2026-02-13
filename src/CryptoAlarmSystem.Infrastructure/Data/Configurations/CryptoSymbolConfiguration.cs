using CryptoAlarmSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAlarmSystem.Infrastructure.Data.Configurations;

public class CryptoSymbolConfiguration : IEntityTypeConfiguration<CryptoSymbol>
{
    public void Configure(EntityTypeBuilder<CryptoSymbol> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
