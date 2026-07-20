using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Pharmacist;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class PharmacistController : ControllerBase
    {
        private readonly PharmacistService pharmacistService;

        public PharmacistController(PharmacistService _pharmacistService)
        {
            pharmacistService = _pharmacistService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(pharmacistService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetByid(int id)
        {
            var pharmacist = pharmacistService.GetById(id);

            if (pharmacist == null)
                return NotFound();

            return Ok(pharmacist);
        }

        [HttpPost]
        public IActionResult Add(CreatePharmacistDto dto)
        {
            var pharmacist = pharmacistService.Add(dto);

            return CreatedAtAction(nameof(GetByid), new { id = pharmacist.PharmacistID }, pharmacist); 
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdatePharmacistDto dto)
        {
            var pharmacist = pharmacistService.Update(id, dto);

            if (pharmacist == null)
                return NotFound();

            return Ok(pharmacist);
        }
    }
}
