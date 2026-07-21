using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.PharmacistOrder
{
    // Used to update the status of  pharmacist order
    public class UpdatePharmacistOrderDto
    {
        [Required(ErrorMessage = "Order status is required")]
        [MaxLength(30)]
        public string Status { get; set; } = string.Empty;
    }
}