using Pharmacy_System.DTOs.PharmacistOrderDetail;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.PharmacistOrder
{
    // Used when a pharmacist creates a new medicine request
    // for the pharmacy
    public class CreatePharmacistOrderDto
    {
        // ID of the pharmacist creating the order
        [Range(1, int.MaxValue)]
        public int PharmacistId { get; set; }

        // ID of the pharmacy that needs the medicines
        [Range(1, int.MaxValue)]
        public int PharmacyId { get; set; }

        // List of medicines and required quantities
        [Required]
        [MinLength(1)]
        public List<CreatePharmacistOrderDetailDto> OrderDetails { get; set; } = new List<CreatePharmacistOrderDetailDto>();
           
    }
}