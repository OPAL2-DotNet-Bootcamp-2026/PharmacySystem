using Pharmacy_System.DTOs.Transfer;
using Pharmacy_System.DTOs.TransferDetail;
using Pharmacy_System.Models;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class TransferService
    {
        private readonly PharmacyContext context;

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
            PharmacyStockService pharmacyStockService,
            PharmacyContext context)
        {
            this.context = context;

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
            List<Transfer> transfers =
                await transferRepo.GetAllTransfer();

            List<TransferDto> transferDtos =
                new List<TransferDto>();

            foreach (Transfer transfer in transfers)
            {
                TransferDto dto = ConvertToDto(transfer);

                transferDtos.Add(dto);
            }

            return transferDtos;
        }


        // Returns one transfer using ID
        public async Task<TransferDto?> GetTransferById(int id)
        {
            Transfer? transfer =
                await transferRepo.GetTransferById(id);

            if (transfer == null)
            {
                return null;
            }

            return ConvertToDto(transfer);
        }


        // Creates transfer
        public async Task<int> CreateTransfer(CreateTransferDto dto)
        {
            // Check warehouse
            Warehouse? warehouse =
                await warehouseRepo.GetWarehouseById(
                    dto.WarehouseID
                );

            if (warehouse == null)
            {
                throw new Exception(
                    "Warehouse not found"
                );
            }


            // Check pharmacy
            Pharmacy? pharmacy =
                await pharmacyRepo.GetPharmacyById(
                    dto.PharmacyID
                );

            if (pharmacy == null)
            {
                throw new Exception(
                    "Pharmacy not found"
                );
            }


            // Get pharmacist order
            PharmacistOrder? pharmacistOrder =
                await pharmacistOrderRepo
                    .GetPharmacistOrderById(
                        dto.PharmacistOrderId
                    );

            if (pharmacistOrder == null)
            {
                throw new Exception(
                    "Pharmacist order not found"
                );
            }


            // Prevent creating more than one transfer
            // for the same pharmacist order
            Transfer? existingTransfer =
                await transferRepo
                    .GetByPharmacistOrderId(
                        dto.PharmacistOrderId
                    );

            if (existingTransfer != null)
            {
                throw new Exception(
                    "A transfer already exists for this pharmacist order"
                );
            }


            // Only approved orders can create transfers
            if (pharmacistOrder.Status.ToLower() != "approved")
            {
                throw new Exception(
                    "Only approved pharmacist orders can create a transfer"
                );
            }


            // Check that order belongs to pharmacy
            if (pharmacistOrder.PharmacyID != dto.PharmacyID)
            {
                throw new Exception(
                    "The pharmacist order does not belong to this pharmacy"
                );
            }


            // Transfer must contain medicines
            if (dto.TransferDetails == null ||
                dto.TransferDetails.Count == 0)
            {
                throw new Exception(
                    "The transfer must contain at least one medicine"
                );
            }


            // Prevent same medicine appearing more than once
            var duplicateMedicine =
                dto.TransferDetails
                    .GroupBy(d => d.MedicineID)
                    .FirstOrDefault(
                        g => g.Count() > 1
                    );

            if (duplicateMedicine != null)
            {
                throw new Exception("The same medicine cannot be added more than once in one transfer" );
                    
               
            }


            using var tx =
                await context.Database
                    .BeginTransactionAsync();


            Transfer transfer = new Transfer
            {
                WarehouseID = dto.WarehouseID,

                PharmacyID = dto.PharmacyID,

                PharmacistOrderId =
                    dto.PharmacistOrderId,

                TransferDate = DateTime.Now,

                Status = "Pending"
            };


            // Add medicines to transfer
            foreach (
                CreateTransferDetailDto detailDto
                in dto.TransferDetails)
            {
                // Quantity must be positive
                if (detailDto.Quantity <= 0)
                {
                    throw new Exception(
                        "Transfer quantity must be greater than zero"
                    );
                }


                // Check medicine exists
                Medicine? medicine = await medicineRepo  .GetMedicineById(detailDto.MedicineID  );
                   
                      
                            
                      

                if (medicine == null)
                {
                    throw new Exception( $"Medicine with ID {detailDto.MedicineID} not found" );
                }


                // Check medicine was requested
                // in pharmacist order
                var orderedDetail =
                    pharmacistOrder  .PharmacistOrderDetails.FirstOrDefault(  d => d.MedicineID ==detailDto.MedicineID);

                if (orderedDetail == null)
                {
                    throw new Exception( $"Medicine {medicine.MedicineName} was not requested in the pharmacist order");
                       
                    
                }


                // Transfer quantity cannot exceed
                // pharmacist ordered quantity
                if (detailDto.Quantity >
                    orderedDetail.Quantity)
                {
                    throw new Exception(
                        $"Transfer quantity for {medicine.MedicineName} exceeds the ordered quantity"
                    );
                }


                // Get medicine stock from warehouse
                WarehouseStock? warehouseStock = await warehouseStockService.GetStock(dto.WarehouseID,detailDto.MedicineID);
                   
                        
                            
                            
                        

                if (warehouseStock == null)
                {
                    throw new Exception(
                        $"Medicine {medicine.MedicineName} does not exist in warehouse stock"
                    );
                }


                // Check warehouse has enough quantity
                if (warehouseStock.Quantity <
                    detailDto.Quantity)
                {
                    throw new Exception(
                        $"Not enough warehouse stock for {medicine.MedicineName}"
                    );
                }


                // Check expiry date exists
                if (warehouseStock.ExpiryDate == null)
                {
                    throw new Exception(
                        $"Expiry date is missing for {medicine.MedicineName}"
                    );
                }


                TransferDetail transferDetail =
                    new TransferDetail
                    {
                        MedicineID =detailDto.MedicineID,
                        Quantity = detailDto.Quantity,
                        ExpiryDate =warehouseStock.ExpiryDate.Value
                            
                                
                    };


                transfer.TransferDetails.Add(
                    transferDetail
                );
            }


            // Decrease warehouse stock
            foreach (
                TransferDetail detail
                in transfer.TransferDetails)
            {
                await warehouseStockService
                    .Decrease(
                        transfer.WarehouseID,
                        detail.MedicineID,
                        detail.Quantity
                    );
            }


            // Save transfer
            await transferRepo.Add(transfer);


            await tx.CommitAsync();


            return transfer.TransferId;
        }


        // Updates transfer status
        public async Task<bool> UpdateTransfer(
            int id,
            UpdateTransferDto dto)
        {
            Transfer? transfer =
                await transferRepo
                    .GetTransferById(id);


            if (transfer == null)
            {
                return false;
            }


            // Cancelled transfer cannot be updated
            if (transfer.Status == "Cancelled")
            {
                throw new Exception(
                    "A cancelled transfer cannot be updated"
                );
            }


            // Received transfer cannot be updated
            if (transfer.Status == "Received")
            {
                throw new Exception(
                    "A received transfer cannot be updated"
                );
            }


            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                throw new Exception(
                    "Status is required"
                );
            }


            string status =
                dto.Status.Trim();


            string[] allowedStatuses =
            {
                "Pending",
                "Shipped",
                "Cancelled"
            };


            string? correctStatus = allowedStatuses.FirstOrDefault( s => s.ToLower() ==status.ToLower() );
               
                    

            if (correctStatus == null)
            {
                throw new Exception(
                    "Status must be Pending, Shipped or Cancelled"
                );
            }


            // If transfer is cancelled,
            // return reserved quantity to warehouse
            if (correctStatus == "Cancelled")
            {
                using var tx =
                    await context.Database
                        .BeginTransactionAsync();


                foreach (
                    TransferDetail detail
                    in transfer.TransferDetails)
                {
                    await warehouseStockService
                        .Increase(
                            transfer.WarehouseID,
                            detail.MedicineID,
                            detail.Quantity,
                            detail.ExpiryDate
                        );
                }


                transfer.Status = correctStatus;
                await transferRepo  .TransferUpdate();
                await tx.CommitAsync();
                return true;
            }


            transfer.Status =
                correctStatus;


            await transferRepo
                .TransferUpdate();


            return true;
        }


        // Confirms that transfer was received
        // by the pharmacy
        public async Task<bool> ConfirmReceive(int id)
        {
            Transfer? transfer =
                await transferRepo
                    .GetTransferById(id);


            if (transfer == null)
            {
                return false;
            }


            // Cancelled transfer cannot be received
            if (transfer.Status == "Cancelled")
            {
                throw new Exception(
                    "A cancelled transfer cannot be received"
                );
            }


            // Prevent receiving twice
            if (transfer.Status == "Received")
            {
                throw new Exception(
                    "This transfer has already been received"
                );
            }


            // Only shipped transfer can be received
            if (transfer.Status != "Shipped")
            {
                throw new Exception(
                    "Only a shipped transfer can be received"
                );
            }


            using var tx =
                await context.Database
                    .BeginTransactionAsync();


            // Increase pharmacy stock
            foreach (
                TransferDetail detail
                in transfer.TransferDetails)
            {
                await pharmacyStockService
                    .Increase(
                        transfer.PharmacyID,
                        detail.MedicineID,
                        detail.Quantity,
                        detail.ExpiryDate
                    );
            }


            // Change transfer status
            transfer.Status = "Received";

            transfer.ReceiveDate =  DateTime.Now;

            await transferRepo .TransferUpdate();
              
            // Get pharmacist order
            PharmacistOrder? pharmacistOrder = await pharmacistOrderRepo.GetPharmacistOrderById( transfer.PharmacistOrderId);
            if (pharmacistOrder == null)
            {
                throw new Exception(
                    "Pharmacist order not found"
                );
            }


            // After receiving the medicines,
            // pharmacist order is completed
            pharmacistOrder.Status =
                "Completed";


            // Save pharmacist order status
            await context.SaveChangesAsync();


            await tx.CommitAsync();


            return true;
        }


        // Convert Transfer to TransferDto
        private TransferDto ConvertToDto(
            Transfer transfer)
        {
            TransferDto dto =
                new TransferDto
                {
                    TransferId = transfer.TransferId,
                    WarehouseID =transfer.WarehouseID,
                    Location = transfer.Warehouse.Location,
                    PharmacyID = transfer.PharmacyID,
                    PharmacyName = transfer.Pharmacy.PharmacyName,
                    PharmacistOrderId = transfer.PharmacistOrderId,
                    TransferDate =transfer.TransferDate,
                    ReceiveDate =transfer.ReceiveDate,
                    Status = transfer.Status
                       
                };


            foreach (
                TransferDetail detail
                in transfer.TransferDetails)
            {
                TransferDetailDto detailDto =
                    new TransferDetailDto
                    {
                        MedicineID =  detail.MedicineID,
                        MedicineName =detail.Medicine.MedicineName,     
                        Quantity =detail.Quantity
                            
                    };


                dto.TransferDetails.Add(detailDto);
                 
                
            }


            return dto;
        }
    }
}