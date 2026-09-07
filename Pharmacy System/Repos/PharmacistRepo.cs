using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class PharmacistRepo
    {
        private PharmacyContext context;


        public PharmacistRepo(
            PharmacyContext _context
        )
        {
            context = _context;
        }



        // =========================================
        // GET ALL PHARMACISTS
        // =========================================

        public async Task<List<Pharmacist>>
            GetAllPharmacist()
        {
            return await context.pharmacists
                .ToListAsync();
        }



        // =========================================
        // GET PHARMACIST BY ID
        // =========================================

        public async Task<Pharmacist?>
            GetPharmacistById(
                int id
            )
        {
            return await context.pharmacists
                .FirstOrDefaultAsync(
                    p =>
                        p.PharmacistID == id
                );
        }



        // =========================================
        // GET PHARMACIST BY USER ID
        // NEW
        // =========================================

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



        // =========================================
        // GET PHARMACIST BY EMAIL
        // =========================================

        public async Task<Pharmacist?>
            GetPharmacistByEmail(
                string email
            )
        {
            return await context.pharmacists
                .FirstOrDefaultAsync(
                    p =>
                        p.Email == email
                );
        }



        // =========================================
        // CHECK EMAIL EXISTS
        // =========================================

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



        // =========================================
        // GET PHARMACY BY ID
        // =========================================

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



        // =========================================
        // GET PHARMACIST BY NAME
        // =========================================

        public async Task<List<Pharmacist>>
            GetPharmacistByName(
                string name
            )
        {
            return await context.pharmacists
                .Where(
                    p =>
                        p.FullName.Contains(
                            name
                        )
                )
                .ToListAsync();
        }



        // =========================================
        // GET PHARMACISTS BY PHARMACY
        // =========================================

        public async Task<List<Pharmacist>>
            GetByPharmacy(
                int pharmacyId
            )
        {
            return await context.pharmacists
                .Where(
                    p =>
                        p.PharmacyID ==
                        pharmacyId
                )
                .ToListAsync();
        }



        // =========================================
        // ADD PHARMACIST
        // =========================================

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



        // =========================================
        // UPDATE PHARMACIST
        // =========================================

        public async Task PharmacistUpdate()
        {
            await context
                .SaveChangesAsync();
        }



        // =========================================
        // SOFT DELETE PHARMACIST
        // =========================================

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