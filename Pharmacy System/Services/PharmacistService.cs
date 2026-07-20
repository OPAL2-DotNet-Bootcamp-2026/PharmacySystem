using Pharmacy_System.Repos;
using Pharmacy_System.Modules;
using Pharmacy_System.DTOs.Pharmacist;

namespace Pharmacy_System.Services
{
    public class PharmacistService
    {
        private readonly PharmacistRepo pharmacistRepo;
        

        public PharmacistService(PharmacistRepo _pharmacistRepo)
        {
            pharmacistRepo = _pharmacistRepo;
            
        }

        // Get All Pharmacists
        public List<PharmacistDto> GetAll()
        {
            List<Pharmacist> pharmacists = pharmacistRepo.GetAllPharmacist();

            List<PharmacistDto> response = new List<PharmacistDto>();

            foreach (var pharmacist in pharmacists)
            {
                response.Add(new PharmacistDto
                {
                    PharmacistID = pharmacist.PharmacistID,
                    FullName = pharmacist.FullName,
                    Email = pharmacist.Email,
                    Phone = pharmacist.Phone,
                    PharmacyID = pharmacist.PharmacyID
                });
            }

            return response;
        }

        // Get Pharmacist By Id
        public PharmacistDto GetById(int id)
        {
            Pharmacist pharmacist = pharmacistRepo.GetPharmacistById(id);

            if (pharmacist == null)
            {
                return null;
            }

            PharmacistDto response = new PharmacistDto
            {
                PharmacistID = pharmacist.PharmacistID,
                FullName = pharmacist.FullName,
                Email = pharmacist.Email,
                Phone= pharmacist.Phone,
                PharmacyID = pharmacist.PharmacyID
            };


            return response;
        }

        // Create Pharmacist
        public PharmacistDto Add(CreatePharmacistDto dto)
        {
            Pharmacist pharmacist = new Pharmacist
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                PharmacyID = dto.PharmacyID
            };

            if (pharmacistRepo.EmailExists(dto.Email))
                return null;

            pharmacistRepo.Add(pharmacist);

            PharmacistDto response = new PharmacistDto
            {
                PharmacistID = pharmacist.PharmacistID,
                FullName = pharmacist.FullName,
                Email = pharmacist.Email,
                Phone = pharmacist.Phone,
                PharmacyID = pharmacist.PharmacyID
            };

            return response;
        }

        // Update Pharmacist
        public PharmacistDto Update(int id, UpdatePharmacistDto dto)
        {
            Pharmacist pharmacist = pharmacistRepo.GetPharmacistById(id);

            if (pharmacist == null)
            {
                return null;
            }
            
            pharmacist.FullName = dto.FullName;
            pharmacist.Email = dto.Email;
            pharmacist.Phone = dto.Phone;
            pharmacist.PharmacyID = dto.PharmacyID;

            pharmacistRepo.PharmacistUpdate();

            PharmacistDto response = new PharmacistDto
            {
                PharmacistID = pharmacist.PharmacistID,
                FullName = pharmacist.FullName,
                Email = pharmacist.Email,
                Phone = pharmacist.Phone,
                PharmacyID = pharmacist.PharmacyID
            };

            return response;
        }
    }
}
