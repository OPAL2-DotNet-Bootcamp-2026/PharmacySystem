using Pharmacy_System.Modules;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.Modules
{
    public class PharmacistOrderDetail
    {
        public int PharmacistOrderId { get; set; }

        public PharmacistOrder PharmacistOrder { get; set; } = null!;//relation

        public int MedicineID { get; set; }//fk

        public Medicine Medicine { get; set; } = null!;//relation

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}