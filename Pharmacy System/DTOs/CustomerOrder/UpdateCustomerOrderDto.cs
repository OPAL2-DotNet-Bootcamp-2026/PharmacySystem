using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.CustomerOrder
{
    // Used to update the current status of a customer order
    public class UpdateCustomerOrderStatusDto
    {
        // New order status ==> such as Pending, Completed, or Cancelled
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}