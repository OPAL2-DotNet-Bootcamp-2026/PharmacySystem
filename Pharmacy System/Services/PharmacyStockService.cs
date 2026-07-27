using Pharmacy_System.Repos;
using Pharmacy_System.Models;
using Pharmacy_System.DTOs.PharmacyStock;

namespace Pharmacy_System.Services
{
    public class PharmacyStockService
    {
        private readonly PharmacyStockRepo pharmacyStockRepo;
        private readonly ILogger<PharmacyStockService> logger;

        public PharmacyStockService(
            PharmacyStockRepo pharmacyStockRepo,
            ILogger<PharmacyStockService> logger)
        {
            this.pharmacyStockRepo = pharmacyStockRepo;
            this.logger = logger;
        }

        public async Task<List<PharmacyStockDto>> GetByPharmacy(int pharmacyId)
        {
            List<PharmacyStock> stocks =
                await pharmacyStockRepo.GetPharmacyStockByPharmacyId(pharmacyId);
            return stocks.Select(ToDto).ToList();
        }

        public async Task<List<PharmacyStockDto>> GetByMedicine(int medicineId)
        {
            List<PharmacyStock> stocks =
                await pharmacyStockRepo.GetPharmacyStockByMedicineId(medicineId);
            return stocks.Select(ToDto).ToList();
        }

        public async Task<PharmacyStock?> GetStock(
           int pharmacyId,
           int medicineId)
        {
            return await pharmacyStockRepo
                .GetPharmacyStockAndMedicineById(
                    pharmacyId,
                    medicineId);
        }

        public async Task Increase(int pharmacyId, int medicineId, int qty, DateOnly expiryDate)
        {
            if (qty <= 0)
                throw new Exception("Quantity must be greater than zero");

            PharmacyStock? stock =
                await pharmacyStockRepo.GetPharmacyStockAndMedicineById(pharmacyId, medicineId);

            if (stock == null)
            {
                PharmacyStock newStock = new PharmacyStock
                {
                    PharmacyID = pharmacyId,
                    MedicineID = medicineId,
                    Quantity = qty,
                    ExpiryDate = expiryDate
                };
                await pharmacyStockRepo.Add(newStock);

                logger.LogInformation(
                    "Pharmacy {PharmacyId}: new stock row for medicine {MedicineId}, qty {Qty}",
                    pharmacyId, medicineId, qty);

                return;
            }

            stock.Quantity += qty;

            logger.LogInformation(
                "Pharmacy {PharmacyId}: medicine {MedicineId} +{Qty} (now {Total})",
                pharmacyId, medicineId, qty, stock.Quantity);

            await pharmacyStockRepo.Update();
        }

        public async Task Decrease(int pharmacyId, int medicineId, int qty)
        {
            if (qty <= 0)
                throw new Exception("Quantity must be greater than zero");

            PharmacyStock? stock =
                await pharmacyStockRepo.GetPharmacyStockAndMedicineById(pharmacyId, medicineId);

            if (stock == null)
                throw new Exception("Medicine stock does not exist in this pharmacy");

            if (stock.Quantity < qty)
                throw new Exception("Not enough quantity in the pharmacy");

            stock.Quantity -= qty;

            logger.LogInformation(
                "Pharmacy {PharmacyId}: medicine {MedicineId} -{Qty} (now {Total})",
                pharmacyId, medicineId, qty, stock.Quantity);

            await pharmacyStockRepo.Update();
        }

        public async Task<List<PharmacyStockDto>> GetLowStock(int pharmacyId)
        {
            List<PharmacyStock> stocks =
                await pharmacyStockRepo.GetPharmacyStockByLowStock(pharmacyId);
            return stocks.Select(ToDto).ToList();
        }

        private static PharmacyStockDto ToDto(PharmacyStock s) => new PharmacyStockDto
        {
            PharmacyStockID = s.PharmacyStockID,
            PharmacyID = s.PharmacyID,
            MedicineID = s.MedicineID,
            Quantity = s.Quantity,
            ExpiryDate = s.ExpiryDate
        };
    }
}