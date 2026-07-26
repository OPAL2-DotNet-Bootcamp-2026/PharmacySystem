using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Pharmacy;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PharmacyController : ControllerBase
    {
        private readonly PharmacyService pharmacyService;

        public PharmacyController(PharmacyService pharmacyService)
        {
            this.pharmacyService = pharmacyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await pharmacyService.GetAll());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            PharmacyDto? pharmacy = await pharmacyService.GetById(id);
            if (pharmacy == null) return NotFound();
            return Ok(pharmacy);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]                       // admin-only: create pharmacy
        public async Task<IActionResult> Add(CreatePharmacyDto dto)
        {
            PharmacyDto pharmacy = await pharmacyService.Add(dto);
            return CreatedAtAction(nameof(GetById), new { id = pharmacy.PharmacyID }, pharmacy);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]                       // admin-only: edit pharmacy
        public async Task<IActionResult> Update(int id, UpdatePharmacyDto dto)
        {
            PharmacyDto? pharmacy = await pharmacyService.Update(id, dto);
            if (pharmacy == null) return NotFound();
            return Ok(pharmacy);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]                       // admin-only, soft delete
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await pharmacyService.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("search")]                                // ?name=al
        public async Task<IActionResult> SearchByName([FromQuery] string name)
            => Ok(await pharmacyService.SearchByName(name));

        [HttpGet("by-location")]                           // ?location=muscat
        public async Task<IActionResult> GetByLocation([FromQuery] string location)
            => Ok(await pharmacyService.GetByLocation(location));

        [HttpGet("{id}/pharmacists")]                      // branch staff
        public async Task<IActionResult> GetPharmacists(int id)
            => Ok(await pharmacyService.GetPharmacists(id));

        [HttpGet("{id}/stock")]                            // current on-hand
        public async Task<IActionResult> GetStock(int id)
            => Ok(await pharmacyService.GetStock(id));
    }
}