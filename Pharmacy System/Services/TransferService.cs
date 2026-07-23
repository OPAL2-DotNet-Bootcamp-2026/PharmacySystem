using Pharmacy_System.DTOs.Transfer;
using Pharmacy_System.DTOs.TransferDetail;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class TransferService
    {
        private readonly TransferRepo transferRepo;
        private readonly WarehouseRepo warehouseRepo;
        private readonly PharmacyRepo pharmacyRepo;
        private readonly PharmacistOrderRepo pharmacistOrderRepo;
        private readonly MedicineRepo medicineRepo;

        public TransferService(
            TransferRepo transferRepo,
            WarehouseRepo warehouseRepo,
            PharmacyRepo pharmacyRepo,
            PharmacistOrderRepo pharmacistOrderRepo,
            MedicineRepo medicineRepo)
        {
            this.transferRepo = transferRepo;
            this.warehouseRepo = warehouseRepo;
            this.pharmacyRepo = pharmacyRepo;
            this.pharmacistOrderRepo = pharmacistOrderRepo;
            this.medicineRepo = medicineRepo;
        }

        // Returns all transfers 
        public async Task<List<TransferDto>> GetAllTransfers()
        {
            List<Transfer> transfers =await transferRepo.GetAllTransfer();
                

            List<TransferDto> transferDtos =new List<TransferDto>();
                

            foreach (Transfer transfer in transfers)
            {
                TransferDto dto = ConvertToDto(transfer);
                transferDtos.Add(dto);
            }

            return transferDtos;
        }

        // Returns one transfer using its ID
        public async Task<TransferDto?> GetTransferById(int id)
        {
            Transfer? transfer =await transferRepo.GetTransferById(id);
                

            if (transfer == null)
            {
                return null;
            }

            return ConvertToDto(transfer);
        }

        // Creates a new transfer
        public async Task<int> CreateTransfer(CreateTransferDto dto)
        {
            // Check that the warehouse exists
            Warehouse? warehouse =await warehouseRepo.GetWarehouseById(dto.WarehouseID);
                

            if (warehouse == null)
            {
                throw new Exception("Warehouse not found");
            }

            // Check that the pharmacy exists
            Pharmacy? pharmacy = await pharmacyRepo.GetPharmacyById(dto.PharmacyID);

            if (pharmacy == null)
            {
                throw new Exception("Pharmacy not found");
            }

            // Check that the pharmacist order exists
            PharmacistOrder? pharmacistOrder =await pharmacistOrderRepo.GetPharmacistOrderById(dto.PharmacistOrderId);
                
                    
            if (pharmacistOrder == null)
            {
                throw new Exception("Pharmacist order not found");
            }


            // Check that the order belongs to the selected pharmacy
            if (pharmacistOrder.PharmacyID != dto.PharmacyID)
            {
                throw new Exception("The pharmacist order does not belong to this pharmacy" );
            
            }

            // Check that at least one medicine was entered
            if (dto.TransferDetails == null ||dto.TransferDetails.Count == 0)
                
            {
                throw new Exception( "The transfer must contain at least one medicine");
              
            }

            // Create the main Transfer model
            Transfer transfer = new Transfer
            {
                WarehouseID = dto.WarehouseID,
                PharmacyID = dto.PharmacyID,
                PharmacistOrderId = dto.PharmacistOrderId,

                TransferDate = DateTime.Now,

                // Default status
                Status = "Pending"
            };

            // Add each medicine to TransferDetails
            foreach (CreateTransferDetailDto detailDto in dto.TransferDetails)
                    
            {
                // Check that the medicine exists
                Medicine? medicine =await medicineRepo.GetMedicineById(detailDto.MedicineID );
                    
                if (medicine == null)
                {
                    throw new Exception( $"Medicine with ID {detailDto.MedicineID} not found");
                       
                    
                }

                // Prevent adding the same medicine twice
                bool medicineAlreadyAdded =transfer.TransferDetails.Any(d => d.MedicineID == detailDto.MedicineID);
                    
                   
                if (medicineAlreadyAdded)
                {
                    throw new Exception("The same medicine cannot be added twice");
                        
                    
                }

                // Create one transfer detail
                TransferDetail transferDetail =
                    new TransferDetail
                    {
                        MedicineID = detailDto.MedicineID,
                        Quantity = detailDto.Quantity
                    };

                // Add the detail to the transfer
                transfer.TransferDetails.Add(transferDetail);
            }

            // Save Transfer and TransferDetails
            await transferRepo.Add(transfer);
            return transfer.TransferId;
        }

        // Updates transfer status and receive date
        public async Task<bool> UpdateTransfer(
            int id,
            UpdateTransferDto dto)
        {
            Transfer? transfer =await transferRepo.GetTransferById(id);
                

            if (transfer == null)
            {
                return false;
            }

            string status = dto.Status.Trim();

            // Only these statuses are allowed
            string[] allowedStatuses =
            {
                "Pending",
                "Shipped",
                "Received",
                "Cancelled"
            };

            string? correctStatus =allowedStatuses.FirstOrDefault(s =>s.ToLower() == status.Trim().ToLower());
                
                    

            if (correctStatus == null)
            {
                throw new Exception("Status must be Pending, Shipped, Received or Cancelled");
             }
            transfer.Status = correctStatus;


            // Prevent changing a cancelled transfer
            if (transfer.Status == "Cancelled")
            {
                throw new Exception("A cancelled transfer cannot be updated");
             }       
                
            

            // Prevent changing a received transfer
            if (transfer.Status == "Received")
            {
                throw new Exception("A received transfer cannot be updated");
                    
                
            }

            transfer.Status = correctStatus;

            // ReceiveDate is added only when status becomes Received
            if (correctStatus == "Received")
            {
                transfer.ReceiveDate =dto.ReceiveDate ?? DateTime.Now;
                    
            }
            else
            {
                transfer.ReceiveDate = null;
            }

            await transferRepo.TransferUpdate();

            return true;
        }

        

        // Converts Transfer model into TransferDto
        private TransferDto ConvertToDto(Transfer transfer)
        {
            TransferDto dto = new TransferDto
            {
                TransferId = transfer.TransferId,

                WarehouseID = transfer.WarehouseID,
                Location = transfer.Warehouse.Location,

                PharmacyID = transfer.PharmacyID,
                PharmacyName = transfer.Pharmacy.PharmacyName,

                PharmacistOrderId =
                    transfer.PharmacistOrderId,

                TransferDate = transfer.TransferDate,
                ReceiveDate = transfer.ReceiveDate,
                Status = transfer.Status
            };

            foreach (TransferDetail detail in transfer.TransferDetails)
                    
            {
                TransferDetailDto detailDto =
                    new TransferDetailDto
                    {
                        MedicineID = detail.MedicineID,
                        MedicineName = detail.Medicine.MedicineName,
                        Quantity = detail.Quantity
                    };

                dto.TransferDetails.Add(detailDto);
            }

            return dto;
        }
    }
}