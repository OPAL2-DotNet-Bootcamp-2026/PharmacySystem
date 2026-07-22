using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class PharmacistOrderRepo
    {
        private PharmacyContext context;

        public PharmacistOrderRepo(PharmacyContext _context)
        {
            context = _context;
        }

        // Returns all pharmacist orders with related data
        public async Task<List<PharmacistOrder>> GetAllPharmacistOrders()
        {
            return await context.pharmacistOrders
                .Include(o => o.Pharmacist)
                .Include(o => o.Pharmacy)
                .Include(o => o.PharmacistOrderDetails).ThenInclude(d => d.Medicine)
                .ToList();
        }

        // Returns one pharmacist order using its ID
        public async Task<PharmacistOrder?> GetPharmacistOrderById(int id)
        {
            return await context.pharmacistOrders
                .Include(o => o.Pharmacist)
                .Include(o => o.Pharmacy)
                .Include(o => o.PharmacistOrderDetails).ThenInclude(d => d.Medicine)
                .FirstOrDefault(o => o.PharmacistOrderId == id);
        }

        // Adds new pharmacist order
        public async Task Add(PharmacistOrder pharmacistOrder)
        {
            await context.PharmacistOrders.Add(pharmacistOrder);
            await context.SaveChanges();
        }

        // Save changes made to  pharmacist order
        public async Task PharmacistOrderUpdate()
        {
            await context.SaveChanges();
        }

        // Delete  pharmacist order
        public async Task PharmacistOrderDelete(PharmacistOrder pharmacistOrder)
        {
            context.pharmacistOrders.Remove(pharmacistOrder);
            await context.SaveChanges();
        }
    }
}