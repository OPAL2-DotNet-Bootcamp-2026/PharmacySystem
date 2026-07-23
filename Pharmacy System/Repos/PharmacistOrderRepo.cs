using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class PharmacistOrderRepo
    {
        private readonly PharmacyContext context;

        public PharmacistOrderRepo(PharmacyContext context)
        {
            this.context = context;
        }

        public async Task<List<PharmacistOrder>> GetAllPharmacistOrders()
        {
            return await context.PharmacistsOrder
                .Include(o => o.Pharmacist)
                .Include(o => o.Pharmacy)
                .Include(o => o.PharmacistOrderDetails)
                    .ThenInclude(d => d.Medicine)
                .ToListAsync();
        }

        public async Task<PharmacistOrder?> GetPharmacistOrderById(int id)
        {
            return await context.PharmacistsOrder
                .Include(o => o.Pharmacist)
                .Include(o => o.Pharmacy)
                .Include(o => o.PharmacistOrderDetails)
                    .ThenInclude(d => d.Medicine)
                .FirstOrDefaultAsync(o => o.PharmacistOrderId == id);
        }

        public async Task Add(PharmacistOrder pharmacistOrder)
        {
            await context.PharmacistsOrder.AddAsync(pharmacistOrder);
            await context.SaveChangesAsync();
        }

        public async Task PharmacistOrderUpdate()
        {
            await context.SaveChangesAsync();
        }

        public async Task PharmacistOrderDelete(
            PharmacistOrder pharmacistOrder)
        {
            context.PharmacistsOrder.Remove(pharmacistOrder);
            await context.SaveChangesAsync();
        }
    }
}