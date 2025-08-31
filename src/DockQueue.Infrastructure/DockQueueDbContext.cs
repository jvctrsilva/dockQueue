using DockQueue.Domain;
using Microsoft.EntityFrameworkCore;

namespace DockQueue.Infrastructure
{
    public class DockQueueDbContext : DbContext
    {
        public DockQueueDbContext(DbContextOptions <DockQueueDbContext> options) 
            : base(options)
        { }

        public DbSet <User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            var userEntity = modelBuilder.Entity <User> ();

            userEntity.HasKey(u => u.Id);
            userEntity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            userEntity.HasIndex(u => u.Number).IsUnique();
            userEntity.Property(u => u.Number).IsRequired().HasMaxLength(11);
            userEntity.HasIndex(u => u.Email).IsUnique();
            userEntity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            userEntity.Property(u => u.Password).IsRequired().HasMaxLength(255);
            userEntity.Property(u => u.Role).IsRequired().HasMaxLength(20);
            userEntity.Property(u => u.CreatedAt).IsRequired();
        }
    }
}