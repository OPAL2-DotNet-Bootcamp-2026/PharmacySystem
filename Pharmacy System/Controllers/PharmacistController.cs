using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.Modules;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("pharmacist")]
    public class PharmacistController : ControllerBase
    {
        private PharmacistService pharmacistService;

        public PharmacistController(PharmacistService _pharmacistService)
        {
            pharmacistService = _pharmacistService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            PharmacistResponseDto created = pharmacistService.Register(dto);
        }
    }
}
