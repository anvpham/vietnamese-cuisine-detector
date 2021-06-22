using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFood>().HasKey(e => new { e.UserId, e.FoodName });
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Food> Food { get; set; }
        
        public DbSet<UserFood> UserFood { get; set; }
    }
}