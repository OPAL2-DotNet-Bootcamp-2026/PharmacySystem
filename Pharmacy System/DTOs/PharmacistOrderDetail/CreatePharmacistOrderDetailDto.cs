using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.PharmacistOrderDetail
{
    // Represents  medicine  inside a pharmacist order
    // The full pharmacist order can contain multiple medicine 
    public class CreatePharmacistOrderDetailDto
    {
        // ID of the medicine for this order 
        [Range(1, int.MaxValue)]
        public int MedicineId { get; set; }

        // Quantity requested 
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}