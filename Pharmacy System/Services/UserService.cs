using Pharmacy_System.DTOs.User;
using Pharmacy_System.Models;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class UserService
    {
        private readonly UserRepo userRepo;
        private readonly AuthService authService;
        private readonly ILogger<UserService> logger;

        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 15;


        public UserService(
            UserRepo _userRepo,
            AuthService _authService,
            ILogger<UserService> _logger
        )
        {
            userRepo = _userRepo;

            authService = _authService;

            logger = _logger;
        }


        // =====================================
        // CHECK EMAIL
        // =====================================

        public async Task<bool> EmailExists(string email)
        {
            email = email.Trim();

            return await userRepo.EmailExists(email);
        }


        // =====================================
        // CHECK USERNAME
        // =====================================

        public async Task<bool> UsernameExists(string username)
        {
            username = username.Trim();

            return await userRepo.UsernameExists(username);
        }


        // =====================================
        // CREATE USER
        // =====================================

        public async Task<UserResponseDto?> CreateUser(
            RegisterUserDto dto
        )
        {
            string email =
                dto.Email.Trim();


            string username =
                dto.Username.Trim();


            bool emailExists =
                await userRepo.EmailExists(
                    email
                );


            if (emailExists)
            {
                return null;
            }


            bool usernameExists =
                await userRepo.UsernameExists(
                    username
                );


            if (usernameExists)
            {
                return null;
            }


            User user = new User()
            {
                Username = username,

                Email = email,

                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        dto.Password
                    ),

                Role = dto.Role,

                IsActive = true
            };


            await userRepo.AddUser(user);


            logger.LogInformation(
                "New user created: {Email} with role {Role}",
                user.Email,
                user.Role
            );


            return new UserResponseDto()
            {
                UserID = user.UserID,

                Username = user.Username,

                Email = user.Email,

                Role = user.Role,

                IsActive = user.IsActive
            };
        }


        // =====================================
        // LOGIN
        // =====================================

        public async Task<LoginResponseDto?> Login(
            LoginDto dto
        )
        {
            User? user =
                await userRepo.GetUserByEmail(
                    dto.Email
                );


            if (user == null)
            {
                logger.LogWarning(
                    "Login failed - no account for {Email}",
                    dto.Email
                );

                return null;
            }


            if (!user.IsActive)
            {
                logger.LogWarning(
                    "Login blocked - account {Email} is inactive",
                    dto.Email
                );

                return null;
            }


            if (
                user.LockedUntil != null &&
                user.LockedUntil > DateTime.UtcNow
            )
            {
                logger.LogWarning(
                    "Login blocked - {Email} is locked until {Until}",
                    dto.Email,
                    user.LockedUntil
                );

                return null;
            }


            bool validPassword =
                BCrypt.Net.BCrypt.Verify(
                    dto.Password,
                    user.PasswordHash
                );


            if (!validPassword)
            {
                user.FailedLoginAttempts++;


                if (
                    user.FailedLoginAttempts >=
                    MaxFailedAttempts
                )
                {
                    user.LockedUntil =
                        DateTime.UtcNow.AddMinutes(
                            LockoutMinutes
                        );


                    user.FailedLoginAttempts = 0;


                    logger.LogWarning(
                        "Account {Email} LOCKED for {Minutes} minutes after {Max} failed attempts",
                        dto.Email,
                        LockoutMinutes,
                        MaxFailedAttempts
                    );
                }

                else
                {
                    logger.LogWarning(
                        "Login failed - wrong password for {Email} (attempt {Count} of {Max})",
                        dto.Email,
                        user.FailedLoginAttempts,
                        MaxFailedAttempts
                    );
                }


                await userRepo.UserUpdate();


                return null;
            }


            if (
                user.FailedLoginAttempts > 0 ||
                user.LockedUntil != null
            )
            {
                user.FailedLoginAttempts = 0;

                user.LockedUntil = null;


                await userRepo.UserUpdate();
            }


            string token =
                authService.GenerateToken(
                    user
                );


            logger.LogInformation(
                "User {Email} logged in as {Role}",
                user.Email,
                user.Role
            );


            return new LoginResponseDto()
            {
                Token = token,

                Username = user.Username,

                Role = user.Role
            };
        }


        // =====================================
        // GET ALL USERS
        // =====================================

        public async Task<List<UserResponseDto>> GetAllUsers()
        {
            List<User> users =
                await userRepo.GetAllUsers();


            return users
                .Select(
                    u =>
                        new UserResponseDto
                        {
                            UserID = u.UserID,

                            Email = u.Email,

                            Username = u.Username,

                            Role = u.Role,

                            IsActive = u.IsActive
                        }
                )
                .ToList();
        }


        // =====================================
        // GET USER BY ID
        // =====================================

        public async Task<UserResponseDto?> GetUserById(
            int id
        )
        {
            User? user =
                await userRepo.GetUserById(
                    id
                );


            if (user == null)
            {
                return null;
            }


            return new UserResponseDto()
            {
                UserID = user.UserID,

                Username = user.Username,

                Email = user.Email,

                Role = user.Role,

                IsActive = user.IsActive
            };
        }


        // =====================================
        // GET USER BY EMAIL
        // =====================================

        public async Task<UserResponseDto?> GetUserByEmail(
            string email
        )
        {
            User? user =
                await userRepo.GetUserByEmail(
                    email
                );


            if (user == null)
            {
                return null;
            }


            return new UserResponseDto()
            {
                UserID = user.UserID,

                Username = user.Username,

                Email = user.Email,

                Role = user.Role,

                IsActive = user.IsActive
            };
        }


        // =====================================
        // SOFT DELETE USER
        // =====================================

        public async Task<bool> UserDelete(
            int id
        )
        {
            User? user =
                await userRepo.GetUserById(
                    id
                );


            if (user == null)
            {
                return false;
            }


            await userRepo.UserDelete(
                user
            );


            logger.LogInformation(
                "User {UserID} was deactivated",
                user.UserID
            );


            return true;
        }
    }
}