using Microsoft.EntityFrameworkCore;
using YemekAlemi.Models;

namespace YemekAlemi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Food> Foods { get; set; }
    }
}