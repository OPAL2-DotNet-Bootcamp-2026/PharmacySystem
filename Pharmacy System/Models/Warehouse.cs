using Pharmacy_System.Modeles;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Warehouse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WarehouseID {  get; set; }  // system generation
        [Required]
        [MaxLength(100)]
        public string Location { get; set; } // input
        [Range(0, int.MaxValue, ErrorMessage = "The value cannot be negative")]
        public int Quantity { get; set; } // input
        [Required]
        public string ExpiryDate { get; set; } //input

        public ICollection<Supply> Supplies { get; set; } = new List<Supply>();
    }
}
