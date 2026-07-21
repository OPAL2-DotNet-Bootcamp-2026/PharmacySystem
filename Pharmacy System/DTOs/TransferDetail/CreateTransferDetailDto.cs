using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.TransferDetail
{
    // Represents one medicine included in the transfer
    public class CreateTransferDetailDto
    {
        // Input: selected medicine
        [Range(1, int.MaxValueErrorMessage = "A valid medicine must be selected")],
        public int MedicineID { get; set; }    
        

        // Input
        [Range(1, int.MaxValue,ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }    
        
    }
}