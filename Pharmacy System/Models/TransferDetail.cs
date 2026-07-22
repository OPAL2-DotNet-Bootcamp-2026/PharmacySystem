using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pharmacy_System.Models;

namespace Pharmacy_System.Modules
{
    public class TransferDetail : BaseEntity
    {
        [Index(nameof(TransferID), nameof(MedicineID), IsUnique = true)]
        //system generated 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransferDetailID { get; set; }

       
        [ForeignKey(nameof(Transfer))]
        [Range(1, int.MaxValue)]
        public int TransferID { get; set; }
        public Transfer Transfer { get; set; } = null!;

      
        [ForeignKey(nameof(Medicine))]
        [Range(1, int.MaxValue)]
        public int MedicineID { get; set; }
        public Medicine Medicine { get; set; } = null!;

       
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}