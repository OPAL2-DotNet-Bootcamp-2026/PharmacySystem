using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Warehouse
{
    public class UpdateWarehouseDto
    {
        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string ExpiryDate { get; set; }
    }


}
}
