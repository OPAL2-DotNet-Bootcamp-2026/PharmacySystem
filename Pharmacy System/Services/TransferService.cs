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


  
        // GET ALL TRANSFERS
    
        public async Task<List<TransferDto>> GetAllTransfers()
        {
            List<Transfer> transfers = await transferRepo.GetAllTransfer();
               

            List<TransferDto> transferDtos =new List<TransferDto>();
                

            foreach (Transfer transfer in transfers)
            {
                TransferDto dto = ConvertToDto(transfer);
                   

                transferDtos.Add(dto);
            }

            return transferDtos;
        }


      
        // GET TRANSFER BY ID

        public async Task<TransferDto?> GetTransferById(int id)
        {
            Transfer? transfer =await transferRepo.GetTransferById(id);
                

            if (transfer == null)
            {
                return null;
            }

            return ConvertToDto(transfer);
        }



        // CREATE TRANSFER

        public async Task<int> CreateTransfer(CreateTransferDto dto)
          
        {
     
            // Check warehouse
    
            Warehouse? warehouse =     await warehouseRepo.GetWarehouseById( dto.WarehouseID );
       
            if (warehouse == null)
            {
                throw new Exception( "Warehouse not found");
            }


 
            // Check pharmacy

            Pharmacy? pharmacy =    await pharmacyRepo.GetPharmacyById(dto.PharmacyID  );
            

            if (pharmacy == null)
            {
                throw new Exception("Pharmacy not found");

            }


            // Get pharmacist order
    
            PharmacistOrder? pharmacistOrder = await pharmacistOrderRepo .GetPharmacistOrderById(dto.PharmacistOrderId);
               

            if (pharmacistOrder == null)
            {
                throw new Exception( "Pharmacist order not found" );
            }


            Transfer? existingTransfer =  await transferRepo.GetByPharmacistOrderId( dto.PharmacistOrderId );
             
       
            if (existingTransfer != null)
            {
                throw new Exception("A transfer already exists for this pharmacist order");

            }


        
            // Only approved orders
            // can create transfers
        
            if (!string.Equals( pharmacistOrder.Status, "Approved",StringComparison.OrdinalIgnoreCase))
                 
            {
                throw new Exception("Only approved pharmacist orders can create a transfer"
                    
                );
            }


            // Check order belongs
            // to same pharmacy
          
            if (pharmacistOrder.PharmacyID !=dto.PharmacyID)
                
            {
                throw new Exception("The pharmacist order does not belong to this pharmacy"  );
               
              
            }


            // Transfer must contain
            // at least one medicine
            if (dto.TransferDetails == null ||dto.TransferDetails.Count == 0)
        
            {
                throw new Exception("The transfer must contain at least one medicine"
                    
                );
            }


            // Transfer must contain
            // ALL ordered medicines
            if (dto.TransferDetails.Count !=pharmacistOrder.PharmacistOrderDetails.Count)
  
            {
                throw new Exception("The transfer must contain all medicines from the pharmacist order"
                    
                );
            }


            // Prevent duplicate medicine
            var duplicateMedicine = dto.TransferDetails.GroupBy(d => d.MedicineID ).FirstOrDefault(g => g.Count() > 1);
              

            if (duplicateMedicine != null)
            {
                throw new Exception("The same medicine cannot be added more than once in one transfer" );

            }


            using var tx = await context.Database.BeginTransactionAsync();
               


            try
            {
                Transfer transfer =
                    new Transfer
                    {
                        WarehouseID = dto.WarehouseID,
                        PharmacyID =dto.PharmacyID,
                        PharmacistOrderId =dto.PharmacistOrderId,
                        TransferDate =DateTime.Now,
                        Status = "Pending"
                           
                    };


                // ADD MEDICINES
             
                foreach ( CreateTransferDetailDto detailDto in dto.TransferDetails)
                   
                    
                {
            
                    // Quantity must be positive
                    if (detailDto.Quantity <= 0)
                    {
                        throw new Exception("Transfer quantity must be greater than zero");
 
                    }


                    // Check medicine exists
                    Medicine? medicine =await medicineRepo.GetMedicineById(detailDto.MedicineID );

                    if (medicine == null)
                    {
                        throw new Exception( $"Medicine with ID {detailDto.MedicineID} not found");

                    }


                    // Check medicine was
                    // requested in order
                    var orderedDetail = pharmacistOrder.PharmacistOrderDetails.FirstOrDefault( d => d.MedicineID ==detailDto.MedicineID );
                       


                    if (orderedDetail == null)
                    {
                        throw new Exception( $"Medicine {medicine.MedicineName} was not requested in the pharmacist order"  );
                           
                      
                    }


              
                    if (detailDto.Quantity !=orderedDetail.Quantity)
                        
                    {
                        throw new Exception( $"Transfer quantity for {medicine.MedicineName} must equal the ordered quantity");
                           
                        
                    }


                    // Get warehouse stock
                    WarehouseStock? warehouseStock =await warehouseStockService.GetStock( dto.WarehouseID,detailDto.MedicineID);

                    if (warehouseStock == null)
                    {
                        throw new Exception(  $"Medicine {medicine.MedicineName} does not exist in warehouse stock");
                          
                        
                    }


                    // Check enough quantity
                    if (warehouseStock.Quantity <detailDto.Quantity)
                        
                    {
                        throw new Exception( $"Not enough warehouse stock for {medicine.MedicineName}");
                           
                        
                    }


                    // 
                    // Check expiry date
                    if (warehouseStock.ExpiryDate ==null)
                        
                    {
                        throw new Exception($"Expiry date is missing for {medicine.MedicineName}");
                            
                        
                    }


                    
                    // Create transfer detail
                    // 
                    TransferDetail transferDetail =
                        new TransferDetail
                        {
                            MedicineID =detailDto.MedicineID,
                            Quantity =detailDto.Quantity,
                            ExpiryDate = warehouseStock.ExpiryDate.Value
                    
                        };


                    transfer.TransferDetails.Add(transferDetail );
                        
                   
                }


                // DECREASE WAREHOUSE STOCK
                foreach (TransferDetail detail in transfer.TransferDetails)
                    
                    
                {
                    await warehouseStockService
                        .Decrease(
                            transfer.WarehouseID,
                            detail.MedicineID,
                            detail.Quantity
                        );
                }


                // SAVE TRANSFER
                await transferRepo.Add(transfer );
                    

                await tx.CommitAsync();


                return transfer.TransferId;
            }
            catch
            {
                await tx.RollbackAsync();

                throw;
            }
        }


        
        // UPDATE TRANSFER STATUS
        public async Task<bool> UpdateTransfer(int id,UpdateTransferDto dto)
            
            
        {
            Transfer? transfer =await transferRepo.GetTransferById(id);
                
            if (transfer == null)
            {
                return false;
            }


            // Cancelled cannot update
            if (transfer.Status =="Cancelled")
                
            {
                throw new Exception("A cancelled transfer cannot be updated" );
                    
               
            }


            // Received cannot update
            if (transfer.Status =="Received")
                
            {
                throw new Exception( "A received transfer cannot be updated" );
                   
               
            }


            // Status required
            if (string.IsNullOrWhiteSpace( dto.Status))
                   
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


            string? correctStatus = allowedStatuses.FirstOrDefault(s => string.Equals(s, status, StringComparison.OrdinalIgnoreCase));
               


            if (correctStatus == null)
            {
                throw new Exception( "Status must be Pending, Shipped or Cancelled" );
                   
               
            }


            // If cancelled,
            // return stock to warehouse
            if (correctStatus =="Cancelled")
                
            {
                using var tx =await context.Database.BeginTransactionAsync();
                    
                        

                try
                {
                    foreach (TransferDetail detail in transfer.TransferDetails)
                        
                        
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

                    await transferRepo.TransferUpdate();

                    await tx.CommitAsync();


                    return true;
                }
                catch
                {
                    await tx.RollbackAsync();

                    throw;
                }
            }


            transfer.Status =correctStatus;

                

            await transferRepo.TransferUpdate();
                


            return true;
        }


        // CONFIRM RECEIVE
        public async Task<bool> ConfirmReceive( int id)
           
        {
            Transfer? transfer =await transferRepo.GetTransferById(id);
                

            if (transfer == null)
            {
                return false;
            }


            // Cancelled cannot receive
            if (transfer.Status =="Cancelled")
                
            {
                throw new Exception("A cancelled transfer cannot be received" );
                    
               
            }


            // Prevent receive twice
            if (transfer.Status =="Received")
                
            {
                throw new Exception("This transfer has already been received" );
                    
               
            }


            // Only shipped can receive
            // 
            if (transfer.Status !="Shipped")
                
            {
                throw new Exception("Only a shipped transfer can be received");
                    
                
            }


            using var tx = await context.Database.BeginTransactionAsync();
               

                    

            try
            {
                // INCREASE PHARMACY STOCK
                foreach ( TransferDetail detail in transfer.TransferDetails)
                   
                    
                {
                    await pharmacyStockService
                        .Increase(
                            transfer.PharmacyID,
                            detail.MedicineID,
                            detail.Quantity,
                            detail.ExpiryDate
                        );
                }


                // UPDATE TRANSFER
                transfer.Status = "Received";

                transfer.ReceiveDate = DateTime.Now;
   

                await transferRepo.TransferUpdate();

                    

                // GET PHARMACIST ORDER
                
                PharmacistOrder?pharmacistOrder = await pharmacistOrderRepo.GetPharmacistOrderById(transfer.PharmacistOrderId);
                    
    
                if (pharmacistOrder == null)
                   
                {
                    throw new Exception(  "Pharmacist order not found" );
                      
                   
                }


                // COMPLETE ORDER
                pharmacistOrder.Status = "Completed";
                   


                await context.SaveChangesAsync();
                  


                await tx.CommitAsync();


                return true;
            }
            catch
            {
                await tx.RollbackAsync();

                throw;
            }
        }


        // CONVERT TO DTO
        // 
        private TransferDto ConvertToDto( Transfer transfer)
           
        {
            TransferDto dto =
                new TransferDto
                {
                    TransferId =transfer.TransferId,
                    WarehouseID = transfer.WarehouseID,
                    Location =transfer.Warehouse.Location,
                    PharmacyID = transfer.PharmacyID,
                    PharmacyName =transfer.Pharmacy .PharmacyName,
                    PharmacistOrderId = transfer .PharmacistOrderId,
                    TransferDate = transfer.TransferDate,
                    ReceiveDate =transfer.ReceiveDate,
                    Status =transfer.Status
                        
                }; 


            foreach (TransferDetail detail in transfer.TransferDetails)
                
                
            {
                TransferDetailDto detailDto = new TransferDetailDto
                   
                    {
                        MedicineID = detail.MedicineID,
                        MedicineName =detail.Medicine .MedicineName,
                        Quantity =  detail.Quantity
                          
                    };


                dto.TransferDetails.Add( detailDto );
                   
               
            }


            return dto;
        }
    }
}