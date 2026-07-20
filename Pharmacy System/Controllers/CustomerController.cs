using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Customer;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService customerService;
        
        public CustomerController(CustomerService _customerService)
        {
            customerService = _customerService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(customerService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var customer = customerService.GetById(id);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public IActionResult Add(CreateCustomerDto dto)
        {
            var customer = customerService.Add(dto);

            return CreatedAtAction(nameof(GetById), new { id = customer.CustomerID }, customer);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateCustomerDto dto)
        {
            var customer = customerService.Update(id, dto);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }
    }
}
