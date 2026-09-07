using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Models;

namespace Pharmacy_System.Repos
{
    public class UserRepo
    {
        private readonly PharmacyContext context;


        public UserRepo(
            PharmacyContext _context
        )
        {
            context = _context;
        }



        // =====================================
        // GET ALL ACTIVE USERS
        // =====================================

        public async Task<List<User>>
            GetAllUsers()
        {
            return await context.users
                .Where(
                    u => u.IsActive
                )
                .ToListAsync();
        }



        // =====================================
        // GET USER BY ID
        // =====================================

        public async Task<User?>
            GetUserById(
                int id
            )
        {
            return await context.users
                .Where(
                    u =>
                        u.UserID == id
                        &&
                        u.IsActive
                )
                .FirstOrDefaultAsync();
        }



        // =====================================
        // GET USER BY EMAIL
        // =====================================

        public async Task<User?>
            GetUserByEmail(
                string email
            )
        {
            return await context.users
                .FirstOrDefaultAsync(
                    u =>
                        u.Email == email
                        &&
                        u.IsActive
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
            return await context.users
                .AnyAsync(
                    u =>
                        u.Email == email
                );
        }



        // =====================================
        // CHECK USERNAME EXISTS
        // NEW
        // =====================================

        public async Task<bool>
            UsernameExists(
                string username
            )
        {
            return await context.users
                .AnyAsync(
                    u =>
                        u.Username == username
                );
        }



        // =====================================
        // ADD USER
        // =====================================

        public async Task AddUser(
            User user
        )
        {
            await context.users
                .AddAsync(
                    user
                );


            await context
                .SaveChangesAsync();
        }



        // =====================================
        // UPDATE USER
        // =====================================

        public async Task UserUpdate()
        {
            await context
                .SaveChangesAsync();
        }



        // =====================================
        // SOFT DELETE USER
        // =====================================

        public async Task UserDelete(
            User user
        )
        {
            user.IsActive =
                false;


            user.UpdatedAt =
                DateTime.UtcNow;


            await context
                .SaveChangesAsync();
        }
    }
}