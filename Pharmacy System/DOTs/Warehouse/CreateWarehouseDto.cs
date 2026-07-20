using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DOTs.Warehouse
{
    public class CreateWarehouseDto
    {
        [Required]
        public string Location { get; set; } = string.Empty;
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string ExpiryDate { get; set; }

    }
}
