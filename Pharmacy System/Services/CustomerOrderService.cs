using Pharmacy_System.DTOs.CustomerOrder;
using Pharmacy_System.DTOs.CustomerOrderDetail;
using Pharmacy_System.Models;
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
        private readonly PharmacyStockService pharmacyStockService;
        private readonly PharmacyContext context;

        public CustomerOrderService(
            CustomerOrderRepo customerOrderRepo,
            CustomerRepo customerRepo,
            PharmacyRepo pharmacyRepo,
            MedicineRepo medicineRepo,
            PharmacyStockService pharmacyStockService,
            PharmacyContext context)
        {
            this.customerOrderRepo = customerOrderRepo;
            this.customerRepo = customerRepo;
            this.pharmacyRepo = pharmacyRepo;
            this.medicineRepo = medicineRepo;
            this.pharmacyStockService = pharmacyStockService;
            this.context = context;
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

        // Creates new customer order
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

            if (dto.OrderDetails == null ||dto.OrderDetails.Count == 0)
                
            {
                throw new Exception("The order must contain at least one medicine");      
                
            }

            CustomerOrder customerOrder =
                new CustomerOrder
                {
                    CustomerId = dto.CustomerID,
                    PharmacyId = dto.PharmacyID,
                    OrderDate = DateTime.Now,
                    TotalCost = 0,
                    Status = "Pending"
                };

            using var tx = await context.Database.BeginTransactionAsync();

            // Add  medicine to the order
            foreach (CreateCustomerOrderDetailDto detailDto in dto.OrderDetails)
                     
            {
                Medicine? medicine =await medicineRepo.GetMedicineById(detailDto.MedicineID);
                    
                    

                if (medicine == null)
                {
                    throw new Exception($"Medicine with ID {detailDto.MedicineID} not found" );
                        
                   
                }

              

                PharmacyStock? pharmacyStock = await pharmacyStockService.GetStock(  dto.PharmacyID,detailDto.MedicineID);

         
                        
                if (pharmacyStock == null)
                {
                    throw new Exception( $"Medicine {medicine.MedicineName} does not exist in pharmacy stock");
                       
                }

                if (pharmacyStock.Quantity < detailDto.Quantity)
                {
                    throw new Exception( $"Not enough pharmacy stock for {medicine.MedicineName}");
                       
                }

                decimal subtotal =detailDto.Quantity * medicine.UnitPrice;
                    

                CustomerOrderDetail orderDetail =
                    new CustomerOrderDetail
                    {
                        MedicineID =detailDto.MedicineID,
                        Quantity = detailDto.Quantity,
                        UnitPrice =medicine.UnitPrice,
                        Subtotal =subtotal
                            
                    };

                customerOrder.CustomerOrderDetails.Add( orderDetail );
              
                customerOrder.TotalCost += subtotal;

            }
           


            foreach (CustomerOrderDetail detail
                     in customerOrder.CustomerOrderDetails)
            {
                await pharmacyStockService.Decrease(
                    customerOrder.PharmacyId,
                    detail.MedicineID,
                    detail.Quantity);
            }



            // Save 
            await customerOrderRepo.Add(customerOrder);

            await tx.CommitAsync();

            return customerOrder.CustomerOrderId;
        }

        // Updates 
        public async Task<bool> UpdateCustomerOrderStatus( int id, UpdateCustomerOrderStatusDto dto)
      
        {
            CustomerOrder? order = await customerOrderRepo.GetCustomerOrderById(id);
               

            if (order == null)
            {
                return false;
            }

            if (order.Status == "Completed")
            {
                throw new Exception("A completed order cannot be updated");
                    
                
            }

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

            string? correctStatus = allowedStatuses.FirstOrDefault(s => s.ToLower() == status.ToLower());
                    
           

            if (correctStatus == null)
            {
                throw new Exception("Status must be Pending, Completed or Cancelled"  );
                    
              
            }

            order.Status = correctStatus;

            await customerOrderRepo.CustomerOrderUpdate();

            return true;
        }

        // Deletes
        public async Task<bool> DeleteCustomerOrder(int id)
        {
            CustomerOrder? order =await customerOrderRepo.GetCustomerOrderById(id);
                

            if (order == null)
            {
                return false;
            }

            if (order.Status != "Pending")
            {
                throw new Exception("Only pending customer orders can be deleted" );
                    
               
            }

            await customerOrderRepo.CustomerOrderDelete(order);

            return true;
        }


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