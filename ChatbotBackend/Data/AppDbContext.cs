using ChatbotBackend.Model;
using Microsoft.EntityFrameworkCore;

namespace ChatbotBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasColumnType("decimal(18,2)");  // Sets precision to 18 and scale to 2

            base.OnModelCreating(modelBuilder);
        }

    }
}
