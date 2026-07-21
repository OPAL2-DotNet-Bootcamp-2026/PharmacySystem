using Pharmacy_System.DTOs.TransferDetail;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Transfer
{
    public class CreateTransferDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "A valid warehouse must be selected")]
        public int WarehouseID { get; set; }   
        

        [Range(1, int.MaxValue,ErrorMessage = "A valid pharmacy must be selected")]
        public int PharmacyID { get; set; }    
        

        [Range(1, int.MaxValue,ErrorMessage = "A valid pharmacist order must be selected")]
        public int PharmacistOrderId { get; set; }    
        

        [Required(ErrorMessage = "Transfer details are required")]
        [MinLength(1, ErrorMessage = "The transfer must contain at least one medicine")]
        public List<CreateTransferDetailDto> TransferDetails { get; set; } = new List<CreateTransferDetailDto>();
           
    }
}