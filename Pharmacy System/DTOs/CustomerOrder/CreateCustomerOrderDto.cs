using Pharmacy_System.DTOs.CustomerOrderDetail;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.CustomerOrder
{
    // Used to receive information when creating  new customer order
    // The order can contain multiple medicine items
    public class CreateCustomerOrderDto
    {
        // ID of the customer placing the order
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        // ID of the pharmacy handling the order
        [Range(1, int.MaxValue)]
        public int PharmacyId { get; set; }

        // List of medicine items included in the customer order
        [Required]
        [MinLength(1)]
        public List<CreateCustomerOrderDetailDto> OrderDetails { get; set; }= new List<CreateCustomerOrderDetailDto>();
            
    }
}