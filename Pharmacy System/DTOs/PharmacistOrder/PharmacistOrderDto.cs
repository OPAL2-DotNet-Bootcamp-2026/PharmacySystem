using Pharmacy_System.DTOs.PharmacistOrderDetail;

namespace Pharmacy_System.DTOs.PharmacistOrder
{
    // Used to return complete pharmacist order information
    public class PharmacistOrderDto
    {
        
        public int PharmacistOrderId { get; set; }

        // Pharmacist information
        public int PharmacistID { get; set; }
        public string FullName { get; set; } = string.Empty;

        // Pharmacy information
        public int PharmacyID { get; set; }
        public string PharmacyName { get; set; } = string.Empty;

        // Generated when the order is created
        public DateTime OrderDate { get; set; }

        // Calculated 
        public decimal TotalCost { get; set; }

        //  order status
        public string Status { get; set; } = string.Empty;

        // Medicines and quantities included in the order
        public List<PharmacistOrderDetailDto> OrderDetails { get; set; }= new List<PharmacistOrderDetailDto>();
            
    }
}