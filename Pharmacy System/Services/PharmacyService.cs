using Pharmacy_System.Repos;
using Pharmacy_System.Modules;
using Pharmacy_System.DTOs.Pharmacy;

namespace Pharmacy_System.Services
{
    public class PharmacyService
    {
        private readonly PharmacyRepo pharmacyRepo;

        public PharmacyService(PharmacyRepo _pharmacyRepo)
        {
            pharmacyRepo = _pharmacyRepo;
        }

        // Get All Pharmacies
        public List<PharmacyDto> GetAll()
        {
            List<Pharmacy> pharmacies = pharmacyRepo.GetAllPharmacy();

            List<PharmacyDto> response = new List<PharmacyDto>();

            foreach (var pharmacy in pharmacies)
            {
                response.Add(new PharmacyDto
                {
                    PharmacyID = pharmacy.PharmacyID,
                    PharmacyName = pharmacy.PharmacyName,
                    Location = pharmacy.Location,
                    Phone = pharmacy.Phone,
                    StorageCapacity = pharmacy.StorageCapacity
                });
            }

            return response;
        }

        // Get Pharmacy By Id
        public PharmacyDto GetById(int id)
        {
            Pharmacy pharmacy = pharmacyRepo.GetPharmacyById(id);

            if (pharmacy == null)
                return null;

            return new PharmacyDto
            {
                PharmacyID = pharmacy.PharmacyID,
                PharmacyName = pharmacy.PharmacyName,
                Location = pharmacy.Location,
                Phone = pharmacy.Phone,
                StorageCapacity = pharmacy.StorageCapacity
            };
        }

        // Add Pharmacy
        public PharmacyDto Add(CreatePharmacyDto dto)
        {
            Pharmacy pharmacy = new Pharmacy
            {
                PharmacyName = dto.PharmacyName,
                Location = dto.Location,
                Phone = dto.Phone,
                StorageCapacity = dto.StorageCapacity
            };

            pharmacyRepo.Add(pharmacy);

            return new PharmacyDto
            {
                PharmacyID = pharmacy.PharmacyID,
                PharmacyName = pharmacy.PharmacyName,
                Location = pharmacy.Location,
                Phone = pharmacy.Phone,
                StorageCapacity = pharmacy.StorageCapacity
            };
        }

        // Update Pharmacy
        public PharmacyDto Update(int id, UpdatePharmacyDto dto)
        {
            Pharmacy pharmacy = pharmacyRepo.GetPharmacyById(id);

            if (pharmacy == null)
                return null;

            pharmacy.PharmacyName = dto.PharmacyName;
            pharmacy.Location = dto.Location;
            pharmacy.Phone = dto.Phone;
            pharmacy.StorageCapacity = dto.StorageCapacity;

            pharmacyRepo.Update();

            return new PharmacyDto
            {
                PharmacyID = pharmacy.PharmacyID,
                PharmacyName = pharmacy.PharmacyName,
                Location = pharmacy.Location,
                Phone = pharmacy.Phone,
                StorageCapacity = pharmacy.StorageCapacity
            };
        }
    }
}
