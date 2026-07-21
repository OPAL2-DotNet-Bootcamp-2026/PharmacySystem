using Pharmacy_System.DTOs.CustomerOrderDetail;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.CustomerOrder
{
    // Used when creating a new customer order
    public class CreateCustomerOrderDto
    {
        // Customer placing the order
        [Range(1, int.MaxValue, ErrorMessage = "A valid customer must be selected")]
           
        public int CustomerID { get; set; }

        // Pharmacy fulfilling the order
        [Range(1, int.MaxValue,ErrorMessage = "A valid pharmacy must be selected")]
            
        public int PharmacyID { get; set; }

        // Medicines and quantities included in the order
        [Required(ErrorMessage = "Order details are required")]
        [MinLength(1,ErrorMessage = "The order must contain at least one medicine")]
            
        public List<CreateCustomerOrderDetailDto> OrderDetails { get; set; }= new List<CreateCustomerOrderDetailDto>();
            
    }
}