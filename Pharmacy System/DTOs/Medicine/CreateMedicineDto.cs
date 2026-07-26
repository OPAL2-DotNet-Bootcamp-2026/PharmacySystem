using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Medicine
{
    public class CreateMedicineDto //for adding a new medicine
    {

        [Required(ErrorMessage = "Medicine name is required")]
        [MaxLength(100,ErrorMessage = "Medicine name cannot exceed 100 characters")]
        public string MedicineName { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryID { get; set; }
        [Required(ErrorMessage = "Unit price is required")]
        [Range(typeof(decimal), "0.01", "10000.00",
            ErrorMessage = "Unit price must be between 0.01 and 10,000")]
        public decimal UnitPrice { get; set; }


    }
}
