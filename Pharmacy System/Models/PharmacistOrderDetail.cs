using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.Modules
{
    public class PharmacistOrderDetail
    {
        public int PharmacistOrderId { get; set; }

        public PharmacistOrder PharmacistOrder { get; set; } = null!;//relation

        public int MedicineId { get; set; }

        public Medicine Medicine { get; set; } = null!;//relation

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}