using Pharmacy_System.Models;
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


        // one to many:  Warehouse to Transfers
        public ICollection<Transfer> Transfers { get; set; }= new List<Transfer>();
        //one to many:  Warehouse to WarehouseStocks
        public ICollection<WarehouseStock> warehouseStocks { get; set; } = new List<WarehouseStock>();
        
        
    }
}
