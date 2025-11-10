using DockQueue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DockQueue.Infra.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Box> Boxes { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }
        public DbSet<OperatorPermissions> OperatorPermissions { get; set; }
        public DbSet<OperatorStatusPermission> OperatorStatusPermissions { get; set; }
        public DbSet<OperatorBoxPermission> OperatorBoxPermissions { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<QueueEntry> QueueEntries { get; set; }
        public DbSet<QueueEntryStatusHistory> QueueEntryStatusHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Aplica todas as configurações (incluindo Driver/QueueEntry/History)
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // OPCIONAL: convenção global para deixar tudo minúsculo
            // (tabelas e colunas que não tiverem HasColumnName/ToTable explícito)
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                // Tabela em minúsculo se não tiver um nome explícito diferente
                var tableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(tableName))
                {
                    entity.SetTableName(tableName.ToLower());
                }

                // Colunas em minúsculo se o nome atual bater com o nome da propriedade
                foreach (var prop in entity.GetProperties())
                {
                    var storeObject = StoreObjectIdentifier.Table(entity.GetTableName()!, null);
                    var currentName = prop.GetColumnName(storeObject);

                    if (!string.IsNullOrEmpty(currentName) &&
                        currentName == prop.Name)
                    {
                        prop.SetColumnName(prop.Name.ToLower());
                    }
                }
            }
        }
    }

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var apiPath = Path.Combine(Directory.GetCurrentDirectory(), "../DockQueue.Api");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
