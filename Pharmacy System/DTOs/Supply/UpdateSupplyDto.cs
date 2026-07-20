using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Supply
{
    // Used when wants to update an existing supply record
    public class UpdateSupplyDto
    {
        // Updated batch number
        [Required]
        [MaxLength(50)]
        public string BatchNumber { get; set; } = string.Empty;

        // Updated medicine quantity
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        // Updated total cost
        [Range(typeof(decimal), "0.01", "99999999")]
        public decimal TotalCost { get; set; }

        // Updated expiry date
        [Required]
        public DateTime ExpiryDate { get; set; }

        // Updated supplier ID
        [Range(1, int.MaxValue)]
        public int SupplierId { get; set; }

        // Updated medicine ID
        [Range(1, int.MaxValue)]
        public int MedicineId { get; set; }

        // Updated warehouse ID
        [Range(1, int.MaxValue)]
        public int WarehouseId { get; set; }
    }
}