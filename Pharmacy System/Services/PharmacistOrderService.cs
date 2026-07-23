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

        public PharmacistOrderService(
            PharmacistOrderRepo pharmacistOrderRepo,
            PharmacistRepo pharmacistRepo,
            PharmacyRepo pharmacyRepo,
            MedicineRepo medicineRepo)
        {
            this.pharmacistOrderRepo = pharmacistOrderRepo;
            this.pharmacistRepo = pharmacistRepo;
            this.pharmacyRepo = pharmacyRepo;
            this.medicineRepo = medicineRepo;
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

        // Returns one pharmacist order using its ID
        public async Task<PharmacistOrderDto?> GetPharmacistOrderById(int id)
        {
            PharmacistOrder? order =await pharmacistOrderRepo.GetPharmacistOrderById(id);
                

            if (order == null)
            {
                return null;
            }

            return ConvertToDto(order);
        }

        // Creates a new pharmacist order
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

            // Create the main pharmacist order
            PharmacistOrder pharmacistOrder = new PharmacistOrder
               
                {
                    PharmacistID = dto.PharmacistID,
                    PharmacyID = dto.PharmacyID,

                    OrderDate = DateTime.Now,

                    // Calculated from order details
                    TotalCost = 0,

                    // Default status
                    Status = "Pending"
                };

            // Add every medicine to the order
            foreach (CreatePharmacistOrderDetailDto detailDto in dto.OrderDetails)
                    
            {
                // Check that the medicine exists
                Medicine? medicine =  await medicineRepo.GetMedicineById( detailDto.MedicineID );
                  
                if (medicine == null)
                {
                    throw new Exception(  $"Medicine with ID {detailDto.MedicineID} not found" );
                      
                   
                }

                // Prevent adding the same medicine twice
                bool medicineAlreadyAdded = pharmacistOrder.PharmacistOrderDetails.Any(d => d.MedicineID == detailDto.MedicineID );
                   
               
                if (medicineAlreadyAdded)
                {
                    throw new Exception("The same medicine cannot be added twice" );
                        
                   
                }

                // Calculate the cost of this medicine
                decimal subtotal = detailDto.Quantity * medicine.UnitPrice;
                   

                // Create one pharmacist order detail
                PharmacistOrderDetail orderDetail =new PharmacistOrderDetail
                    
                    {
                        MedicineID = detailDto.MedicineID,
                        Quantity = detailDto.Quantity
                    };

                // Add detail to the order
                pharmacistOrder.PharmacistOrderDetails.Add( orderDetail);
                   
          
                // Add subtotal to the total order cost
                pharmacistOrder.TotalCost += subtotal;
            }

            // Save the order and its details
            await pharmacistOrderRepo.Add(pharmacistOrder);

            return pharmacistOrder.PharmacistOrderId;
        }

        // Updates pharmacist order status
        public async Task<bool> UpdatePharmacistOrderStatus(int id, UpdatePharmacistOrderDto dto)
            
           
        {
            PharmacistOrder? order =await pharmacistOrderRepo.GetPharmacistOrderById(id);
                

            if (order == null)
            {
                return false;
            }

            // Approved order cannot be changed
            if (order.Status == "Approved")
            {
                throw new Exception( "An approved order cannot be updated");
                   
                
            }

            // Cancelled order cannot be changed
            if (order.Status == "Cancelled")
            {
                throw new Exception("A cancelled order cannot be updated" );
                    
            
            }

            string status = dto.Status.Trim();

            string[] allowedStatuses =
            {
                "Pending",
                "Approved",
                "Cancelled"
            };

            // Find the correct status and ignore capital/small letters
            string? correctStatus =allowedStatuses.FirstOrDefault(s => s.ToLower() == status.ToLower() );
                
      
            if (correctStatus == null)
            {
                throw new Exception("Status must be Pending, Approved or Cancelled" );
  
            }

            order.Status = correctStatus;

            await pharmacistOrderRepo.PharmacistOrderUpdate();

            return true;
        }

        

        // Converts PharmacistOrder model into PharmacistOrderDto
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

            // Convert every order detail into DTO
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