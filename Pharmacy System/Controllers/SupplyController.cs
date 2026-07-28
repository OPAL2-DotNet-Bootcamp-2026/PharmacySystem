using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Supply;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class SupplyController : ControllerBase
    {
        private readonly SupplyService supplyService;

        public SupplyController(SupplyService supplyService)
        {
            this.supplyService = supplyService;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> GetAllSupplies()
        {
            List<SupplyDto> supplies = await supplyService.GetAllSupplies();
            return Ok(supplies);   

            
        }

        // GET
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplyById(int id)
        {
            SupplyDto? supply =await supplyService.GetSupplyById(id);
                

            if (supply == null)
            {
                return NotFound("Supply not found");
            }

            return Ok(supply);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> CreateSupply([FromBody] CreateSupplyDto dto)
            
        {
            try
            {
                int supplyId =await supplyService.CreateSupply(dto);
                    

                return Ok(new
                {
                    SupplyId = supplyId,
                    Message = "Supply created successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupply(int id, [FromBody] UpdateSupplyDto dto)
            
           
        {
            try
            {
                bool updated = await supplyService.UpdateSupply(id, dto);
                   

                if (!updated)
                {
                    return NotFound("Supply not found");
                }

                return Ok("Supply updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //DELETE
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteSupply(int id)
        {
            bool deleted =
                await supplyService.DeleteSupply(id);

            if (!deleted)
            {
                return NotFound("Supply not found");
            }

            return Ok("Supply deleted successfully");
        }
    }
}