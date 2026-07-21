namespace Pharmacy_System.DTOs.Medicine
{
    public class MedicineDto  // // it return data, so it do not need for [Required] or default values.
    {

        public int MedicineID { get; set; }

        public string MedicineName { get; set; }

        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public decimal UnitPrice { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
