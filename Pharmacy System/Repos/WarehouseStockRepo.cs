using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Models;

namespace Pharmacy_System.Repos
{
    public class WarehouseStockRepo
    {

        private readonly PharmacyContext context;

        public WarehouseStockRepo(PharmacyContext _context)
        {
            context = _context;
        }

        // Get all stock records for one warehouse
        public async Task<List<WarehouseStock>> GetByWarehouse(int warehouseId)
        {
            return await context.warehouseStocks.Where(ws => ws.WarehouseID == warehouseId)
                                                .Include(ws => ws.Medicine)
                                                .ToListAsync();
        }

        // Get the warehouses that contain a specific medicine
        public async Task<List<WarehouseStock>> GetByMedicine(int medicineId)
        {
            return await context.warehouseStocks.Where(ws => ws.MedicineID == medicineId)
                                                .Include(ws => ws.Warehouse)
                                                .ToListAsync();
        }

        // Get one medicine stock record from one warehouse
        public async Task<WarehouseStock?> GetStock(int warehouseId,int medicineId)
        {
            return await context.warehouseStocks.FirstOrDefaultAsync(ws =>ws.WarehouseID == warehouseId &&ws.MedicineID == medicineId);
        }

        // Get medicines with a low quantity
        public async Task<List<WarehouseStock>> GetLowStock(int warehouseId,int minimumQuantity)
        {
            return await context.warehouseStocks.Where(ws =>ws.WarehouseID == warehouseId &&ws.Quantity <= minimumQuantity)
                                                .Include(ws => ws.Medicine)
                                                .ToListAsync();
        }


        
        public async Task Add(WarehouseStock warehouseStock)
        {
            await context.warehouseStocks.AddAsync(warehouseStock);
            await context.SaveChangesAsync();
        }


      
        public async Task WarehouseStockUpdate()
        {
            await context.SaveChangesAsync();
        }


      
        public async Task WarehouseStockDelete(WarehouseStock warehouseStock)
        {
            context.warehouseStocks.Remove(warehouseStock);
            await context.SaveChangesAsync();
        }

    }
}
