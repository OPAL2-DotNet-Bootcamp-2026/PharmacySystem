using Pharmacy_System.DTOs.PharmacistOrder;
using Pharmacy_System.DTOs.PharmacistOrderDetail;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class PharmacistOrderService
    {
        private readonly PharmacistOrderRepo pharmacistOrderRepo;
        private readonly PharmacistRepo pharmacistRepo;
        private readonly PharmacyRepo pharmacyRepo;
        private readonly MedicineRepo medicineRepo;
        private readonly EmailService emailService;
        private readonly ILogger<PharmacistOrderService> logger;

        public PharmacistOrderService(
            PharmacistOrderRepo pharmacistOrderRepo,
            PharmacistRepo pharmacistRepo,
            PharmacyRepo pharmacyRepo,
            MedicineRepo medicineRepo,
            EmailService emailService,
            ILogger<PharmacistOrderService> logger)
        {
            this.pharmacistOrderRepo = pharmacistOrderRepo;
            this.pharmacistRepo = pharmacistRepo;
            this.pharmacyRepo = pharmacyRepo;
            this.medicineRepo = medicineRepo;
            this.emailService = emailService;
            this.logger = logger;
        }

        // Returns all pharmacist orders 
        public async Task<List<PharmacistOrderDto>> GetAllPharmacistOrders()
        {
            List<PharmacistOrder> orders =await pharmacistOrderRepo.GetAllPharmacistOrders();         

            List<PharmacistOrderDto> orderDtos =new List<PharmacistOrderDto>();
                

            foreach (PharmacistOrder order in orders)
            {
                PharmacistOrderDto dto = ConvertToDto(order);
                orderDtos.Add(dto);
            }

            return orderDtos;
        }

        // Returns one pharmacist order using  ID
        public async Task<PharmacistOrderDto?> GetPharmacistOrderById(int id)
        {
            PharmacistOrder? order =await pharmacistOrderRepo.GetPharmacistOrderById(id);
                

            if (order == null)
            {
                return null;
            }

            return ConvertToDto(order);
        }

        // Creates  new pharmacist order
        public async Task<int> CreatePharmacistOrder(CreatePharmacistOrderDto dto)
            
        {
            // Check that the pharmacist exists
            Pharmacist? pharmacist = await pharmacistRepo.GetPharmacistById(dto.PharmacistID);
               

            if (pharmacist == null)
            {
                throw new Exception("Pharmacist not found");
            }

            // Check that the pharmacy exists
            Pharmacy? pharmacy =await pharmacyRepo.GetPharmacyById(dto.PharmacyID);
                

            if (pharmacy == null)
            {
                throw new Exception("Pharmacy not found");
            }

            // Check that the pharmacist works in the selected pharmacy
            if (pharmacist.PharmacyID != dto.PharmacyID)
            {
                throw new Exception("The pharmacist does not belong to this pharmacy");
                    
                
            }

            // Check that the order contains at least one medicine
            if (dto.OrderDetails == null ||dto.OrderDetails.Count == 0)
                
            {
                throw new Exception("The order must contain at least one medicine");
                    
                
            }

            PharmacistOrder pharmacistOrder = new PharmacistOrder
               
                {
                    PharmacistID = dto.PharmacistID,
                    PharmacyID = dto.PharmacyID,

                    OrderDate = DateTime.Now,

                    TotalCost = 0,

                    // Default status
                    Status = "Pending"
                };

            // Add every medicine to  order
            foreach (CreatePharmacistOrderDetailDto detailDto in dto.OrderDetails)
                    
            {
                // Check  medicine exists
                Medicine? medicine =  await medicineRepo.GetMedicineById( detailDto.MedicineID );
                  
                if (medicine == null)
                {
                    throw new Exception(  $"Medicine with ID {detailDto.MedicineID} not found" );
                      
                   
                }

               

                // Calculate  cost of  medicine
                decimal subtotal = detailDto.Quantity * medicine.UnitPrice;
                   

                // Create one pharmacist order detail
                PharmacistOrderDetail orderDetail =new PharmacistOrderDetail
                    
                    {
                        MedicineID = detailDto.MedicineID,
                        Quantity = detailDto.Quantity
                    };

                // Add detail order
                pharmacistOrder.PharmacistOrderDetails.Add( orderDetail);
                   
          
                // Add subtotal to total order cost
                pharmacistOrder.TotalCost += subtotal;
            }

          
            await pharmacistOrderRepo.Add(pharmacistOrder);

            return pharmacistOrder.PharmacistOrderId;
        }

        // Updates pharmacist order status
        public async Task<bool> UpdatePharmacistOrderStatus(int id, UpdatePharmacistOrderDto dto)


        {
            PharmacistOrder? order = await pharmacistOrderRepo.GetPharmacistOrderById(id);


            if (order == null)
            {
                return false;
            }

           

            if (order.Status == "Approved")
            {
                throw new Exception("An approved order cannot be updated");


            }

            if (order.Status == "Cancelled")
            {
                throw new Exception("A cancelled order cannot be updated");


            }

            string status = dto.Status.Trim();
            string[] allowedStatuses =
            {
               "Pending",
               "Approved",
               "Cancelled",
               "Completed"
             };

            // Find the correct status 
            string? correctStatus = allowedStatuses.FirstOrDefault(s => s.ToLower() == status.ToLower());


            if (correctStatus == null)
            {
                throw new Exception("Status must be Pending, Approved, Cancelled or Completed");


            }

            order.Status = correctStatus;

            await pharmacistOrderRepo.PharmacistOrderUpdate();

            logger.LogInformation("Order {OrderId} status changed to {Status}",order.PharmacistOrderId, correctStatus);


           


            // Send email only when the order is approved
            if (correctStatus == "Approved")
            {
                await emailService.SendAsync(
                    order.Pharmacist.Email,
                    "Order Approved",
                    $"Your pharmacist order number {order.PharmacistOrderId} has been approved.");

            }
            return true;
        }



        public async Task<bool> DeletePharmacistOrder(int id)
        {
            PharmacistOrder? order =await pharmacistOrderRepo.GetPharmacistOrderById(id);
                

            // Return false if the order does not exist
            if (order == null)
            {
                return false;
            }

            // Only pending orders can be deleted
            if (order.Status != "Pending")
            {
                throw new Exception("Only pending pharmacist orders can be deleted" );

            }

            await pharmacistOrderRepo.PharmacistOrderDelete(order);

            return true;
        }

        private PharmacistOrderDto ConvertToDto(
            PharmacistOrder order)
        {
            PharmacistOrderDto dto =
                new PharmacistOrderDto
                {
                    PharmacistOrderId = order.PharmacistOrderId,
                    PharmacistID = order.PharmacistID,                
                    FullName =order.Pharmacist.FullName,
                    PharmacyID =order.PharmacyID,
                    PharmacyName =order.Pharmacy.PharmacyName,
                    OrderDate = order.OrderDate,
                    TotalCost =order.TotalCost,
                    Status = order.Status
                       
                };

            foreach (PharmacistOrderDetail detail in order.PharmacistOrderDetails)
                     
            {
                PharmacistOrderDetailDto detailDto =
                    new PharmacistOrderDetailDto
                    {
                        MedicineID =detail.MedicineID,
                        MedicineName =detail.Medicine.MedicineName,
                        Quantity =detail.Quantity
                            
                    };

                dto.OrderDetails.Add(detailDto);
            }

            return dto;
        }
    }
}