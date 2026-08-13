using Pharmacy_System.DTOs.WarehouseStock;
using Pharmacy_System.Models;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class WarehouseStockService
    {
        private readonly WarehouseStockRepo warehouseStockRepo;
        private readonly ILogger<WarehouseStockService> logger;


        public WarehouseStockService(
            WarehouseStockRepo _warehouseStockRepo,
            ILogger<WarehouseStockService> logger)
        {
            warehouseStockRepo = _warehouseStockRepo;
            this.logger = logger;
        }


        // Get all medicines stored in one warehouse
        public async Task<List<WarehouseStockDto>> GetByWarehouse(
            int warehouseId)
        {
            List<WarehouseStock> stocks =
                await warehouseStockRepo.GetByWarehouse(warehouseId);


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


        // Get stock records for one medicine
        public async Task<List<WarehouseStockDto>> GetByMedicine(
            int medicineId)
        {
            List<WarehouseStock> stocks =
                await warehouseStockRepo.GetByMedicine(medicineId);


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


        // Get one medicine stock from warehouse
        public async Task<WarehouseStock?> GetStock(
            int warehouseId,
            int medicineId)
        {
            return await warehouseStockRepo.GetStock(
                warehouseId,
                medicineId
            );
        }


        // Increase stock when a new supply is received
        public async Task Increase(
            int warehouseId,
            int medicineId,
            int qty,
            DateOnly expiryDate)
        {
            if (qty <= 0)
            {
                throw new Exception(
                    "Quantity must be greater than zero"
                );
            }


            WarehouseStock? stock =
                await warehouseStockRepo.GetStock(
                    warehouseId,
                    medicineId
                );


            // If medicine does not exist,
            // create a new stock row
            if (stock == null)
            {
                WarehouseStock newStock =
                    new WarehouseStock
                    {
                        WarehouseID = warehouseId,
                        MedicineID = medicineId,
                        Quantity = qty,
                        ExpiryDate = expiryDate
                    };


                await warehouseStockRepo.Add(newStock);


                logger.LogInformation(
                    "Warehouse {WarehouseId}: new stock row for medicine {MedicineId}, qty {Qty}",
                    warehouseId,
                    medicineId,
                    qty
                );


                return;
            }


            // Increase quantity
            stock.Quantity += qty;


            // Keep the nearest expiry date
            if (stock.ExpiryDate == null ||
                expiryDate < stock.ExpiryDate)
            {
                stock.ExpiryDate = expiryDate;
            }


            logger.LogInformation(
                "Warehouse {WarehouseId}: medicine {MedicineId} +{Qty} (now {Total})",
                warehouseId,
                medicineId,
                qty,
                stock.Quantity
            );


            await warehouseStockRepo.WarehouseStockUpdate();
        }


        // Decrease stock when medicine is transferred
        public async Task Decrease(
            int warehouseId,
            int medicineId,
            int qty)
        {
            if (qty <= 0)
            {
                throw new Exception(
                    "Quantity must be greater than zero"
                );
            }


            WarehouseStock? stock =
                await warehouseStockRepo.GetStock(
                    warehouseId,
                    medicineId
                );


            if (stock == null)
            {
                throw new Exception(
                    "Medicine stock does not exist"
                );
            }


            if (stock.Quantity < qty)
            {
                throw new Exception(
                    "Not enough quantity in the warehouse"
                );
            }


            stock.Quantity -= qty;


            logger.LogInformation(
                "Warehouse {WarehouseId}: medicine {MedicineId} -{Qty} (now {Total})",
                warehouseId,
                medicineId,
                qty,
                stock.Quantity
            );


            await warehouseStockRepo.WarehouseStockUpdate();
        }


        // Get low stock medicines
        public async Task<List<WarehouseStockDto>> GetLowStock(
            int warehouseId,
            int minimumQuantity)
        {
            List<WarehouseStock> stocks =
                await warehouseStockRepo.GetLowStock(
                    warehouseId,
                    minimumQuantity
                );


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