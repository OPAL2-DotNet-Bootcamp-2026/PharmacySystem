using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Customer;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    
    [ApiController, Route("api/[controller]"), Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService customerService;

        public CustomerController(CustomerService customerService)
        {
            this.customerService = customerService;
        }

        [HttpGet]                                          // GET api/Customer
        public async Task<IActionResult> GetAll()
            => Ok(await customerService.GetAll());

        [HttpGet("{id}")]                                  // GET api/Customer/5
        public async Task<IActionResult> GetById(int id)
        {
            CustomerDto? customer = await customerService.GetById(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpPost]                                         // POST api/Customer
        public async Task<IActionResult> Add(CreateCustomerDto dto)
        {
            CustomerDto customer = await customerService.Add(dto);
            return CreatedAtAction(nameof(GetById), new { id = customer.CustomerID }, customer);
        }

        [HttpPut("{id}")]                                  // PUT api/Customer/5
        public async Task<IActionResult> Update(int id, UpdateCustomerDto dto)
        {
            CustomerDto? customer = await customerService.Update(id, dto);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpDelete("{id}")]                               // DELETE api/Customer/5  (soft)
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await customerService.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("search")]                                // GET api/Customer/search?name=sara
        public async Task<IActionResult> SearchByName([FromQuery] string name)
            => Ok(await customerService.SearchByName(name));

        [HttpGet("by-phone")]                              // GET api/Customer/by-phone?phone=...
        public async Task<IActionResult> GetByPhone([FromQuery] string phone)
        {
            CustomerDto? customer = await customerService.GetByPhone(phone);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpGet("{id}/orders")]                           // GET api/Customer/5/orders
        public async Task<IActionResult> GetCustomerOrders(int id)
            => Ok(await customerService.GetCustomerOrders(id));
    }
}