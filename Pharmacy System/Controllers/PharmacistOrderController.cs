using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.PharmacistOrder;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class PharmacistOrderController : ControllerBase
    {
        private readonly PharmacistOrderService pharmacistOrderService;

        public PharmacistOrderController(PharmacistOrderService pharmacistOrderService)
            
        {
            this.pharmacistOrderService = pharmacistOrderService;
        }

        // GET
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Pharmacist")]

        public async Task<IActionResult> GetAllPharmacistOrders()
        {
            List<PharmacistOrderDto> orders =await pharmacistOrderService.GetAllPharmacistOrders();
                

            return Ok(orders);
        }

        // GET
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Pharmacist")]

        public async Task<IActionResult> GetPharmacistOrderById(int id)
        {
            PharmacistOrderDto? order =  await pharmacistOrderService.GetPharmacistOrderById(id);
              

            if (order == null)
            {
                return NotFound("Pharmacist order not found");
            }

            return Ok(order);
        }

        // POST
        [HttpPost]
        [Authorize(Roles = "Admin,Pharmacist")]

        public async Task<IActionResult> CreatePharmacistOrder([FromBody] CreatePharmacistOrderDto dto)
            
        {
            try
            {
                int orderId = await pharmacistOrderService .CreatePharmacistOrder(dto);
                   
                       

                return Ok(new
                {
                    PharmacistOrderId = orderId,
                    Message = "Pharmacist order created successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Manager")]

        public async Task<IActionResult> UpdatePharmacistOrderStatus( int id, [FromBody] UpdatePharmacistOrderDto dto)
           
           
        {
            try
            {
                bool updated =await pharmacistOrderService.UpdatePharmacistOrderStatus(id, dto);
                    
                        

                if (!updated)
                {
                    return NotFound("Pharmacist order not found");
                }

                return Ok("Pharmacist order status updated successfully");
                    
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeletePharmacistOrder(int id)
        {
            try
            {
                bool deleted = await pharmacistOrderService.DeletePharmacistOrder(id);
                   
                        

                if (!deleted)
                {
                    return NotFound("Pharmacist order not found");
                }

                return Ok("Pharmacist order deleted successfully"  );
                    
              
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}