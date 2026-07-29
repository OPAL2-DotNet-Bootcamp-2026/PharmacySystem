using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Supplier;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SupplierController : ControllerBase
    {
        private SupplierService supplierService;

        public SupplierController(SupplierService _supplierService)
        {
            supplierService = _supplierService;
        }


    
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAllSuppliers()
        {
            List<SupplierDto> suppliers = await supplierService.GetAllSuppliers();

            return Ok(suppliers);
        }


     
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            SupplierDto? supplier = await supplierService.GetSupplierById(id);

            if (supplier == null)
            {
                return NotFound(new
                {
                    message = $"Supplier with ID {id} was not found."
                });
            }

            return Ok(supplier);
        }


      
        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateSupplier(
            [FromBody] CreateSupplierDto dto)
        {
            int supplierId = await supplierService.CreateSupplier(dto);

            return Ok(new
            {
                message = "Supplier created successfully.",
                SupplierID = supplierId
            });
        }


        [HttpPut("Update/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateSupplier(int id,[FromBody] UpdateSupplierDto dto)
        {
            try
            {
                bool updated = await supplierService.UpdateSupplier(id, dto);

                if (!updated)
                {
                    return NotFound(new
                    {
                        message = $"Supplier with ID {id} was not found."
                    });
                }

                return Ok(new
                {
                    message = "Supplier updated successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }



        // Delete a supplier
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            bool deleted = await supplierService.DeleteSupplier(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"Supplier with ID {id} was not found."
                });
            }

            return Ok(new
            {
                message = "Supplier deleted successfully."
            });
        }


    }
}
