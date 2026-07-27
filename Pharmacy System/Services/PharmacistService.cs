using Pharmacy_System.Repos;
using Pharmacy_System.Modules;
using Pharmacy_System.Models;
using Pharmacy_System.DTOs.Pharmacist;

namespace Pharmacy_System.Services
{
    public class PharmacistService
    {
        // All three are supplied by dependency injection.
        // Because they're registered AddScoped, DI gives all three the
        // SAME PharmacyContext instance for one web request — which is
        // what lets a transaction opened here also cover the repos' saves.
        private readonly PharmacistRepo pharmacistRepo;
        private readonly UserRepo userRepo;
        private readonly PharmacyContext context;

        public PharmacistService(
            PharmacistRepo pharmacistRepo,
            UserRepo userRepo,
            PharmacyContext context)
        {
            this.pharmacistRepo = pharmacistRepo;
            this.userRepo = userRepo;
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
            // Reject early if the email is already taken (avoids starting work we'll undo).
            if (await userRepo.EmailExists(dto.Email))
                return null;

            // Open the "all or nothing" wrapper on the shared context.
            // 'using' means: if we leave this method without committing,
            // the transaction is disposed and everything rolls back.
            using var tx = await context.Database.BeginTransactionAsync();

            // Step 1 — create the login account.
            User user = new User
            {
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),   // never store plaintext
                Role = "Pharmacist"
            };
            await userRepo.AddUser(user);   // saves, but stays PENDING inside the transaction
                                        // after this, user.UserID is filled in by the database

            // Step 2 — create the profile that points at that User.
            Pharmacist pharmacist = new Pharmacist
            {
                UserID = user.UserID,   // link the profile to the account we just made
                PharmacyID = dto.PharmacyID,
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email
            };
            await pharmacistRepo.Add(pharmacist);   // if THIS throws, we never reach the commit
                                                    // below, so Step 1 is rolled back too

            // Both inserts worked — make them permanent, together.
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

        // Salted one-way hash. Requires the BCrypt.Net-Next NuGet package.
        private static string HashPassword(string plain)
            => BCrypt.Net.BCrypt.HashPassword(plain);
    }
}