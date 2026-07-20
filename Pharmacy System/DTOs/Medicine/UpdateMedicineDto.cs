using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.DTOs.Medicine
{
    public class UpdateMedicineDto
    {

        [Required]
        [MaxLength(100)]
        public string MedicineName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        public bool IsAvailable { get; set; }

    }
}
