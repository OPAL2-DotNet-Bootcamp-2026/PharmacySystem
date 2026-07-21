using Pharmacy_System.DTOs.PharmacistOrderDetail;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.PharmacistOrder
{
    public class CreatePharmacistOrderDto
    {
        // Pharmacist creating the order
        [Range(1, int.MaxValue, ErrorMessage = "A valid pharmacist must be selected")]
         public int PharmacistID { get; set; }

        [Range(1, int.MaxValue,  ErrorMessage = "A valid pharmacy must be selected")]
             public int PharmacyID { get; set; }  
   

        // Medicines and quantities included in the order
        [Required(ErrorMessage = "Order details are required")]
        [MinLength(1, ErrorMessage = "The order must contain at least one medicine")]
        public List<CreatePharmacistOrderDetailDto> OrderDetails { get; set; } = new List<CreatePharmacistOrderDetailDto>();
           
    }
}