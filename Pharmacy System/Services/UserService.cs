using Pharmacy_System.DTOs.User;
using Pharmacy_System.Models;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class UserService
    {
        private readonly UserRepo userRepo;

        private readonly PharmacistRepo pharmacistRepo;

        private readonly AuthService authService;

        private readonly ILogger<UserService> logger;


        private const int MaxFailedAttempts =
            5;


        private const int LockoutMinutes =
            15;



        // =========================================
        // CONSTRUCTOR
        // =========================================

        public UserService(
            UserRepo _userRepo,
            PharmacistRepo _pharmacistRepo,
            AuthService _authService,
            ILogger<UserService> _logger
        )
        {
            userRepo =
                _userRepo;


            pharmacistRepo =
                _pharmacistRepo;


            authService =
                _authService;


            logger =
                _logger;
        }



        // =========================================
        // CREATE USER
        // =========================================

        public async Task<UserResponseDto?>
            CreateUser(
                RegisterUserDto dto
            )
        {

            bool emailExists =
                await userRepo.EmailExists(
                    dto.Email
                );


            if (emailExists)
            {
                return null;
            }


                Email = email,

            User user =
                new User()
                {
                    Username =
                        dto.Username,

                    Email =
                        dto.Email,

                    PasswordHash =
                        BCrypt.Net.BCrypt
                            .HashPassword(
                                dto.Password
                            ),

                    Role =
                        dto.Role,

                    IsActive =
                        true
                };



            await userRepo.AddUser(
                user
            );



            logger.LogInformation(
                "New user created: {Email} with role {Role}",
                user.Email,
                user.Role
            );



            return new UserResponseDto()
            {
                UserID =
                    user.UserID,

                Username =
                    user.Username,

                Email =
                    user.Email,

                Role =
                    user.Role,

                IsActive =
                    user.IsActive
            };

        }



        // =========================================
        // LOGIN
        // =========================================

        public async Task<LoginResponseDto?>
            Login(
                LoginDto dto
            )
        {

            User? user =
                await userRepo
                    .GetUserByEmail(
                        dto.Email
                    );



            // USER NOT FOUND

            if (user == null)
            {
                logger.LogWarning(
                    "Login failed - no account for {Email}",
                    dto.Email
                );


                return null;
            }



            // USER IS INACTIVE

            if (!user.IsActive)
            {
                logger.LogWarning(
                    "Login blocked - account {Email} is inactive",
                    dto.Email
                );


                return null;
            }



            // ACCOUNT IS LOCKED

            if (
                user.LockedUntil != null
                &&
                user.LockedUntil >
                DateTime.UtcNow
            )
            {
                logger.LogWarning(
                    "Login blocked - {Email} is locked until {Until}",
                    dto.Email,
                    user.LockedUntil
                );


                return null;
            }



            // CHECK PASSWORD

            bool validPassword =
                BCrypt.Net.BCrypt.Verify(
                    dto.Password,
                    user.PasswordHash
                );



            // WRONG PASSWORD

            if (!validPassword)
            {

                user.FailedLoginAttempts++;



                if (
                    user.FailedLoginAttempts >=
                    MaxFailedAttempts
                )
                {

                    user.LockedUntil =
                        DateTime.UtcNow
                            .AddMinutes(
                                LockoutMinutes
                            );


                    // Reset counter when locked

                    user.FailedLoginAttempts =
                        0;



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



                await userRepo
                    .UserUpdate();


                return null;

            }



            // =====================================
            // SUCCESSFUL LOGIN
            // CLEAR OLD FAILED ATTEMPTS
            // =====================================

            if (
                user.FailedLoginAttempts > 0
                ||
                user.LockedUntil != null
            )
            {

                user.FailedLoginAttempts =
                    0;


                user.LockedUntil =
                    null;


                await userRepo
                    .UserUpdate();

            }



            // CREATE JWT TOKEN

            string token =
                authService
                    .GenerateToken(
                        user
                    );



            logger.LogInformation(
                "User {Email} logged in as {Role}",
                user.Email,
                user.Role
            );



            return new LoginResponseDto()
            {
                Token =
                    token,

                Username =
                    user.Username,

                Role =
                    user.Role
            };
        }



        // =========================================
        // GET ALL USERS
        // =========================================

        public async Task<List<UserResponseDto>>
            GetAllUsers()
        {

            List<User> users =
                await userRepo
                    .GetAllUsers();



            return users
                .Select(
                    u =>
                        new UserResponseDto
                        {
                            UserID =
                                u.UserID,

                            Email =
                                u.Email,

                            Username =
                                u.Username,

                            Role =
                                u.Role,

                            IsActive =
                                u.IsActive
                        }
                )
                .ToList();

        }



        // =========================================
        // GET USER BY ID
        // =========================================

        public async Task<UserResponseDto?>
            GetUserById(
                int id
            )
        {

            User? user =
                await userRepo
                    .GetUserById(
                        id
                    );


            if (user == null)
            {
                return null;
            }



            return new UserResponseDto()
            {
                UserID =
                    user.UserID,

                Username =
                    user.Username,

                Email =
                    user.Email,

                Role =
                    user.Role,

                IsActive =
                    user.IsActive
            };
        }



        // =========================================
        // GET USER BY EMAIL
        // =========================================

        public async Task<UserResponseDto?>
            GetUserByEmail(
                string email
            )
        {

            User? user =
                await userRepo
                    .GetUserByEmail(
                        email
                    );


            if (user == null)
            {
                return null;
            }



            return new UserResponseDto()
            {
                UserID =
                    user.UserID,

                Username =
                    user.Username,

                Email =
                    user.Email,

                Role =
                    user.Role,

                IsActive =
                    user.IsActive
            };
        }



        // =========================================
        // DELETE USER
        // SOFT DELETE
        // =========================================

        public async Task<bool>
            UserDelete(
                int id
            )
        {

            // Find user

            User? user =
                await userRepo
                    .GetUserById(
                        id
                    );


            if (user == null)
            {
                return false;
            }



            // =====================================
            // IF USER IS A PHARMACIST
            // =====================================

            if (
                user.Role ==
                "Pharmacist"
            )
            {

                // Find pharmacist profile
                // using UserID

                Pharmacist? pharmacist =
                    await pharmacistRepo
                        .GetPharmacistByUserId(
                            user.UserID
                        );



                // If profile exists,
                // deactivate it too

                if (
                    pharmacist != null
                )
                {

                    await pharmacistRepo
                        .PharmacistDelete(
                            pharmacist
                        );

                }

            }



            // =====================================
            // DEACTIVATE USER LOGIN
            // =====================================

            await userRepo
                .UserDelete(
                    user
                );



            logger.LogInformation(
                "User {UserID} with role {Role} was deactivated",
                user.UserID,
                user.Role
            );



            return true;

        }
    }
}