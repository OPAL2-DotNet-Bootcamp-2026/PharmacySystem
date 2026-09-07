using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class PharmacistRepo
    {
        private readonly PharmacyContext context;


        public PharmacistRepo(
            PharmacyContext _context
        )
        {
            context = _context;
        }


        // =====================================
        // GET ALL ACTIVE PHARMACISTS
        // =====================================

        public async Task<List<Pharmacist>>
            GetAllPharmacist()
        {
            return await context.pharmacists
                .Where(
                    p => p.IsActive
                )
                .ToListAsync();
        }


        // =====================================
        // GET ACTIVE PHARMACIST BY ID
        // =====================================

        public async Task<Pharmacist?>
            GetPharmacistById(
                int id
            )
        {
            return await context.pharmacists
                .FirstOrDefaultAsync(
                    p =>
                        p.PharmacistID == id
                        &&
                        p.IsActive
                );
        }


        // =====================================
        // GET PHARMACIST BY USER ID
        // IMPORTANT FOR DELETE
        // =====================================

        public async Task<Pharmacist?>
            GetPharmacistByUserId(
                int userId
            )
        {
            return await context.pharmacists
                .FirstOrDefaultAsync(
                    p =>
                        p.UserID == userId
                );
        }


        // =====================================
        // GET ACTIVE PHARMACIST BY EMAIL
        // =====================================

        public async Task<Pharmacist?>
            GetPharmacistByEmail(
                string email
            )
        {
            return await context.pharmacists
                .FirstOrDefaultAsync(
                    p =>
                        p.Email == email
                        &&
                        p.IsActive
                );
        }


        // =====================================
        // CHECK EMAIL EXISTS
        // =====================================

        public async Task<bool>
            EmailExists(
                string email
            )
        {
            return await context.pharmacists
                .AnyAsync(
                    p =>
                        p.Email == email
                );
        }


        // =====================================
        // GET PHARMACY
        // =====================================

        public async Task<Pharmacy?>
            GetPharmacyById(
                int id
            )
        {
            return await context.pharmacies
                .FirstOrDefaultAsync(
                    p =>
                        p.PharmacyID == id
                );
        }


        // =====================================
        // GET ACTIVE PHARMACIST BY NAME
        // =====================================

        public async Task<List<Pharmacist>>
            GetPharmacistByName(
                string name
            )
        {
            return await context.pharmacists
                .Where(
                    p =>
                        p.FullName.Contains(name)
                        &&
                        p.IsActive
                )
                .ToListAsync();
        }


        // =====================================
        // GET ACTIVE PHARMACISTS BY PHARMACY
        // =====================================

        public async Task<List<Pharmacist>>
            GetByPharmacy(
                int pharmacyId
            )
        {
            return await context.pharmacists
                .Where(
                    p =>
                        p.PharmacyID == pharmacyId
                        &&
                        p.IsActive
                )
                .ToListAsync();
        }


        // =====================================
        // ADD
        // =====================================

        public async Task Add(
            Pharmacist pharmacist
        )
        {
            await context.pharmacists
                .AddAsync(
                    pharmacist
                );


            await context
                .SaveChangesAsync();
        }


        // =====================================
        // UPDATE
        // =====================================

        public async Task PharmacistUpdate()
        {
            await context
                .SaveChangesAsync();
        }


        // =====================================
        // SOFT DELETE
        // =====================================

        public async Task PharmacistDelete(
            Pharmacist pharmacist
        )
        {
            pharmacist.IsActive =
                false;


            await context
                .SaveChangesAsync();
        }
    }
}