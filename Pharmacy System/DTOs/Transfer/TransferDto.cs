using Pharmacy_System.DTOs.TransferDetail;

namespace Pharmacy_System.DTOs.Transfer
{
    public class TransferDto
    {
        public int TransferId { get; set; }


        public int WarehouseID { get; set; }
        public string Location { get; set; } = string.Empty;


        public int PharmacyID { get; set; }
        public string PharmacyName { get; set; } = string.Empty;

        public int PharmacistOrderId { get; set; }

        public DateTime TransferDate { get; set; }

        public DateTime? ReceiveDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public List<TransferDetailDto> TransferDetails { get; set; }= new List<TransferDetailDto>();
            
    }
}