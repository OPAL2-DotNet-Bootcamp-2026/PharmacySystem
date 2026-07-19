using Pharmacy_System.Modeles;
using Pharmacy_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Xml;

namespace Pharmacy_System.Modules
{
    public class Medicine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MedicineID { get; set; }  // system generation
        [Required(ErrorMessage = "Medicine name is required")]  // cannot be null
        [MaxLength(100)]
        public string MedicineName { get; set; }  // input

        [Column(TypeName = "decimal(10,2)")]
        [Range(typeof(decimal), "0.01", "10000.00", ErrorMessage = "Unit price must be between 0.01 and 10,000")]
        public decimal UnitPrice { get; set; }  // input
        public bool isAvailable { get; set; } = true;  // system default
        [Required]
        [MaxLength(100)]
        public string Category { get; set; }  // input

        // Warehouse relationship
        [ForeignKey(nameof(Warehouse))]
        public int WarehouseId { get; set; }//FK
        public Warehouse Warehouse { get; set; }
        public ICollection<Supply> Supplies { get; set; } = new List<Supply>();


        public ICollection<PharmacistOrder> PharmacistOrders { get; set; } = new List<PharmacistOrder>();
        public ICollection<CustomerOrder> CustomerOrders { get; set; } = new List<CustomerOrder>();
        public ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();

    }
}
