using Pharmacy_System.DTOs.Pharmacist;
using Pharmacy_System.DTOs.User;
using Pharmacy_System.Models;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class PharmacistService
    {
        // All three are supplied by dependency injection.
        // Because they're registered AddScoped, DI gives all three the
        // SAME PharmacyContext instance for one web request — which is
        // what lets a transaction opened here also cover the repos' saves.
        private readonly PharmacistRepo pharmacistRepo;
        private readonly UserService userService;
        private readonly PharmacyContext context;

        public PharmacistService(PharmacistRepo pharmacistRepo, UserService userService, PharmacyContext context)
        {
            this.pharmacistRepo = pharmacistRepo;
            this.userService = userService;
            this.context = context;
        }

        // ---- reads ----

        public async Task<List<PharmacistDto>> GetAll()
        {
            List<Pharmacist> pharmacists = await pharmacistRepo.GetAllPharmacist();
            return pharmacists.Select(ToDto).ToList();
        }

        public async Task<PharmacistDto?> GetById(int id)
        {
            Pharmacist? pharmacist = await pharmacistRepo.GetPharmacistById(id);
            return pharmacist == null ? null : ToDto(pharmacist);
        }

        // ---- create: profile + linked User account, all-or-nothing ----

        public async Task<PharmacistDto?> Add(CreatePharmacistDto dto)
        {
            using var tx = await context.Database.BeginTransactionAsync();

            // Step 1 — create the login account through UserService,
            // so hashing and defaults live in ONE place.
            RegisterUserDto userDto = new RegisterUserDto
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password,
                Role = "Pharmacist"
            };

            UserResponseDto? user = await userService.CreateUser(userDto);

            if (user == null)
                return null;   // email already registered

            // Step 2 — create the profile that points at that User.
            Pharmacist pharmacist = new Pharmacist
            {
                UserID = user.UserID,
                PharmacyID = dto.PharmacyID,
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email
            };
            await pharmacistRepo.Add(pharmacist);

            await tx.CommitAsync();

            return ToDto(pharmacist);
        }

        // ---- update: profile fields only (login lives on the User account) ----

        public async Task<PharmacistDto?> Update(int id, UpdatePharmacistDto dto)
        {
            Pharmacist? pharmacist = await pharmacistRepo.GetPharmacistById(id);
            if (pharmacist == null) return null;

            pharmacist.FullName = dto.FullName;
            pharmacist.Phone = dto.Phone;
            pharmacist.Email = dto.Email;
            pharmacist.PharmacyID = dto.PharmacyID;

            await pharmacistRepo.PharmacistUpdate();
            return ToDto(pharmacist);
        }

        // ---- soft delete (repo sets IsActive = false) ----

        public async Task<bool> Delete(int id)
        {
            Pharmacist? pharmacist = await pharmacistRepo.GetPharmacistById(id);
            if (pharmacist == null) return false;

            await pharmacistRepo.PharmacistDelete(pharmacist);
            return true;
        }

        // ---- queries ----

        public async Task<List<PharmacistDto>> GetByPharmacy(int pharmacyId)
        {
            List<Pharmacist> list = await pharmacistRepo.GetByPharmacy(pharmacyId);
            return list.Select(ToDto).ToList();
        }

        public async Task<List<PharmacistDto>> SearchByName(string name)
        {
            List<Pharmacist> list = await pharmacistRepo.GetPharmacistByName(name);
            return list.Select(ToDto).ToList();
        }

        // ---- helpers ----

        // Maps the entity to the read DTO. (PharmacyName stays empty unless the
        // repo .Include()s the pharmacy navigation — see the review note.)
        private static PharmacistDto ToDto(Pharmacist p) => new PharmacistDto
        {
            PharmacistID = p.PharmacistID,
            UserID = p.UserID,
            PharmacyID = p.PharmacyID,
            FullName = p.FullName,
            Phone = p.Phone,
            Email = p.Email,
            IsActive = p.IsActive
        };
    }
}