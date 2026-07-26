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

           // Read JWT settings from appsettings.json
           public AuthService(IConfiguration _config)
            {
                config = _config;
            }


           // Create a JWT token for the user.
           public string GenerateToken(User user)
            {
                string secretKey =
                    config["JwtSettings:SecretKey"]!;

                string issuer =
                    config["JwtSettings:Issuer"]!;

                string audience =
                    config["JwtSettings:Audience"]!;

                int hours = int.Parse(
                    config["JwtSettings:ExpiryHours"]!);


               // Create the security key.
                 SymmetricSecurityKey key =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey));


                 // Sign the token.
                 SigningCredentials credentials =
                    new SigningCredentials(
                        key,
                        SecurityAlgorithms.HmacSha256);


            // User information inside the token.
                Claim[] claims =
                {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserID.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                new Claim(
                    ClaimTypes.Email,
                    user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };


                 // Create the token.
                   JwtSecurityToken token =
                    new JwtSecurityToken(
                        issuer: issuer,
                        audience: audience,
                        claims: claims,
                        expires: DateTime.UtcNow.AddHours(hours),
                        signingCredentials: credentials
                    );


                    // Return the token as a string.
                    return new JwtSecurityTokenHandler().WriteToken(token);
            }

        }
}