using Microsoft.EntityFrameworkCore;
using TriAR.Model;

namespace TriAR.Database
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() => Database.EnsureCreated();

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=C:\sqlite\TZ.db");
        }

        public DbSet<Rest> Rests { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Producer> Producers { get; set; }

        public static ApplicationContext New => new ApplicationContext();
    }
}
