using Pharmacy_System.Repos;
using Pharmacy_System.Modules;
using Pharmacy_System.DTOs.Customer;

namespace Pharmacy_System.Services
{
    public class CustomerService
    {
        private readonly CustomerRepo customerRepo;

        public CustomerService(CustomerRepo _customerRepo)
        {
            customerRepo = _customerRepo;
        }

        // Get All Customers
        public List<CustomerDto> GetAll()
        {
            List<Customer> customers = customerRepo.GetAllCustomer();

            List<CustomerDto> reponse = new List<CustomerDto>();

            foreach (var customer in customers)
            {
                reponse.Add(new CustomerDto
                {
                    CustomerID = customer.CustomerID,
                    FullName = customer.FullName,
                    Phone = customer.Phone,
                    DOB = customer.DOB,
                });
            }

            return reponse;
        }

        // Get Customer By Id
        public CustomerDto GetById(int id)
        {
            Customer customer = customerRepo.GetCustomerById(id);
            if (customer == null)
                return null;

            return new CustomerDto
            {
                CustomerID = customer.CustomerID,
                FullName = customer.FullName,
                Phone = customer.Phone,
                DOB = customer.DOB
            };
        }

        // Add Customer
        public CustomerDto Add(CustomerDto dto)
        {
            Customer customer = new Customer
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                DOB = dto.DOB
            };

            customerRepo.Add(customer);

            return new CustomerDto
            {
                CustomerID = dto.CustomerID,
                FullName = dto.FullName,
                Phone = dto.Phone,
                DOB = dto.DOB
            };
        }

        // Update Customer
        public CustomerDto Update(int id, UpdateCustomerDto dto)
        {
            Customer customer = customerRepo.GetCustomerById(id);

            if (customer == null)
                return null;

            customer.FullName = dto.FullName;
            customer.Phone = dto.Phone;
            customer.DOB = dto.DOB;

            customerRepo.CustomerUpdate();

            return new CustomerDto
            {
                CustomerID = customer.CustomerID,
                FullName = dto.FullName,
                Phone = dto.Phone,
                DOB = dto.DOB
            };
        }
    }
}
