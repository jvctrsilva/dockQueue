using Microsoft.EntityFrameworkCore;
using DockQueue.Domain.Entities;

namespace DockQueue.Infra.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        protected ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
