using Microsoft.EntityFrameworkCore;  // Use EF Core's DbContext and DbSet
using ProductCategoryManagement.Models;  // Reference to our Models namespace

namespace ProductCategoryManagement.Models
{
    public class AppDbContext : DbContext
    {
        // Constructor that accepts DbContextOptions to enable Dependency Injection
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet properties to represent tables
        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        // Later, we’ll add the Product DbSet here as well
        // public DbSet<Product> Products { get; set; }
    }
}
