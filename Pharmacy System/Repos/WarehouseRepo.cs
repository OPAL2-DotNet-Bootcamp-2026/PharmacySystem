using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;
namespace Pharmacy_System.Repos
{
    public class WarehouseRepo
    {
        private PharmacyContext context;

        public WarehouseRepo(PharmacyContext _context)
        {
            context = _context;
        }

        public Warehouse GetWarehouse()  // Return the only our warehouse with its medicines
        {
            return context.warehouse.Include(w => w.Medicines).FirstOrDefault();
        }
        
        public bool WarehouseExists() // here to check that we have just one warehouse and to prevents adding a second warehouse.
        {
            return context.warehouse.Any();
        }

        public void Add(Warehouse warehouse) 
        {
            context.warehouse.Add(warehouse);
            context.SaveChanges();
        }

  
        public void WarehouseUpdate()  // save warehouse changes
        {
            context.SaveChanges();
        }

    }
}
