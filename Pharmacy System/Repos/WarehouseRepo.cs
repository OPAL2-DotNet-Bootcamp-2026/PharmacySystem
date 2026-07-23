using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Models;
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

        public async Task<Warehouse?> GetWarehouse()  // Return the only our warehouse with its medicines
        {
            return await context.warehouses.FirstOrDefaultAsync();
        }


        public async Task<Warehouse> GetWarehouseById(int id)
        {
            return await context.warehouses.FirstOrDefaultAsync(w => w.WarehouseID == id);
        }


        public async Task Add(Warehouse warehouse) 
        {
            await context.warehouses.AddAsync(warehouse);
            await context.SaveChangesAsync();
        }

  
        public async Task WarehouseUpdate()  // save warehouse changes
        {
            await context.SaveChangesAsync();
        }
        // Get medicines currently held in the warehouse
        public async Task<List<WarehouseStock>> GetStock(int warehouseId)
        {
            return await context.warehouseStocks.Where(ws =>ws.WarehouseID == warehouseId &&
                    ws.Quantity > 0).Include(ws => ws.Medicine).ToListAsync();
        }

        // Get warehouse items ordered by nearest expiry date
        public async Task<List<WarehouseStock>> GetExpiringItems(int warehouseId,int days)
        {
            //only store today date 
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            // to Calculate the last day
            DateOnly expiryLimit =today.AddDays(days);

            return await context.warehouseStocks.Where(ws =>ws.WarehouseID == warehouseId &&
                                                                              ws.ExpiryDate.HasValue &&
                                                                              ws.ExpiryDate.Value >= today &&
                                                                              ws.ExpiryDate.Value <= expiryLimit)
                                                                              .Include(ws => ws.Medicine)
                                                                              .ToListAsync();
        }

    }
}
