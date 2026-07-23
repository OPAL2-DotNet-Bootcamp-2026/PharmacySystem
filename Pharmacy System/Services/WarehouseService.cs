using Pharmacy_System.DTOs.Warehouse;
using Pharmacy_System.DTOs.WarehouseStock;
using Pharmacy_System.Models;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class WarehouseService
    {
        private readonly WarehouseRepo warehouseRepo;

        public WarehouseService(WarehouseRepo _warehouseRepo)
        {
            warehouseRepo = _warehouseRepo;
        }

        // Get the warehouse
        public async Task<WarehouseDto?> GetWarehouse()
        {
            Warehouse? warehouse = await warehouseRepo.GetWarehouse();

            if (warehouse == null)
            {
                return null;
            }

            return new WarehouseDto
            {
                WarehouseID = warehouse.WarehouseID,
                Location = warehouse.Location
            };

        }

        // Get warehouse by ID
        public async Task<WarehouseDto?> GetWarehouseById(int id)
        {
            Warehouse? warehouse = await warehouseRepo.GetWarehouseById(id);

            if (warehouse == null)
            {
                return null;
            }

            return new WarehouseDto
            {
                WarehouseID = warehouse.WarehouseID,
                Location = warehouse.Location
            };

        }

       
        public async Task<int> AddWarehouse(CreateWarehouseDto dto) //Add
        {
            Warehouse warehouse = new Warehouse
            {
                Location = dto.Location
            };

            await warehouseRepo.Add(warehouse);

            return warehouse.WarehouseID;
        }

        // Update warehouse
        public async Task<bool> UpdateWarehouse(int id,UpdateWarehouseDto dto)
        {
            Warehouse? warehouse =await warehouseRepo.GetWarehouseById(id);

            if (warehouse == null)
            {
                return false;
            }

            warehouse.Location = dto.Location;

            await warehouseRepo.WarehouseUpdate();

            return true;
        }

        // Get medicines currently held in the warehouse
        public async Task<List<WarehouseStockDto>> GetStock(int warehouseId)
        {
            Warehouse? warehouse = await warehouseRepo.GetWarehouseById(warehouseId);

            if (warehouse == null)
            {
                throw new Exception("Warehouse does not exist");
            }

            List<WarehouseStock> stocks = await warehouseRepo.GetStock(warehouseId);

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

        // Get warehouse items ordered by nearest expiry date
        public async Task<List<WarehouseStockDto>> GetExpiringItems(int warehouseId)
        {
            int days = 30; //to check medicines that will expire within the next 30 days

            Warehouse? warehouse = await warehouseRepo.GetWarehouseById(warehouseId);

            if (warehouse == null)
            {
                throw new Exception("Warehouse does not exist");
            }

            List<WarehouseStock> stocks = await warehouseRepo.GetExpiringItems(warehouseId,days);


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

        //delete warehouse => do not needed because our system has one permanent warehouse.
    }
}
