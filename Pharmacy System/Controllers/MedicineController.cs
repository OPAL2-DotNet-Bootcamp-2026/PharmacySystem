using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Medicine;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require login
    public class MedicineController : ControllerBase
    {
        private readonly MedicineService medicineService;

        public MedicineController(MedicineService _medicineService)
        {
            medicineService = _medicineService;
        }


        // GET medicine/GetAll
        // Any authenticated user can view medicines
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            List<MedicineDto> medicines =
                await medicineService.GetAll();

            return Ok(medicines);
        }


        // GET medicine/GetById/3
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            MedicineDto? medicine =
                await medicineService.GetById(id);

            if (medicine == null)
            {
                return NotFound(new
                {
                    message = $"Medicine with ID {id} was not found."
                });
            }

            return Ok(medicine);
        }


        // GET medicine/GetAvailable
        [HttpGet("GetAvailable")]
        public async Task<IActionResult> GetAvailable()
        {
            List<MedicineDto> medicines =
                await medicineService.GetAvailable();

            return Ok(medicines);
        }


        // GET medicine/GetByCategory/2
        [HttpGet("GetByCategory/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            List<MedicineDto> medicines =
                await medicineService.GetByCategory(categoryId);

            return Ok(medicines);
        }


        // GET medicine/SearchByName?name=Panadol
        [HttpGet("SearchByName")]
        public async Task<IActionResult> SearchByName(
            [FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new
                {
                    message = "Medicine name is required."
                });
            }

            List<MedicineDto> medicines =
                await medicineService.SearchByName(name);

            return Ok(medicines);
        }


        // POST medicine/AddNewMedicine
        // Admin or Manager only
        [HttpPost("AddNewMedicine")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddNewMedicine(
            [FromBody] CreateMedicineDto dto)
        {
            int medicineId =
                await medicineService.AddNewMedicine(dto);

            return Ok(new
            {
                message = "Medicine added successfully.",
                MedicineID = medicineId
            });
        }


        // PUT medicine/Update/3
        // Admin or Manager only
        [HttpPut("Update/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateMedicineDto dto)
        {
            bool updated = await medicineService.Update(id, dto);

            if (!updated)
            {
                return NotFound(new
                {
                    message = $"Medicine with ID {id} was not found."
                });
            }

            return Ok(new
            {
                message = "Medicine updated successfully."
            });
        }


        // Change or Patch medicine availability  /ToggleAvailability/3
        // Admin or Manager only
        [HttpPatch("ToggleAvailability/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            bool changed = await medicineService.ToggleAvailability(id);

            if (!changed)
            {
                return NotFound(new
                {
                    message = $"Medicine with ID {id} was not found."
                });
            }

            return Ok(new
            {
                message = "Medicine availability changed successfully."
            });
        }


        // DELETE medicine/Delete/3
        // Admin only
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MedicineDelete(int id)
        {
            bool deleted = await medicineService.MedicineDelete(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"Medicine with ID {id} was not found."
                });
            }

            return Ok(new
            {
                message = "Medicine deleted successfully."
            });
        }
    }
}

