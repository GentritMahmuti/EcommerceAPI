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
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasIndex(x => new { x.UserId, x.ProductId }).IsUnique();
            });

            modelBuilder.Entity<WishListItem>(entity =>
            {
                entity.HasIndex(x => new { x.UserId, x.ProductId }).IsUnique();
            });

            modelBuilder.Entity<ProductOrderData>()
            .HasKey(pod => new { pod.ProductId, pod.OrderDataId });

            modelBuilder.Entity<ProductOrderData>()
                .HasOne(p => p.Product)
                .WithMany(pod => pod.ProductOrderData)
                .HasForeignKey(pi => pi.ProductId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProductOrderData>()
                .HasOne(od => od.OrderData)
                .WithMany(pod => pod.ProductOrderData)
                .HasForeignKey(odi => odi.OrderDataId);


        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OrderData> OrderData { get; set; }
        public DbSet<ProductOrderData> ProductOrderDatas { get; set; }
        public DbSet<CartItem> CardItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WishListItem> WishListItems { get; set; }




    }
}
