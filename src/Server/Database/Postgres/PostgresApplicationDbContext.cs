using Microsoft.EntityFrameworkCore;

namespace NoCrast.Server.Database.Postgres
{
    public class PostgresApplicationDbContext : ApplicationDbContext
    {
        public PostgresApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasPostgresExtension("uuid-ossp");
        }
    }
}
