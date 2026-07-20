using Pharmacy_System.DTOs.PharmacistOrderDetail;

namespace Pharmacy_System.DTOs.PharmacistOrder
{
    // Used to return complete pharmacist order information from  API
    
    public class PharmacistOrderDto
    {
        // Order ID 
        public int PharmacistOrderId { get; set; }

        // ID of the pharmacist who created  order
        public int PharmacistId { get; set; }

        // Name of the pharmacist displayed in  response
        public string PharmacistName { get; set; } = string.Empty;

        // ID of the pharmacy that requested  medicines
        public int PharmacyId { get; set; }

        // Date when the order was created
        public DateTime OrderDate { get; set; }

        // Current status of order
        public string Status { get; set; } = string.Empty;

        // List of all medicines and quantities included in  order
        public List<PharmacistOrderDetailDto> OrderDetails { get; set; }  = new List<PharmacistOrderDetailDto>();
          
    }
}