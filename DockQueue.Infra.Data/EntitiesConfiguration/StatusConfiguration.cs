using DockQueue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DockQueue.Infra.Data.EntitiesConfiguration
{
    internal class StatusConfiguration : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.ToTable("statuses");
            builder.HasKey(s => s.Id);

            builder.HasIndex(s => s.Code).IsUnique();
            builder.Property(s => s.Code).IsRequired().HasMaxLength(50);

            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Description).HasMaxLength(255);
            builder.Property(s => s.DisplayOrder).IsRequired();
            builder.Property(s => s.IsDefault).IsRequired();
            builder.Property(s => s.IsTerminal).IsRequired();
            builder.Property(s => s.Active).IsRequired();
            builder.Property(s => s.CreatedAt).IsRequired();
        }
    }
}
