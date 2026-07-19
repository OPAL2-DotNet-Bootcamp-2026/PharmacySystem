using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System
{
    public class PharmacyContext : DbContext
    {
        public DbSet<Customer> customers { get; set; }
        public DbSet<Medicine> medicines { get; set; }
        public DbSet<Pharmacist> pharmacists { get; set; }
        public DbSet<PharmacistOrder> pharmacistsOrder { get; set; }
        public DbSet<Pharmacy> pharmacies { get; set; }
        public DbSet<Supplier> suppliers { get; set; }
        public DbSet<Supply> supplies { get; set; }
        public DbSet<Transfer> transfers { get; set; }
        public DbSet<Warehouse> warehouses { get; set; }

        public PharmacyContext(DbContextOptions<PharmacyContext> options) : base(options)
        {

        }
    }
}
