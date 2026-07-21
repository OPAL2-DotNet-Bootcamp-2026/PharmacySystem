using Pharmacy_System.Modules;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Warehouse : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WarehouseID {  get; set; }  // system generation
        [Required]
        [MaxLength(150)]
        public string Location { get; set; } // input
       
     
        public ICollection<Supply> Supplies { get; set; } = new List<Supply>();
        public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
    }
}
