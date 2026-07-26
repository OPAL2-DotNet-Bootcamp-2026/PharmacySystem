using Pharmacy_System.Repos;
using Pharmacy_System.Modules;
using Pharmacy_System.DTOs.Customer;
using Pharmacy_System.DTOs.CustomerOrder;

namespace Pharmacy_System.Services
{
    public class CustomerService
    {
        private readonly CustomerRepo customerRepo;

        public CustomerService(CustomerRepo _customerRepo)
        {
            customerRepo = _customerRepo;
        }

        private static CustomerDto ToDto(Customer c) => new CustomerDto
        {
            CustomerID = c.CustomerID,
            FullName = c.FullName,
            Phone = c.Phone,
            Email = c.Email,
            DOB = c.DOB,
            IsActive = c.IsActive
        };

        public async Task<List<CustomerDto>> GetAll()
        {
            List<Customer> customers = await customerRepo.GetAllCustomer();

            return customers.Select(ToDto).ToList();
        }
        
        public async Task<CustomerDto?> GetById(int id)
        {
            Customer? customer = await customerRepo.GetCustomerById(id);
            return customer == null ? null : ToDto(customer);
        }

        public async Task<CustomerDto> Add(CreateCustomerDto dto)
        {
            Customer customer = new Customer
            {
                FullName = dto.FullName,
                Phone    = dto.Phone,
                Email    = dto.Email,
                DOB      = dto.DOB!.Value
            };

            await customerRepo.Add(customer);
            return ToDto(customer);
        }

        public async Task<CustomerDto?> Update(int id, UpdateCustomerDto dto)
        {
            Customer? customer = await customerRepo.GetCustomerById(id);
            if(customer == null)
                return null;

            customer.FullName = dto.FullName;
            customer.Phone    = dto.Phone;
            customer.Email    = dto.Email;
            customer.DOB      = dto.DOB!.Value;

            await customerRepo.CustomerUpdate();
            return ToDto(customer);
        }

        public async Task<bool> Delete(int id)
        {
            Customer? customer = await customerRepo.GetCustomerById(id);
            if(customer == null)
                return false;

            await customerRepo.CustomerDelete(customer);
            return true;
        }

        public async Task<List<CustomerDto>> SearchByName(string name)
        {
            List<Customer> customers = await customerRepo.GetCustomerByName(name);
            return customers.Select(ToDto).ToList();
        }

        public async Task<CustomerDto?> GetByPhone(string phone)
        {
            Customer? customer = await customerRepo.GetCustomerByPhone(phone);
            return customer == null ? null : ToDto(customer);
        }

        public async Task<List<CustomerOrderDto>> GetCustomerOrders(int id)
        {
            List<CustomerOrder> orders = await customerRepo.GetCustomerOrdersById(id);
            return orders.Select(o => new CustomerOrderDto
            {
                CustomerOrderId = o.CustomerOrderId,
                CustomerID      = o.CustomerId,
                PharmacyID      = o.PharmacyId,
                OrderDate       = o.OrderDate,
                TotalCost       = o.TotalCost,
                Status          = o.Status
            }).ToList();
        }        
    }
}
