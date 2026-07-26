using Pharmacy_System.Repos;
using Pharmacy_System.Modules;
using Pharmacy_System.Models;
using Pharmacy_System.DTOs.Pharmacy;
using Pharmacy_System.DTOs.Pharmacist;
using Pharmacy_System.DTOs.PharmacyStock;

namespace Pharmacy_System.Services
{
    public class PharmacyService
    {
        private readonly PharmacyRepo pharmacyRepo;
        private readonly PharmacistRepo pharmacistRepo;

        public PharmacyService(PharmacyRepo pharmacyRepo, PharmacistRepo pharmacistRepo)
        {
            this.pharmacyRepo = pharmacyRepo;
            this.pharmacistRepo = pharmacistRepo;
        }

        public async Task<List<PharmacyDto>> GetAll()
        {
            List<Pharmacy> pharmacies = await pharmacyRepo.GetAllPharmacy();
            return pharmacies.Select(ToDto).ToList();
        }

        public async Task<PharmacyDto?> GetById(int id)
        {
            Pharmacy? pharmacy = await pharmacyRepo.GetPharmacyById(id);
            return pharmacy == null ? null : ToDto(pharmacy);
        }

        public async Task<PharmacyDto> Add(CreatePharmacyDto dto)
        {
            Pharmacy pharmacy = new Pharmacy
            {
                PharmacyName = dto.PharmacyName,
                Location = dto.Location,
                Phone = dto.Phone,
                StorageCapacity = dto.StorageCapacity
            };

            await pharmacyRepo.Add(pharmacy);
            return ToDto(pharmacy);
        }

        public async Task<PharmacyDto?> Update(int id, UpdatePharmacyDto dto)
        {
            Pharmacy? pharmacy = await pharmacyRepo.GetPharmacyById(id);
            if (pharmacy == null) return null;

            pharmacy.PharmacyName = dto.PharmacyName;
            pharmacy.Location = dto.Location;
            pharmacy.Phone = dto.Phone;
            pharmacy.StorageCapacity = dto.StorageCapacity;

            await pharmacyRepo.Update();
            return ToDto(pharmacy);
        }

        public async Task<bool> Delete(int id)
        {
            Pharmacy? pharmacy = await pharmacyRepo.GetPharmacyById(id);
            if (pharmacy == null) return false;

            await pharmacyRepo.Delete(pharmacy);
            return true;
        }

        public async Task<List<PharmacyDto>> SearchByName(string name)
        {
            List<Pharmacy> pharmacies = await pharmacyRepo.GetPharmacyByName(name);
            return pharmacies.Select(ToDto).ToList();
        }

        public async Task<List<PharmacyDto>> GetByLocation(string location)
        {
            List<Pharmacy> pharmacies = await pharmacyRepo.GetByLocation(location);
            return pharmacies.Select(ToDto).ToList();
        }

        public async Task<List<PharmacistDto>> GetPharmacists(int id)
        {
            List<Pharmacist> staff = await pharmacistRepo.GetByPharmacy(id);
            return staff.Select(p => new PharmacistDto
            {
                PharmacistID = p.PharmacistID,
                UserID = p.UserID,
                PharmacyID = p.PharmacyID,
                FullName = p.FullName,
                Phone = p.Phone,
                Email = p.Email,
                IsActive = p.IsActive
            }).ToList();
        }

        public async Task<List<PharmacyStockDto>> GetStock(int id)
        {
            List<PharmacyStock> stock = await pharmacyRepo.GetPharmacyStockById(id);
            return stock.Select(s => new PharmacyStockDto
            {
                PharmacyStockID = s.PharmacyStockID,
                PharmacyID = s.PharmacyID,
                MedicineID = s.MedicineID,
                Quantity = s.Quantity,
                ExpiryDate = s.ExpiryDate
            }).ToList();
        }

        private static PharmacyDto ToDto(Pharmacy p) => new PharmacyDto
        {
            PharmacyID = p.PharmacyID,
            PharmacyName = p.PharmacyName,
            Location = p.Location,
            Phone = p.Phone,
            StorageCapacity = p.StorageCapacity,
            IsActive = p.IsActive
        };
    }
}
