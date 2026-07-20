using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.PharmacistOrder
{
    // Used to update the current status of a pharmacist order
    public class UpdatePharmacistOrderDto
    {
        // New order status ==> such as Pending, Approved,Rejected, or Completed
        // 
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}