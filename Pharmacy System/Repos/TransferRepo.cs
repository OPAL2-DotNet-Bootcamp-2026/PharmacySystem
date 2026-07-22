using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class TransferRepo
    {
        private PharmacyContext context;

        public TransferRepo(PharmacyContext _context)
        {
            context = _context;
        }

        // Returns all transfers with related data
        public async Task<List<Transfer>> GetAllTransfer()
        {
            return  await context.Transfers
                .Include(t => t.Warehouse)
                .Include(t => t.Pharmacy)
                .Include(t => t.PharmacistOrder)
                // Loads transfer details, then loads the medicine inside each detail
                .Include(t => t.TransferDetails)
                    .ThenInclude(d => d.Medicine)
                .ToList();
        }

        // Returns one transfer by ID with related data
        public async Task<Transfer?> GetTransferById(int id)
        {
            return await context.Transfers
                .Include(t => t.Warehouse)
                .Include(t => t.Pharmacy)
                .Include(t => t.PharmacistOrder)
                // Loads transfer details, then loads the medicine inside each detail
                .Include(t => t.TransferDetails)
                    .ThenInclude(d => d.Medicine)
                .FirstOrDefault(t => t.TransferId == id);
        }

        // Adds a new transfer with its transfer details
        public async Task Add(Transfer transfer)
        {
            await context.Transfers.Add(transfer);
            await context.SaveChanges();
        }

        // Saves changes made to an existing transfer
        public async Task TransferUpdate()
        {
            await context.SaveChanges();
        }

        // Deletes an existing transfer
        public async Task TransferDelete(Transfer transfer)
        {
            context.Transfers.Remove(transfer);
            await context.SaveChanges();
        }
    }
}