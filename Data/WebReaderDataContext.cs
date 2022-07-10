using Microsoft.EntityFrameworkCore;
using WebReader.Models;

namespace Data
{
    public class WebReaderDataContext : DbContext
    {
        public WebReaderDataContext(DbContextOptions options) : base(options) { }
        public DbSet<Climate> Climates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WebReaderDataContext).Assembly);

            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;
            }

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker
                .Entries()
                .Where(entry =>
                    entry.Entity.GetType().GetProperty("DataCadastro") != null &&
                    entry.Entity.GetType().GetProperty("DataAlteracao") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("InsertTimestamp").CurrentValue = DateTime.Now;
                    entry.Property("UpdateTimestamp").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("InsertTimestamp").IsModified = false;
                    entry.Property("UpdateTimestamp").CurrentValue = DateTime.Now;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}