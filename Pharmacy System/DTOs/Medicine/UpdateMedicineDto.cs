using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.DTOs.Medicine
{
    public class UpdateMedicineDto
    {

        [Required(ErrorMessage = "Medicine name is required")]
        [MaxLength(100, ErrorMessage = "Medicine name cannot exceed 150 characters")]
        public string MedicineName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        public int CategoryID { get; set; } //fk

        [Range(typeof(decimal), "0.01", "10000.00",
           ErrorMessage = "Unit price must be between 0.01 and 10,000")]
        public decimal UnitPrice { get; set; }

        public bool IsAvailable { get; set; }

    }
}
