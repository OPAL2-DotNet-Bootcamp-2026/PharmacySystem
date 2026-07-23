using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.DTOs.WarehouseStock
{
    public class AdjustStockDto
    {
        [Required(ErrorMessage = "Warehouse ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Select a valid warehouse")]
        public int WarehouseID { get; set; }

        [Required(ErrorMessage = "Medicine ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Select a valid medicine")]
        public int MedicineID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public int Quantity { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ExpiryDate { get; set; } // Optional

    }
}
