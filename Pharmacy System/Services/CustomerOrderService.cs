using Pharmacy_System.DTOs.CustomerOrder;
using Pharmacy_System.DTOs.CustomerOrderDetail;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class CustomerOrderService
    {
        private readonly CustomerOrderRepo customerOrderRepo;
        private readonly CustomerRepo customerRepo;
        private readonly PharmacyRepo pharmacyRepo;
        private readonly MedicineRepo medicineRepo;

        public CustomerOrderService(
            CustomerOrderRepo customerOrderRepo,
            CustomerRepo customerRepo,
            PharmacyRepo pharmacyRepo,
            MedicineRepo medicineRepo)
        {
            this.customerOrderRepo = customerOrderRepo;
            this.customerRepo = customerRepo;
            this.pharmacyRepo = pharmacyRepo;
            this.medicineRepo = medicineRepo;
        }

        // Returns all customer orders as DTOs
        public async Task<List<CustomerOrderDto>> GetAllCustomerOrders()
        {
            List<CustomerOrder> orders =await customerOrderRepo.GetAllCustomerOrders();
                

            List<CustomerOrderDto> orderDtos = new List<CustomerOrderDto>();

            foreach (CustomerOrder order in orders)
            {
                CustomerOrderDto dto = ConvertToDto(order);
                orderDtos.Add(dto);
            }

            return orderDtos;
        }

        // Returns one customer order using its ID
        public async Task<CustomerOrderDto?> GetCustomerOrderById(int id)
        {
            CustomerOrder? order = await customerOrderRepo.GetCustomerOrderById(id);
               

            if (order == null)
            {
                return null;
            }

            return ConvertToDto(order);
        }

        // Creates a new customer order
        public async Task<int> CreateCustomerOrder(CreateCustomerOrderDto dto)
            
        {
            // Check that the customer exists
            Customer? customer =await customerRepo.GetCustomerById(dto.CustomerID);
                

            if (customer == null)
            {
                throw new Exception("Customer not found");
            }

            // Check that the pharmacy exists
            Pharmacy? pharmacy =await pharmacyRepo.GetPharmacyById(dto.PharmacyID);
                

            if (pharmacy == null)
            {
                throw new Exception("Pharmacy not found");
            }

            // Check that the order contains at least one medicine
            if (dto.OrderDetails == null ||dto.OrderDetails.Count == 0)
                
            {
                throw new Exception("The order must contain at least one medicine");      
                
            }

            // Create the main customer order
            CustomerOrder customerOrder =
                new CustomerOrder
                {
                    CustomerId = dto.CustomerID,
                    PharmacyId = dto.PharmacyID,
                    OrderDate = DateTime.Now,
                    TotalCost = 0,
                    Status = "Pending"
                };

            // Add every medicine to the order
            foreach (CreateCustomerOrderDetailDto detailDto in dto.OrderDetails)
                     
            {
                // Check that the medicine exists
                Medicine? medicine =await medicineRepo.GetMedicineById(detailDto.MedicineID);
                    
                    

                if (medicine == null)
                {
                    throw new Exception($"Medicine with ID {detailDto.MedicineID} not found" );
                        
                   
                }

                // Prevent adding the same medicine twice
                bool medicineAlreadyAdded =customerOrder.CustomerOrderDetails.Any( d => d.MedicineID ==detailDto.MedicineID );
                  

                if (medicineAlreadyAdded)
                {
                    throw new Exception("The same medicine cannot be added twice" );
                        
                   
                }

                // Calculate subtotal
                decimal subtotal =detailDto.Quantity * medicine.UnitPrice;
                    

                // Create one order detail
                CustomerOrderDetail orderDetail =
                    new CustomerOrderDetail
                    {
                        MedicineID =detailDto.MedicineID,
                        Quantity = detailDto.Quantity,
                        UnitPrice =medicine.UnitPrice,
                        Subtotal =subtotal
                            
                    };

                // Add detail to the order
                customerOrder.CustomerOrderDetails.Add( orderDetail );
              
                // Add subtotal to total cost
                customerOrder.TotalCost += subtotal;
            }

            // Save order and details
            await customerOrderRepo.Add(customerOrder);

            return customerOrder.CustomerOrderId;
        }

        // Updates customer order status
        public async Task<bool> UpdateCustomerOrderStatus( int id, UpdateCustomerOrderStatusDto dto)
      
        {
            CustomerOrder? order = await customerOrderRepo.GetCustomerOrderById(id);
               

            if (order == null)
            {
                return false;
            }

            // Completed order cannot be updated
            if (order.Status == "Completed")
            {
                throw new Exception("A completed order cannot be updated");
                    
                
            }

            // Cancelled order cannot be updated
            if (order.Status == "Cancelled")
            {
                throw new Exception("A cancelled order cannot be updated" );
           
            }

            string status = dto.Status.Trim();

            string[] allowedStatuses =
            {
                "Pending",
                "Completed",
                "Cancelled"
            };

            // Ignore capital and small letters
            string? correctStatus = allowedStatuses.FirstOrDefault(s => s.ToLower() == status.ToLower());
                    
           

            if (correctStatus == null)
            {
                throw new Exception("Status must be Pending, Completed or Cancelled"  );
                    
              
            }

            order.Status = correctStatus;

            await customerOrderRepo.CustomerOrderUpdate();

            return true;
        }

        // Deletes a customer order
        public async Task<bool> DeleteCustomerOrder(int id)
        {
            CustomerOrder? order =await customerOrderRepo.GetCustomerOrderById(id);
                

            // Return false if the order does not exist
            if (order == null)
            {
                return false;
            }

            // Only pending orders can be deleted
            if (order.Status != "Pending")
            {
                throw new Exception("Only pending customer orders can be deleted" );
                    
               
            }

            await customerOrderRepo.CustomerOrderDelete(order);

            return true;
        }


        // Converts CustomerOrder model into CustomerOrderDto
        private CustomerOrderDto ConvertToDto(
            CustomerOrder order)
        {
            CustomerOrderDto dto =
                new CustomerOrderDto
                {
                    CustomerOrderId =order.CustomerOrderId,
                      
                    CustomerID =order.CustomerId,
                    
                    FullName =order.Customer.FullName,
                        
                    PharmacyID = order.PharmacyId,
                  
                    PharmacyName = order.Pharmacy.PharmacyName,
                   
                    OrderDate = order.OrderDate,
     
                    TotalCost =order.TotalCost,
                        
                    Status =  order.Status
                      
                };

            // Convert order details into DTOs
            foreach (CustomerOrderDetail detail in order.CustomerOrderDetails)
                    
            {
                CustomerOrderDetailDto detailDto =
                    new CustomerOrderDetailDto
                    {
                        MedicineID =detail.MedicineID,

                        MedicineName =detail.Medicine.MedicineName,

                        Quantity = detail.Quantity,
 
                        UnitPrice =detail.UnitPrice,
 
                        Subtotal =detail.Subtotal
                            
                    };

                dto.OrderDetails.Add(detailDto);
            }

            return dto;
        }
    }
}