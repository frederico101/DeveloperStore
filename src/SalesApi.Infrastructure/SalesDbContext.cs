using Domain;
using Microsoft.EntityFrameworkCore;

namespace Structure
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure SaleItem
            modelBuilder.Entity<SaleItem>(entity =>
            {
                entity.HasKey(si => si.Id); // Use a unique identifier for each sale item
                entity.Property(si => si.Id).ValueGeneratedOnAdd(); // Auto-generate ID
                entity.Property(si => si.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(si => si.Discount).HasColumnType("decimal(18,2)");

                // Configure relationship with Sale
                entity.HasOne<Sale>()
                    .WithMany(s => s.Items)
                    .HasForeignKey(si => si.SaleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Sale
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(s => s.SaleId);
                entity.Property(s => s.SaleNumber).IsRequired().HasMaxLength(50);
                entity.Property(s => s.Customer).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Branch).IsRequired().HasMaxLength(50);
                entity.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)");
            });

            // Configure Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id); // Use a unique identifier for each product
                entity.Property(p => p.Id).ValueGeneratedOnAdd(); // Auto-generate ID
                entity.Property(p => p.Title).HasMaxLength(100); // Product title
                entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)"); // Product price
                entity.Property(p => p.Description).HasMaxLength(500); // Optional description
                entity.Property(p => p.Category).HasMaxLength(100); // Optional category
                entity.Property(p => p.Image).HasMaxLength(255); // Optional image URL
            });
        }
    }
}