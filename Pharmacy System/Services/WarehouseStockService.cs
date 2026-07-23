
using Pharmacy_System.DTOs.WarehouseStock;
using Pharmacy_System.Models;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class WarehouseStockService
    {

        private readonly WarehouseStockRepo warehouseStockRepo;

        public WarehouseStockService(WarehouseStockRepo _warehouseStockRepo)
        {
            warehouseStockRepo = _warehouseStockRepo;
        }


        // here to get all medicines stored in one warehouse
        public async Task<List<WarehouseStockDto>> GetByWarehouse(int warehouseId)
        {
            List<WarehouseStock> stocks = await warehouseStockRepo.GetByWarehouse(warehouseId);

            return stocks.Select(ws => new WarehouseStockDto
            {
                WarehouseStockID = ws.WarehouseStockID,
                WarehouseID = ws.WarehouseID,
                MedicineID = ws.MedicineID,
                MedicineName = ws.Medicine.MedicineName,
                Quantity = ws.Quantity,
                ExpiryDate = ws.ExpiryDate

            }).ToList();
        }


        // here to get stock records for one medicine
        public async Task<List<WarehouseStockDto>> GetByMedicine(int medicineId)
        {
            List<WarehouseStock> stocks = await warehouseStockRepo.GetByMedicine(medicineId);

            return stocks.Select(ws => new WarehouseStockDto
            {
                WarehouseStockID = ws.WarehouseStockID,
                WarehouseID = ws.WarehouseID,
                MedicineID = ws.MedicineID,
                MedicineName = ws.Medicine.MedicineName,
                Quantity = ws.Quantity,
                ExpiryDate = ws.ExpiryDate

            }).ToList();
        }


        // Increase stock when a supply is received
        public async Task Increase(int warehouseId, int medicineId, int qty)
        {
            if (qty <= 0)
            {
                throw new Exception(
                    "Quantity must be greater than zero");
            }

            WarehouseStock? stock = await warehouseStockRepo.GetStock(warehouseId, medicineId);


            if (stock == null)
            {
                WarehouseStock newStock = new WarehouseStock
                {
                    WarehouseID = warehouseId,
                    MedicineID = medicineId,
                    Quantity = qty
                };

                await warehouseStockRepo.Add(newStock);

                return;
            }

            stock.Quantity += qty;

            await warehouseStockRepo.WarehouseStockUpdate();

        }


        // Decrease stock when medicine is transferred out
        public async Task Decrease(int warehouseId,int medicineId,int qty)
        {
            if (qty <= 0)
            {
                throw new Exception(
                    "Quantity must be greater than zero");
            }

            WarehouseStock? stock =await warehouseStockRepo.GetStock(warehouseId,medicineId);

            if (stock == null)
            {
                throw new Exception("Medicine stock does not exist");
            }

            if (stock.Quantity < qty)
            {
                throw new Exception("Not enough quantity in the warehouse");
            }

            stock.Quantity -= qty;

            await warehouseStockRepo.WarehouseStockUpdate();
        }

        public async Task<List<WarehouseStockDto>> GetLowStock(int warehouseId,int minimumQuantity)
        {
            List<WarehouseStock> stocks = await warehouseStockRepo.GetLowStock(warehouseId,minimumQuantity);

            return stocks.Select(ws => new WarehouseStockDto
            {
                WarehouseStockID = ws.WarehouseStockID,
                WarehouseID = ws.WarehouseID,
                MedicineID = ws.MedicineID,
                MedicineName = ws.Medicine.MedicineName,
                Quantity = ws.Quantity,
                ExpiryDate = ws.ExpiryDate

            }).ToList();
        }










    }
}  
