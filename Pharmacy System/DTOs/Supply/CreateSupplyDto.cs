using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Supply
{
    public class CreateSupplyDto
    {
        [Required(ErrorMessage = "Batch number is required")]
        [MaxLength(50)]
        public string BatchNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Range(typeof(decimal), "0.01", "99999999.99",ErrorMessage = "Total cost must be greater than 0")]
        public decimal TotalCost { get; set; }

        [Required(ErrorMessage = "Expiry date is required")]
        public DateTime? ExpiryDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid supplier must be selected")]
        public int SupplierID { get; set; }   
        

        [Range(1, int.MaxValue,ErrorMessage = "A valid medicine must be selected")]
        public int MedicineID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid warehouse must be selected")]
        public int WarehouseID { get; set; }
    }
}