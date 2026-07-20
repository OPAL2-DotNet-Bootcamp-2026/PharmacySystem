using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DOTs.Medicine
{
    public class CreateMedicineDto //for adding a new medicine
    {

        [Required]
        public string MedicineName { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }


    }
}
