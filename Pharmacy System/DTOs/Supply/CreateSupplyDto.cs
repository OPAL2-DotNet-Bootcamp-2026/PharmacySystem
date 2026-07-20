using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Supply
{
    // Used when the user wants to create a new supply record
    public class CreateSupplyDto
    {
        // entered Batch number 
        [Required]
        [MaxLength(50)]
        public string BatchNumber { get; set; } = string.Empty;

        // Quantity of medicine supplied
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        // Total cost of the whole supply
        [Range(typeof(decimal), "0.01", "99999999")]
        public decimal TotalCost { get; set; }

        // Expiry date of the supplied medicine
        [Required]
        public DateTime ExpiryDate { get; set; }

        // Foreign key used to select the supplier
        [Range(1, int.MaxValue)]
        public int SupplierId { get; set; }

        // Foreign key used to select the medicine
        [Range(1, int.MaxValue)]
        public int MedicineId { get; set; }

        // Foreign key used to select the warehouse
        [Range(1, int.MaxValue)]
        public int WarehouseId { get; set; }
    }
}