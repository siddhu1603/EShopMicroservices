using Microsoft.EntityFrameworkCore;
using Discount.Grpc.Models;

namespace Discount.Grpc.Data
{
    public class DiscountContext : DbContext
    {
        public DbSet<Coupon> Coupons { get; set; } = default!;

        public DiscountContext(DbContextOptions<DiscountContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon { Id = 1, ProductName = "Samsung S24 Ultra", Description = "Samsung discount", Amount = 8000 },
                new Coupon { Id = 2, ProductName = "Iphone 16 Pro", Description = "Iphone discount", Amount = 6000 }
                );
        }
    }
}
