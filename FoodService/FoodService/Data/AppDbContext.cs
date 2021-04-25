using FoodService.Models;
using Microsoft.EntityFrameworkCore;


namespace FoodService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Food> Foods { get; set; }
    }
}