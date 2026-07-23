using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class PharmacistRepo
    {
        private PharmacyContext context;

        public PharmacistRepo(PharmacyContext _context)
        {
            context = _context;
        }

        public async Task<List<Pharmacist>> GetAllPharmacist()
        {
            return await context.pharmacists.ToListAsync();
        }

        public async Task<Pharmacist?> GetPharmacistById(int id)
        {
            return await context.pharmacists.FirstOrDefaultAsync(p => p.PharmacistID == id);
        }

        public async Task<Pharmacist?> GetPharmacistByEmail(string email)
        {
            return await context.pharmacists.FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<bool> EmailExists(string email)
        {
            return await context.pharmacists.AnyAsync(p => p.Email == email);
        }

        public async Task<Pharmacy?> GetPharmacyById(int id)
        {
            return await context.pharmacies.FirstOrDefaultAsync(p => p.PharmacyID == id);
        }

        public async Task<List<Pharmacist>> GetPharmacistByName(string name)
        {
            return await context.pharmacists.Where(n => n.FullName == name).ToListAsync();
        }

        public async Task Add(Pharmacist pharmacists)
        {
            await context.pharmacists.AddAsync(pharmacists);
            await context.SaveChangesAsync();
        }

        public async Task PharmacistUpdate()
        {
            await context.SaveChangesAsync();
        }

        public async Task PharmacistDelete(Pharmacist pharmacists)
        {
            pharmacists.IsActive = false;
            await context.SaveChangesAsync();
        }
    }
}
