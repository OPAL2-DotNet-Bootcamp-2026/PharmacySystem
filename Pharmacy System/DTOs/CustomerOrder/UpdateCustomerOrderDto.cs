using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.CustomerOrder
{
    //  update the status of a customer order
    public class UpdateCustomerOrderStatusDto
    {
        [Required(ErrorMessage = "Order status is required")]
        [MaxLength(30)]
            
        public string Status { get; set; } = string.Empty;
    }
}