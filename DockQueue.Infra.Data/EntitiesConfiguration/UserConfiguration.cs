using Microsoft.EntityFrameworkCore;
using DockQueue.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DockQueue.Infra.Data.EntitiesConfiguration
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(u => u.Number).IsUnique();
            builder.Property(u => u.Number).IsRequired().HasMaxLength(11);
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Password).HasMaxLength(255).IsRequired();
            builder.Property(u => u.Role).HasMaxLength(20).IsRequired();
            builder.Property(u => u.CreatedAt).IsRequired();
        }
    }
}
