using Pharmacy_System.DTOs.Transfer;
using Pharmacy_System.DTOs.TransferDetail;
using Pharmacy_System.Models;
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

        private readonly WarehouseStockService warehouseStockService;
        private readonly PharmacyStockService pharmacyStockService;

        public TransferService(
            TransferRepo transferRepo,
            WarehouseRepo warehouseRepo,
            PharmacyRepo pharmacyRepo,
            PharmacistOrderRepo pharmacistOrderRepo,
            MedicineRepo medicineRepo,
            WarehouseStockService warehouseStockService,
            PharmacyStockService pharmacyStockService)
        {
            this.transferRepo = transferRepo;
            this.warehouseRepo = warehouseRepo;
            this.pharmacyRepo = pharmacyRepo;
            this.pharmacistOrderRepo = pharmacistOrderRepo;
            this.medicineRepo = medicineRepo;

            this.warehouseStockService = warehouseStockService;
            this.pharmacyStockService = pharmacyStockService;
        }

        // Returns all transfers 
        public async Task<List<TransferDto>> GetAllTransfers()
        {
            List<Transfer> transfers = await transferRepo.GetAllTransfer();


            List<TransferDto> transferDtos = new List<TransferDto>();


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
            Transfer? transfer = await transferRepo.GetTransferById(id);


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
            Warehouse? warehouse = await warehouseRepo.GetWarehouseById(
                dto.WarehouseID);


            if (warehouse == null)
            {
                throw new Exception("Warehouse not found");
            }

            // Check that the pharmacy exists
            Pharmacy? pharmacy = await pharmacyRepo.GetPharmacyById(
                dto.PharmacyID);

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

            // Only approved orders can create transfers
            if (pharmacistOrder.Status.ToLower() != "approved")
            {
                throw new Exception("Only approved pharmacist orders can create a transfer");
                    

            }

            // Check that the order belongs to the selected pharmacy
            if (pharmacistOrder.PharmacyID != dto.PharmacyID)
            {
                throw new Exception( "The pharmacist order does not belong to this pharmacy");
                   

            }

            // Check that at least one medicine was entered
            if (dto.TransferDetails == null ||dto.TransferDetails.Count == 0)
                

            {
                throw new Exception("The transfer must contain at least one medicine");
                    

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
                Medicine? medicine = await medicineRepo.GetMedicineById(detailDto.MedicineID);
                    

                if (medicine == null)
                {
                    throw new Exception( $"Medicine with ID {detailDto.MedicineID} not found");
                       


                }

                // Prevent adding the same medicine twice
                bool medicineAlreadyAdded = transfer.TransferDetails.Any( d => d.MedicineID == detailDto.MedicineID);
                   
                       


                if (medicineAlreadyAdded)
                {
                    throw new Exception( "The same medicine cannot be added twice");
                       


                }

                // Check that the medicine exists in warehouse stock
                WarehouseStock? warehouseStock =await warehouseStockService.GetStock( dto.WarehouseID, detailDto.MedicineID);
                    
                       
                       

                if (warehouseStock == null)
                {
                    throw new Exception( $"Medicine {medicine.MedicineName} does not exist in warehouse stock");
                       
                }

                // Check that the warehouse has enough quantity
                if (warehouseStock.Quantity < detailDto.Quantity)
                {
                    throw new Exception( $"Not enough warehouse stock for {medicine.MedicineName}");
                       
                }

                // Check that the warehouse stock has an expiry date
                if (warehouseStock.ExpiryDate == null)
                {
                    throw new Exception( $"Expiry date is missing for {medicine.MedicineName}");
                       
                }

                // Create one transfer detail
                TransferDetail transferDetail =
                    new TransferDetail
                    {
                        MedicineID = detailDto.MedicineID,
                        Quantity = detailDto.Quantity,
                        ExpiryDate = warehouseStock.ExpiryDate.Value
                    };

                // Add the detail to the transfer
                transfer.TransferDetails.Add(transferDetail);
            }

            // Decrease warehouse stock for each transferred medicine
            foreach (TransferDetail detail in transfer.TransferDetails)
            {
                await warehouseStockService.Decrease(
                    transfer.WarehouseID,
                    detail.MedicineID,
                    detail.Quantity);
            }

            // Save Transfer and TransferDetails
            await transferRepo.Add(transfer);

            return transfer.TransferId;
        }

        // Updates transfer status
        public async Task<bool> UpdateTransfer( int id,  UpdateTransferDto dto)
           
          
        {
            Transfer? transfer = await transferRepo.GetTransferById(id);


            if (transfer == null)
            {
                return false;
            }

            // Check the old status before changing it
            if (transfer.Status == "Cancelled")
            {
                throw new Exception( "A cancelled transfer cannot be updated");
                   

            }

            if (transfer.Status == "Received")
            {
                throw new Exception("A received transfer cannot be updated");
                    

            }

            // Check that status was entered
            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                throw new Exception( "Status is required");
                   

            }

            string status = dto.Status.Trim();

            string[] allowedStatuses =
            {
                "Pending",
                "Shipped",
                "Cancelled"
            };

            // Search for the status in the allowed list

            string? correctStatus =allowedStatuses.FirstOrDefault(s => s.ToLower() == status.ToLower());
                
                    


            if (correctStatus == null)
            {
                throw new Exception("Status must be Pending, Shipped or Cancelled");
                    

            }

            transfer.Status = correctStatus;

            await transferRepo.TransferUpdate();

            return true;
        }


        // Confirms that the transfer was received by the pharmacy
        public async Task<bool> ConfirmReceive(int id)
        {
            Transfer? transfer = await transferRepo.GetTransferById(id);
               

            if (transfer == null)
            {
                return false;
            }

            // A cancelled transfer cannot be received
            if (transfer.Status == "Cancelled")
            {
                throw new Exception( "A cancelled transfer cannot be received");
                   
            }

            // Prevent increasing pharmacy stock more than once
            if (transfer.Status == "Received")
            {
                throw new Exception("This transfer has already been received");
                    
            }

            // Only shipped transfers can be received
            if (transfer.Status != "Shipped")
            {
                throw new Exception("Only a shipped transfer can be received");
                    
            }

            // Increase pharmacy stock for each received medicine
            foreach (TransferDetail detail in transfer.TransferDetails)
            {
                await pharmacyStockService.Increase(
                    transfer.PharmacyID,
                    detail.MedicineID,
                    detail.Quantity,
                    detail.ExpiryDate);
            }

            // Change transfer status to Received
            transfer.Status = "Received";

            // ReceiveDate is generated by the system
            transfer.ReceiveDate = DateTime.Now;

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

                PharmacistOrderId = transfer.PharmacistOrderId,

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
                        MedicineName =
                            detail.Medicine.MedicineName,
                        Quantity = detail.Quantity
                    };

                dto.TransferDetails.Add(detailDto);
            }

            return dto;
        }
    }
}