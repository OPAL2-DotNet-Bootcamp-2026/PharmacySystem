using Microsoft.IdentityModel.Tokens;
using Pharmacy_System.DTOs.User;
using Pharmacy_System.Models;
using Pharmacy_System.Repos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Pharmacy_System.Services
{
    public class AuthService
    {
        private IConfiguration config;
        private UserRepo userRepo;

        public AuthService(
            IConfiguration _config,
            UserRepo _userRepo)
        {
            config = _config;
            userRepo = _userRepo;

        }

        // Check email and password, then return a token
        public async Task<string> Login(LoginDto dto)
        {
            User? user = await userRepo.GetUserByEmail(dto.Email);

            if (user == null)
            {
                throw new Exception("Invalid email or password");
            }

            if (user.IsActive == false)
            {
                throw new Exception("This account is inactive");
            }

            bool passwordIsCorrect =
                BCrypt.Net.BCrypt.Verify(dto.PasswordHash,user.PasswordHash);

            if (passwordIsCorrect == false)
            {
                throw new Exception("Invalid email or password");
            }

            return GenerateToken(user);
        }


        // Admin creates a new system user
        public async Task<int> CreateUser(
            RegisterUserDto dto)
        {
            User? existingUser =await userRepo.GetUserByEmail(dto.Email);

            if (existingUser != null)
            {
                throw new Exception("The email already exists");
            }

            User user = new User
            {
                Email = dto.Email.Trim().ToLower(),

                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(dto.Password),

                Role = dto.Role,
                IsActive = true
            };

            await userRepo.AddUser(user);

            return user.UserID;
        }



        // Generate JWT token
        public string GenerateToken(User user)
        {
            List<Claim> claims =
                new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,user.UserID.ToString()),

                    new Claim(ClaimTypes.Email,user.Email),

                    new Claim(ClaimTypes.Role,user.Role)
                };

            string secretKey =config["Jwt:Key"]!;  //create and protect the JWT token.

            //gets the secret password used to secure the token
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            //converts the secret password into a security key.
            SigningCredentials credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                    issuer: config["Jwt:Issuer"],
                    audience: config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);  //converts the token object into a string that can be returned after login.
        }
    






}


}