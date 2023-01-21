using EcommerceAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Data
{
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<CartItem>(entity =>
            //{
            //    entity.HasIndex(x => new { x.UserId, x.ProductId }).IsUnique();
            //});

            //modelBuilder.Entity<WishListItem>(entity =>
            //{
            //    entity.HasIndex(x => new { x.UserId, x.ProductId }).IsUnique();
            //});

            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderData> OrderData { get; set; } 
        public DbSet<ProductOrderData> ProductOrderData { get; set; }
        public DbSet<WishListItem> WishListItems { get; set; }



    }
}
