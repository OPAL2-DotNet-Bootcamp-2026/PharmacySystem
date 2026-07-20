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
        public DbSet<Supply> Supplies { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Warehouse> warehouse { get; set; }

        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<CustomerOrderDetail> CustomerOrderDetails { get; set; }
        public DbSet<PharmacistOrderDetail> PharmacistOrderDetails { get; set; }

        public PharmacyContext(DbContextOptions<PharmacyContext> options) : base(options)
        {

        }
    }
}
