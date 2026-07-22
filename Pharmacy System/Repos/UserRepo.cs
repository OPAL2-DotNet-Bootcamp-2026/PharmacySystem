using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Models;

namespace Pharmacy_System.Repos
{
    public class UserRepo
    {
        private readonly PharmacyContext context;

        public UserRepo(PharmacyContext _context)
        {
            context = _context;
        }

        // Get all active users from the database

        public async Task<List<User>> GetAllUsers()
        {
            return await context.users.Where(u => u.IsActive).ToListAsync();
        }


        // get active user by ID
        public async Task<User?> GetUserById(int id)
        {
            return await context.users.Where(u => u.UserID == id && u.IsActive)
                .FirstOrDefaultAsync(); // Return the user, or null if not found
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await context.users.FirstOrDefaultAsync(u =>u.Email == email &&u.IsActive);
        }

       
        public async Task<bool> EmailExists(string email)  // Check if the email already exists
        {
            return await context.users.AnyAsync(u => u.Email == email);
        }

        
        public async Task Add(User user)
        {
            await context.users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task UserUpdate()
        {
            await context.SaveChangesAsync();
        }


        public async Task UserDelete(User user) // Soft delete 
        {
            user.IsActive = false;

            await context.SaveChangesAsync();
        }

    }
}
