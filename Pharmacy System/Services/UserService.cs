using Pharmacy_System.DTOs.User;
using Pharmacy_System.Models;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class UserService
    {

        private UserRepo userRepo;
        private AuthService authService;
        private readonly ILogger<UserService> logger;

        public UserService(UserRepo _userRepo, AuthService _authService, ILogger<UserService> logger) 
        {
            userRepo = _userRepo;
            authService = _authService;
            this.logger = logger;
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

            logger.LogInformation("New user created: {Email} with role {Role}", user.Email, user.Role);

            return response;
        }


        // --- Login 
        public async Task<LoginResponseDto?> Login(LoginDto dto)
        {
            User? user = await userRepo.GetUserByEmail(dto.Email);

            if (user == null)
            {
                logger.LogWarning("Login failed - no account for {Email}", dto.Email);
                return null;
            }

            // Inactive users cannot log in
            if (!user.IsActive)
            {
                logger.LogWarning("Login blocked - inactive account {Email}", dto.Email);
                return null;
            }

            // Compare the entered password with the stored hashed password
            bool validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!validPassword)
            {
                logger.LogWarning("Login failed - wrong password for {Email}", dto.Email);
                return null;
            }

            // Generate JWT token
            string token = authService.GenerateToken(user);

            logger.LogInformation("User {Email} logged in as {Role}", user.Email, user.Role);

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
                Username = u.Username,
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
            response.Username = user.Username;
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
            response.Username = user.Username;
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
