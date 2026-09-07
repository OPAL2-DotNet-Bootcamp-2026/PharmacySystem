using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.User;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;


        public UserController(
            UserService _userService
        )
        {
            userService = _userService;
        }


        // =====================================
        // CREATE USER
        // =====================================

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateUser(
            RegisterUserDto dto
        )
        {
            bool emailExists =
                await userService.EmailExists(
                    dto.Email
                );


            if (emailExists)
            {
                return BadRequest(
                    new
                    {
                        message =
                            "This email is already used."
                    }
                );
            }


            bool usernameExists =
                await userService.UsernameExists(
                    dto.Username
                );


            if (usernameExists)
            {
                return BadRequest(
                    new
                    {
                        message =
                            "This username is already used."
                    }
                );
            }


            UserResponseDto? user =
                await userService.CreateUser(
                    dto
                );


            if (user == null)
            {
                return BadRequest(
                    new
                    {
                        message =
                            "Could not create user."
                    }
                );
            }


            return Ok(user);
        }


        // =====================================
        // LOGIN
        // =====================================

        [HttpPost("login")]
        [AllowAnonymous]

        public async Task<IActionResult> Login(
            LoginDto dto
        )
        {
            LoginResponseDto? result =
                await userService.Login(
                    dto
                );


            if (result == null)
            {
                return Unauthorized(
                    new
                    {
                        message =
                            "Invalid email or password."
                    }
                );
            }


            return Ok(result);
        }


        // =====================================
        // GET ALL USERS
        // =====================================

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetAllUsers()
        {
            var users =
                await userService.GetAllUsers();


            return Ok(users);
        }


        // =====================================
        // GET USER BY ID
        // =====================================

        [HttpGet("GetUserById/{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetUserById(
            int id
        )
        {
            var user =
                await userService.GetUserById(
                    id
                );


            if (user == null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "User not found."
                    }
                );
            }


            return Ok(user);
        }


        // =====================================
        // GET USER BY EMAIL
        // =====================================

        [HttpGet("GetUserByEmail/{email}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetUserByEmail(
            string email
        )
        {
            var user =
                await userService.GetUserByEmail(
                    email
                );


            if (user == null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "User not found."
                    }
                );
            }


            return Ok(user);
        }


        // =====================================
        // DELETE USER
        // =====================================

        [HttpDelete("DeleteUser/{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteUser(
            int id
        )
        {
            bool deleted =
                await userService.UserDelete(
                    id
                );


            if (!deleted)
            {
                return NotFound(
                    new
                    {
                        message =
                            "User not found."
                    }
                );
            }


            return Ok(
                new
                {
                    message =
                        "User deleted successfully."
                }
            );
        }
    }
}