using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.CustomerOrder;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerOrderController : ControllerBase
    {
        private readonly CustomerOrderService customerOrderService;

        public CustomerOrderController( CustomerOrderService customerOrderService)
           
        {
            this.customerOrderService = customerOrderService;
        }

        // Returns all customer orders
        // GET: api/CustomerOrder
        [HttpGet]
        public async Task<IActionResult> GetAllCustomerOrders()
        {
            List<CustomerOrderDto> orders = await customerOrderService.GetAllCustomerOrders();
               

            return Ok(orders);
        }

        // Returns one customer order by ID
        // GET: api/CustomerOrder/1
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

        // Creates a new customer order
        // POST: api/CustomerOrder
        [HttpPost]
        public async Task<IActionResult> CreateCustomerOrder(  CreateCustomerOrderDto dto)
          
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

        // Updates customer order status
        // PUT: api/CustomerOrder/1/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateCustomerOrderStatus( int id,UpdateCustomerOrderStatusDto dto)
           
            
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

        // Deletes a customer order
        // DELETE: api/CustomerOrder/1
        [HttpDelete("{id}")]
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