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
        public DbSet<TransferDetail> transferDetails { get; set; }
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


        private void StampAuditFields()
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
        }

        // Every sync save funnels through here
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            StampAuditFields();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        // Every async save funnels through here.
        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            StampAuditFields();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Default everything to "don't cascade": protects soft-deleted rows
            // and avoids SQL Server's multiple-cascade-paths rejection.
            foreach (var fk in modelBuilder.Model.GetEntityTypes()
                                                 .SelectMany(e => e.GetForeignKeys()))
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            // Re-enable cascade only where the parent OWNS the children.
            modelBuilder.Entity<CustomerOrder>().HasMany(o => o.CustomerOrderDetails)
                .WithOne(d => d.CustomerOrder).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PharmacistOrder>().HasMany(o => o.PharmacistOrderDetails)
                .WithOne(d => d.PharmacistOrder).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transfer>().HasMany(t => t.TransferDetails)
                .WithOne(d => d.Transfer).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasData(new User
            {
                UserID = 1,
                Username = "admin",
                Email = "admin@pharmacy.com",
                PasswordHash = "$2a$12$BV55ozb08WyBqeRg3/jN5O1/zkmg7dHlVLDiHBb.L89srGQgAm15G", // Admin123
                Role = "Admin",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            });
        }
    }
}
