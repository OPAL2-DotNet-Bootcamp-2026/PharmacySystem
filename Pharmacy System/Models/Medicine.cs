using Pharmacy_System.Modules;
using Pharmacy_System.Modules;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Medicine 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MedicineID { get; set; }

        [Required(ErrorMessage = "Medicine name is required")]
        [MaxLength(100)]
        public string MedicineName { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(typeof(decimal), "0.01", "10000.00",
            ErrorMessage = "Unit price must be between 0.01 and 10,000")]
        public decimal UnitPrice { get; set; }
        [Required]
        public bool isAvailable { get; set; } = true;
        [Required]
        public bool IsActive { get; set; } = true;


        // Warehouse relationship
        [ForeignKey(nameof(Warehouse))]
        public int WarehouseID { get; set; }

        public Warehouse Warehouse { get; set; }

        // Warehouse relationship
        [ForeignKey(nameof(Warehouse))]
        public int CategoryID { get; set; }
        //public Category Category { get; set; }


        // One-to-Many relationship with Supplies
        public ICollection<Supply> Supplies { get; set; } = new List<Supply>();


        // Customer order relationship
        public ICollection<CustomerOrderDetail> CustomerOrderDetails { get; set; }
            = new List<CustomerOrderDetail>();

        // Pharmacist order relationship
        public ICollection<PharmacistOrderDetail> PharmacistOrderDetails { get; set; }
            = new List<PharmacistOrderDetail>();

        // Many-to-Many relationship with Transfers
        public ICollection<Transfer> Transfers { get; set; }= new List<Transfer>();
    


    }
}