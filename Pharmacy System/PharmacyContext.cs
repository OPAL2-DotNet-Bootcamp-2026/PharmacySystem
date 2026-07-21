using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Models;
using Pharmacy_System.Modules;

namespace Pharmacy_System
{
    public class PharmacyContext : DbContext
    {
        public DbSet<Category> categories {  get; set; }
        public DbSet<Customer> customers { get; set; }
        public DbSet<CustomerOrder> customerOrders { get; set; }
        public DbSet<CustomerOrderDetail> customerOrderDetails { get; set; }
        public DbSet<Medicine> medicines { get; set; }
        public DbSet<Payment> payments { get; set; }
        public DbSet<Pharmacist> pharmacists { get; set; }
        public DbSet<PharmacistOrder> PharmacistsOrder { get; set; }
        public DbSet<PharmacistOrderDetail> PharmacistOrderDetails { get; set; }
        public DbSet<Pharmacy> pharmacies { get; set; }
        public DbSet<PharmacyStock> pharmacyStocks { get; set; }
        public DbSet<Supplier> suppliers { get; set; }
        public DbSet<Supply> Supplies { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<TransferDetails> transferDetails { get; set; }
        public DbSet<User> users { get; set; } 
        public DbSet<Warehouse> warehouses { get; set; }
        public DbSet<WarehouseStock> warehouseStocks { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"
                                    Server=localhost;
                                    Database=PharmacyDB;
                                    Trusted_Connection=True;
                                    TrustServerCertificate=True;");
        }
        

        public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                    entry.Entity.CreatedAt = now;
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                }
            }

            return base.SaveChangesAsync(ct);
        }
    }
}
