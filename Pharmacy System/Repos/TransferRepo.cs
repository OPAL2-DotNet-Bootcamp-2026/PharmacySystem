using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class TransferRepo
    {
        private readonly PharmacyContext context;

        public TransferRepo(PharmacyContext context)
        {
            this.context = context;
        }

        // Returns all transfers with related data
        public async Task<List<Transfer>> GetAllTransfer()
        {
            return await context.Transfers
                .Include(t => t.Warehouse)
                .Include(t => t.Pharmacy)
                .Include(t => t.PharmacistOrder)
                .Include(t => t.TransferDetails)
                    .ThenInclude(d => d.Medicine)
                .ToListAsync();
        }

        // Returns one transfer by ID with related data
        public async Task<Transfer?> GetTransferById(int id)
        {
            return await context.Transfers
                .Include(t => t.Warehouse)
                .Include(t => t.Pharmacy)
                .Include(t => t.PharmacistOrder)
                .Include(t => t.TransferDetails)
                    .ThenInclude(d => d.Medicine)
                .FirstOrDefaultAsync(t => t.TransferId == id);
        }

        // Adds a new transfer with its transfer details
        public async Task Add(Transfer transfer)
        {
            await context.Transfers.AddAsync(transfer);
            await context.SaveChangesAsync();
        }

        // Saves changes made to an existing transfer
        public async Task TransferUpdate()
        {
            await context.SaveChangesAsync();
        }

        // Deletes an existing transfer
        public async Task TransferDelete(Transfer transfer)
        {
            context.Transfers.Remove(transfer);
            await context.SaveChangesAsync();
        }
    }
}