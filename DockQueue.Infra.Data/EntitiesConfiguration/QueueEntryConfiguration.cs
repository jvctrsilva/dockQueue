using DockQueue.Domain.Entities;
using DockQueue.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DockQueue.Infra.Data
{
    public class QueueEntryConfiguration : IEntityTypeConfiguration<QueueEntry>
    {
        public void Configure(EntityTypeBuilder<QueueEntry> builder)
        {
            builder.ToTable("queue_entries");

            builder.HasKey(q => q.Id);

            builder.Property(q => q.Id)
                .HasColumnName("id");

            builder.Property(q => q.Type)
                .HasColumnName("type")
                .HasConversion<int>()           // enum -> int
                .IsRequired();

            builder.Property(q => q.DriverId)
                .HasColumnName("driver_id")
                .IsRequired();

            builder.Property(q => q.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(q => q.Position)
                .HasColumnName("position")
                .IsRequired();

            builder.Property(q => q.Priority)
                .HasColumnName("priority");

            builder.Property(q => q.StatusId)
                .HasColumnName("status_id")
                .IsRequired();

            builder.Property(q => q.BoxId)
                .HasColumnName("box_id");

            builder.Property(q => q.LastUpdatedByUserId)
                .HasColumnName("last_updated_by_user_id");

            // Relacionamentos

            builder.HasOne(q => q.Driver)
                .WithMany() // se quiser coleção, depois adiciona ICollection<QueueEntry> em Driver
                .HasForeignKey(q => q.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(q => q.Status)
                .WithMany()
                .HasForeignKey(q => q.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(q => q.Box)
                .WithMany()
                .HasForeignKey(q => q.BoxId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(q => q.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(q => q.LastUpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices úteis
            builder.HasIndex(q => new { q.Type, q.Position })
                .HasDatabaseName("ix_queue_entries_type_position");
        }
    }
}
