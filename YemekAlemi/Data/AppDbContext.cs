using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YemekAlemi.Models;

namespace YemekAlemi.Data
{

    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Food> Foods { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<AppLog> AppLogs { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
    }
}