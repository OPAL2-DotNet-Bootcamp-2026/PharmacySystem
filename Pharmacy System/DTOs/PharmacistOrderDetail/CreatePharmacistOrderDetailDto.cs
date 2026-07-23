using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.PharmacistOrderDetail
{
    // Represents one medicine requested inside  pharmacist order
    public class CreatePharmacistOrderDetailDto
    {

        [Range(1, int.MaxValue, ErrorMessage = "A valid medicine must be selected")]
         public int MedicineID { get; set; }
        

      
        [Range(1, int.MaxValue,ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }    
        
    }
}