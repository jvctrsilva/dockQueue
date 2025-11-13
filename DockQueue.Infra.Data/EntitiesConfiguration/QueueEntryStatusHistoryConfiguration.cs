using DockQueue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DockQueue.Infra.Data.Mappings
{
    public class QueueEntryStatusHistoryConfiguration : IEntityTypeConfiguration<QueueEntryStatusHistory>
    {
        public void Configure(EntityTypeBuilder<QueueEntryStatusHistory> builder)
        {
            builder.ToTable("queue_entry_status_history");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id)
                .HasColumnName("id");

            builder.Property(h => h.QueueEntryId)
                .HasColumnName("queue_entry_id")
                .IsRequired();

            builder.Property(h => h.OldStatusId)
                .HasColumnName("old_status_id")
                .IsRequired();

            builder.Property(h => h.NewStatusId)
                .HasColumnName("new_status_id")
                .IsRequired();

            builder.Property(h => h.ChangedAt)
                .HasColumnName("changed_at")
                .IsRequired();

            builder.Property(h => h.ChangedByUserId)
                .HasColumnName("changed_by_user_id");

            builder.HasOne(h => h.QueueEntry)
                .WithMany()
                .HasForeignKey(h => h.QueueEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(h => h.ChangedByUser)
                .WithMany()
                .HasForeignKey(h => h.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
