using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.User;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class UserController : ControllerBase
    {
        private UserService userService;

        public UserController(UserService _userService)
        {
            userService = _userService;
        }



        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserDto dto)
        {
            UserResponseDto? created = await userService.CreateUser(dto);

            if (created == null)
            {
                return BadRequest(new
                {
                    message = "Email is already registered."
                });
            }

            return Ok(created);
        }



        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            LoginResponseDto? result = await userService.Login(dto);

            if (result == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password"
                });
            }

            return Ok(result);
        }


        // GET user/GetAllUsers 
        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            List<UserResponseDto> users =
                await userService.GetAllUsers();

            return Ok(users);
        }


        // GET user/GetUserData/3
        [HttpGet("GetUserData/{id}")]
        [Authorize(Roles = "Admin,Manager,Pharmacist")]
        public async Task<IActionResult> GetUserData(int id)
        {
            UserResponseDto? user = await userService.GetUserById(id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = $"User with ID {id} was not found."
                });
            }

            return Ok(user);
        }


        // DELETE user/DeleteUser/3 

        [HttpDelete("DeleteUser/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            bool deleted = await userService.UserDelete(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"User with ID {id} was not found"
                });
            }

            return Ok(new
            {
                message = "User deactivated successfully"
            });
        }
    }

}

