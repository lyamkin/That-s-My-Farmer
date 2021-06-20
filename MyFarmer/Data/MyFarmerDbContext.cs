
using MyFarmer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MyFarmer.Data
{
    public class MyFarmerDbContext : IdentityDbContext
    {

        public MyFarmerDbContext(DbContextOptions<MyFarmerDbContext> options) : base(options) { }
        public DbSet<Service> Servises { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<FarmsService> FarmsServices { get; set; }
        public DbSet<Farm> Farms { get; set; }
        public DbSet<FarmsFood> FarmsFoods { get; set; }
        public DbSet<CustomerComment> CustomerComments { get; set; }
        public DbSet<FarmImage> FarmImages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data source=MyFarmer.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // On event model creating
            base.OnModelCreating(modelBuilder);
        }

    }
}