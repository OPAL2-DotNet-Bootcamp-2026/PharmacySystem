using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Warehouse
{
    public class CreateWarehouseDto
    {
        [Required(ErrorMessage = "Warehouse location is required")]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;
    

    }
}
