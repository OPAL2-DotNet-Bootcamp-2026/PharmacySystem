using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Pharmacy;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class PharmacyController : ControllerBase
    {
        private readonly PharmacyService pharmacyService;
        
        public PharmacyController(PharmacyService _pharmacyService)
        {
            pharmacyService = _pharmacyService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(pharmacyService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var pharmacy = pharmacyService.GetById(id);

            if (pharmacy == null)
                return NotFound();

            return Ok(pharmacy);
        }

        [HttpPost]
        public IActionResult Add(CreatePharmacyDto dto)
        {
            var pharmacy = pharmacyService.Add(dto);

            return CreatedAtAction(nameof(GetById), new { id = pharmacy.PharmacyID }, pharmacy);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdatePharmacyDto dto)
        {
            var pharmacy = pharmacyService.Update(id, dto);

            if (pharmacy == null)
                return NotFound();

            return Ok(pharmacy);
        }
    }
}
