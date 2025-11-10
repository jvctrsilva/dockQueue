using DockQueue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DockQueue.Infra.Data
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.ToTable("drivers");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .HasColumnName("id");

            builder.Property(d => d.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.DocumentNumber)
                .HasColumnName("document_number")
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(d => d.VehiclePlate)
                .HasColumnName("vehicle_plate")
                .IsRequired()
                .HasMaxLength(10);

            // Se quiser garantir unicidade de documento:
            builder.HasIndex(d => d.DocumentNumber)
                .HasDatabaseName("ix_drivers_document_number")
                .IsUnique(false); // coloca true se quiser único

            // Se quiser índice de placa:
            builder.HasIndex(d => d.VehiclePlate)
                .HasDatabaseName("ix_drivers_vehicle_plate");
        }
    }
}
