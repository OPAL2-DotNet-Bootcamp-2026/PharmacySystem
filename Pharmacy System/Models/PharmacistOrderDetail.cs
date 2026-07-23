using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pharmacy_System.Models;

namespace Pharmacy_System.Modules
{
    // Composite primary key:
    // PharmacistOrderId + MedicineID
    [PrimaryKey(nameof(PharmacistOrderId), nameof(MedicineID))]
    public class PharmacistOrderDetail : BaseEntity
    {
       
       
        [ForeignKey(nameof(PharmacistOrder))]
        public int PharmacistOrderId { get; set; }
        public PharmacistOrder PharmacistOrder { get; set; } = null!;

       
        [ForeignKey(nameof(Medicine))]
        [Range(1, int.MaxValue)]
        public int MedicineID { get; set; }
        public Medicine Medicine { get; set; } = null!;


        // Input: quantity requested by the pharmacist
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}