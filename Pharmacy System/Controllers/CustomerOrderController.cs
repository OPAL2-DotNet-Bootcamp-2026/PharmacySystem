using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.CustomerOrder;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Pharmacist")]

    public class CustomerOrderController : ControllerBase
    {
        private readonly CustomerOrderService customerOrderService;

        public CustomerOrderController( CustomerOrderService customerOrderService)
           
        {
            this.customerOrderService = customerOrderService;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> GetAllCustomerOrders()
        {
            List<CustomerOrderDto> orders = await customerOrderService.GetAllCustomerOrders();
               

            return Ok(orders);
        }

        // GET
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerOrderById(int id)
        {
            CustomerOrderDto? order =  await customerOrderService.GetCustomerOrderById(id);
              

            if (order == null)
            {
                return NotFound("Customer order not found");
            }

            return Ok(order);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> CreateCustomerOrder([FromBody] CreateCustomerOrderDto dto)
          
        {
            try
            {
                int orderId = await customerOrderService.CreateCustomerOrder(dto);
                   

                return Ok(new
                {
                    CustomerOrderId = orderId,
                    Message = "Customer order created successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateCustomerOrderStatus( int id, [FromBody] UpdateCustomerOrderStatusDto dto)
           
            
        {
            try
            {
                bool updated =  await customerOrderService  .UpdateCustomerOrderStatus(id, dto);

                      

                if (!updated)
                {
                    return NotFound("Customer order not found");
                }

                return Ok("Customer order status updated successfully" );
                    
               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

      
        // DELETE
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteCustomerOrder(int id)
        {
            try
            {
                bool deleted =await customerOrderService.DeleteCustomerOrder(id);
                    

                if (!deleted)
                {
                    return NotFound("Customer order not found");
                }

                return Ok("Customer order deleted successfully");
                    
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}