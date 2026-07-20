namespace Pharmacy_System.DTOs.Medicine
{
    public class MedicineDto  // // it return data, so it do not need for [Required] or default values.
    {

        public int MedicineID{ get; set; }

        public string MedicineName { get; set; }

        public string Category { get; set; }

        public decimal UnitPrice { get; set; }

        public bool IsAvailable { get; set; }


    }
}
