using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Pharmacist;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PharmacistController : ControllerBase
    {
        private readonly PharmacistService pharmacistService;

        public PharmacistController(PharmacistService pharmacistService)
        {
            this.pharmacistService = pharmacistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await pharmacistService.GetAll());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)   // was GetByid — fix the name
        {
            PharmacistDto? pharmacist = await pharmacistService.GetById(id);
            if (pharmacist == null) return NotFound();
            return Ok(pharmacist);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]                       // admin creates profile + User account
        public async Task<IActionResult> Add(CreatePharmacistDto dto)
        {
            PharmacistDto? pharmacist = await pharmacistService.Add(dto);
            if (pharmacist == null)
                return Conflict("A user with this email already exists");   // service returns null on duplicate
            return CreatedAtAction(nameof(GetById), new { id = pharmacist.PharmacistID }, pharmacist);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePharmacistDto dto)
        {
            PharmacistDto? pharmacist = await pharmacistService.Update(id, dto);
            if (pharmacist == null) return NotFound();
            return Ok(pharmacist);
        }

        [HttpDelete("{id}")]                               // soft delete
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await pharmacistService.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("by-pharmacy/{pharmacyId}")]              // staff of one branch
        public async Task<IActionResult> GetByPharmacy(int pharmacyId)
            => Ok(await pharmacistService.GetByPharmacy(pharmacyId));

        [HttpGet("search")]                                // ?name=sara
        public async Task<IActionResult> SearchByName([FromQuery] string name)
            => Ok(await pharmacistService.SearchByName(name));
    }
}