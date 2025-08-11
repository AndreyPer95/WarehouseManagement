using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Models.Resources;
using WarehouseManagement.Models.Units;
using WarehouseManagement.Models.Receipts;
using WarehouseManagement.Models.Warehouse;

namespace WarehouseManagement.Data
{
    public class WarehouseContext : DbContext
    {
        public WarehouseContext(DbContextOptions<WarehouseContext> options)
            : base(options)
        {
        }

        public DbSet<Resource> Resources { get; set; } = null!;
        public DbSet<Unit> Units { get; set; } = null!;
        public DbSet<Receipt> Receipts { get; set; } = null!;
        public DbSet<ReceiptResource> ReceiptResources { get; set; } = null!;
        public DbSet<WarehouseBalance> WarehouseBalances { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация Resource
            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Конфигурация Unit
            modelBuilder.Entity<Unit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Конфигурация Receipt
            modelBuilder.Entity<Receipt>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Number).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Date).IsRequired();
                entity.HasIndex(e => e.Number).IsUnique();
            });

            // Конфигурация ReceiptResource
            modelBuilder.Entity<ReceiptResource>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasPrecision(18, 3);
                
                entity.HasOne(e => e.Receipt)
                    .WithMany(r => r.ReceiptResources)
                    .HasForeignKey(e => e.ReceiptId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Resource)
                    .WithMany(r => r.ReceiptResources)
                    .HasForeignKey(e => e.ResourceId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Unit)
                    .WithMany(u => u.ReceiptResources)
                    .HasForeignKey(e => e.UnitId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Конфигурация WarehouseBalance
            modelBuilder.Entity<WarehouseBalance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasPrecision(18, 3);
                
                entity.HasOne(e => e.Resource)
                    .WithMany()
                    .HasForeignKey(e => e.ResourceId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Unit)
                    .WithMany()
                    .HasForeignKey(e => e.UnitId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                // Уникальный индекс для комбинации Resource и Unit
                entity.HasIndex(e => new { e.ResourceId, e.UnitId }).IsUnique();
            });
        }
    }
}
