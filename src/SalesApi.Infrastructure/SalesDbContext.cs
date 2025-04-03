using Domain;
using Microsoft.EntityFrameworkCore;

namespace Structure
{
   public class SalesDbContext : DbContext
    {
    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    //public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure SaleItem
            modelBuilder.Entity<SaleItem>(entity =>
            {
                entity.HasKey(si => si.Id); // Use a unique identifier for each sale item
                entity.Property(si => si.Id).ValueGeneratedOnAdd(); // Auto-generate ID
                entity.HasKey(si => si.ProductId); // Define primary key
                entity.Property(si => si.ProductName).IsRequired().HasMaxLength(100);
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
        }
   }
}
