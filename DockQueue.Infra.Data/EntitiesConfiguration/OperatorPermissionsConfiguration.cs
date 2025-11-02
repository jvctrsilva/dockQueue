    using DockQueue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DockQueue.Infra.Data.EntitiesConfiguration
{
    internal class OperatorPermissionsConfiguration : IEntityTypeConfiguration<OperatorPermissions>
    {
        public void Configure(EntityTypeBuilder<OperatorPermissions> b)
        {
            b.ToTable("operator_permissions");
            b.HasKey(x => x.Id);

            b.HasIndex(x => x.UserId).IsUnique();      // 1 registro de permissões por usuário
            b.Property(x => x.UserId).IsRequired();

            // Flags de telas armazenadas como int
            b.Property(x => x.AllowedScreens).HasConversion<int>().IsRequired();

            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.UpdatedAt).IsRequired();

            // Relacionamentos com coleções
            b.HasMany(x => x.AllowedStatuses)
             .WithOne()
             .HasForeignKey("OperatorPermissionsId")
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.AllowedBoxes)
             .WithOne()
             .HasForeignKey("OperatorPermissionsId")
             .OnDelete(DeleteBehavior.Cascade);
        }
    }

    internal class OperatorStatusPermissionConfiguration : IEntityTypeConfiguration<OperatorStatusPermission>
    {
        public void Configure(EntityTypeBuilder<OperatorStatusPermission> b)
        {
            b.ToTable("operator_status_permissions");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.UserId, x.StatusId }).IsUnique(); // evita duplicidade
            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.StatusId).IsRequired();
        }
    }

    internal class OperatorBoxPermissionConfiguration : IEntityTypeConfiguration<OperatorBoxPermission>
    {
        public void Configure(EntityTypeBuilder<OperatorBoxPermission> b)
        {
            b.ToTable("operator_box_permissions");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.UserId, x.BoxId }).IsUnique();
            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.BoxId).IsRequired();
        }
    }
}
