using Pharmacy_System.DTOs.User;
using Pharmacy_System.Models;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class UserService
    {

            private UserRepo userRepo;
            private AuthService authService;

            public UserService(UserRepo _userRepo, AuthService _authService) 
            {
                userRepo = _userRepo;
                authService = _authService;
            }

        // --- Create User  / Register 
        public async Task<UserResponseDto?> CreateUser(RegisterUserDto dto)
        {
            // Check if the email is already registered
            bool emailExists = await userRepo.EmailExists(dto.Email);

            if (emailExists)
            {
                return null;
            }

            User user = new User();

            user.Username = dto.Username;
            user.Email = dto.Email;

            // Hash the password before saving it in the database
            user.PasswordHash =BCrypt.Net.BCrypt.HashPassword(dto.Password);

            user.Role = dto.Role;
            user.IsActive = true;

            await userRepo.AddUser(user);

            UserResponseDto response = new UserResponseDto();

            response.UserID = user.UserID;
            response.Username = user.Username;
            response.Email = user.Email;
            response.Role = user.Role;
            response.IsActive = user.IsActive;

            return response;
        }


        // --- Login 
        public async Task<LoginResponseDto?> Login(LoginDto dto)
        {
            User? user = await userRepo.GetUserByEmail(dto.Email);

            if (user == null)
            {
                return null;
            }

            // Inactive users cannot log in
            if (!user.IsActive)
            {
                return null;
            }

            // Compare the entered password with the stored hashed password
            bool validPassword = BCrypt.Net.BCrypt.Verify(dto.PasswordHash, user.PasswordHash);

            if (!validPassword)
            {
                return null;
            }

           // Generate JWT token
         string token = authService.GenerateToken(user);

            LoginResponseDto response = new LoginResponseDto();

            response.Token = token;
            response.Username = user.Username;
            response.Role = user.Role;


            return response;
       }


        //  Get All Users 
        public async Task<List<UserResponseDto>> GetAllUsers()
        {
            List<User> users = await userRepo.GetAllUsers();

            return users.Select(u => new UserResponseDto
            {
                UserID = u.UserID,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive
            }).ToList();
        }

        // Get User by ID
        public async Task<UserResponseDto?> GetUserById(int id)
        {
            User? user = await userRepo.GetUserById(id);

            if (user == null)
            {
                return null;
            }

            UserResponseDto response = new UserResponseDto();

            response.UserID = user.UserID;
            response.Email = user.Email;
            response.Role = user.Role;
            response.IsActive = user.IsActive;

            return response;
        }

        //  Get User by Email 
        public async Task<UserResponseDto?> GetUserByEmail(string email)
        {
            User? user = await userRepo.GetUserByEmail(email);

            if (user == null)
            {
                return null;
            }

            UserResponseDto response = new UserResponseDto();

            response.UserID = user.UserID;
            response.Email = user.Email;
            response.Role = user.Role;
            response.IsActive = user.IsActive;

            return response;
        }

        //  Delete User 
        public async Task<bool> UserDelete(int id)
        {
            User? user = await userRepo.GetUserById(id);

            if (user == null)
            {
                return false;
            }

            // Soft delete
            await userRepo.UserDelete(user);

            return true;
        }

    }
}
