using DockQueue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DockQueue.Infra.Data.EntitiesConfiguration
{
    internal class BoxConfiguration : IEntityTypeConfiguration<Box>
    {
        public void Configure(EntityTypeBuilder<Box> builder)
        {
            builder.ToTable("boxes");           // nome da tabela
            builder.HasKey(b => b.Id);          // chave primária
            builder.Property(b => b.Name).IsRequired().HasMaxLength(20);
            builder.Property(b => b.Status).IsRequired();
            builder.Property(b => b.DriverId).IsRequired(false);        // pode ser nulo se o box estiver vazio
            builder.Property(b => b.CreatedAt).IsRequired();

            // Se quiser, você pode adicionar FK para Driver
            //builder.HasOne(b => b.Driver)       // propriedade de navegação
            //       .WithMany()                 // um Driver pode ocupar múltiplos boxes ao longo do tempo, ou 1:1 se preferir
            //       .HasForeignKey(b => b.DriverId)
            //       .OnDelete(DeleteBehavior.SetNull); // se o driver for deletado, mantém o box livre
        }
    }
}
