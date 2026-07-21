using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class SupplyRepo
    {
        private PharmacyContext context;

        public SupplyRepo(PharmacyContext _context)
        {
            context = _context;
        }

        // Returns all supply records with related data
        public List<Supply> GetAllSupply()
        {
            return context.Supplies
                .Include(s => s.Supplier)
                .Include(s => s.Medicine)
                .Include(s => s.Warehouse)
                .ToList();
        }

        // Returns one supply using  ID
        public Supply? GetSupplyById(int id)
        {
            return context.Supplies
                .Include(s => s.Supplier)
                .Include(s => s.Medicine)
                .Include(s => s.Warehouse)
                .FirstOrDefault(s => s.SupplyId == id);
        }

        // Adds  new supply 
        public void Add(Supply supply)
        {
            context.Supplies.Add(supply);
            context.SaveChanges();
        }

        // Save changes made to supply 
        public void SupplyUpdate()
        {
            context.SaveChanges();
        }

        // Delete supply 
        public void SupplyDelete(Supply supply)
        {
            context.Supplies.Remove(supply);
            context.SaveChanges();
        }
    }
}