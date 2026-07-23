using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.PharmacistOrder;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PharmacistOrderController : ControllerBase
    {
        private readonly PharmacistOrderService pharmacistOrderService;

        public PharmacistOrderController(PharmacistOrderService pharmacistOrderService)
            
        {
            this.pharmacistOrderService = pharmacistOrderService;
        }

        // Returns all pharmacist orders
        // GET: api/PharmacistOrder
        [HttpGet]
        public async Task<IActionResult> GetAllPharmacistOrders()
        {
            List<PharmacistOrderDto> orders =await pharmacistOrderService.GetAllPharmacistOrders();
                

            return Ok(orders);
        }

        // Returns one pharmacist order by ID
        // GET: api/PharmacistOrder/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPharmacistOrderById(int id)
        {
            PharmacistOrderDto? order =  await pharmacistOrderService.GetPharmacistOrderById(id);
              

            if (order == null)
            {
                return NotFound("Pharmacist order not found");
            }

            return Ok(order);
        }

        // Creates a new pharmacist order
        // POST: api/PharmacistOrder
        [HttpPost]
        public async Task<IActionResult> CreatePharmacistOrder(CreatePharmacistOrderDto dto)
            
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

        // Updates pharmacist order status
        // PUT: api/PharmacistOrder/1/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdatePharmacistOrderStatus( int id, UpdatePharmacistOrderDto dto)
           
           
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

        // Deletes a pharmacist order
        // DELETE: api/PharmacistOrder/1
        [HttpDelete("{id}")]
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