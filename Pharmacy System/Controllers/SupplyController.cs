using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Supply;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplyController : ControllerBase
    {
        private readonly SupplyService supplyService;

        public SupplyController(SupplyService supplyService)
        {
            this.supplyService = supplyService;
        }

        // GET: api/Supply
        [HttpGet]
        public async Task<IActionResult> GetAllSupplies()
        {
            List<SupplyDto> supplies = await supplyService.GetAllSupplies();
            return Ok(supplies);   

            
        }

        // GET: api/Supply/1
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

        // POST: api/Supply
        [HttpPost]
        public async Task<IActionResult> CreateSupply(CreateSupplyDto dto)
            
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

        // PUT: api/Supply/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupply(int id, UpdateSupplyDto dto)
            
           
        {
            try
            {
                bool updated =
                    await supplyService.UpdateSupply(id, dto);

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

        // DELETE: api/Supply/1
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteSupply(int id)
        //{
        //    bool deleted =
        //        await supplyService.DeleteSupply(id);

        //    if (!deleted)
        //    {
        //        return NotFound("Supply not found");
        //    }

        //    return Ok("Supply deleted successfully");
        }
    }
}