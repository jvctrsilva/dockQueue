using DockQueue.Domain.Entities;
using DockQueue.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DockQueue.Infra.Data.EntitiesConfiguration
{
    internal class SystemSettingsConfiguration : IEntityTypeConfiguration<SystemSettings>
    {
        public void Configure(EntityTypeBuilder<SystemSettings> builder)
        {
            builder.ToTable("system_settings");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OperatingDays)
                   .HasConversion<int>() // Flags => int
                   .IsRequired();

            builder.Property(x => x.StartTime)
                   .HasColumnType("time")
                   .IsRequired();

            builder.Property(x => x.EndTime)
                   .HasColumnType("time")
                   .IsRequired();

            builder.Property(x => x.TimeZone)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        }
    }
}
