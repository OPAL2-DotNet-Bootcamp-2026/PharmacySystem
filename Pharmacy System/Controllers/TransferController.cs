using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Transfer;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferController : ControllerBase
    {
        private readonly TransferService transferService;

        public TransferController(TransferService transferService)
           
        {
            this.transferService = transferService;
        }

        // GET: api/Transfer
        [HttpGet]
        public async Task<IActionResult> GetAllTransfers()
        {
            List<TransferDto> transfers =await transferService.GetAllTransfers();
                

            return Ok(transfers);
        }

        // GET: api/Transfer/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransferById(int id)
        {
            TransferDto? transfer = await transferService.GetTransferById(id);
               

            if (transfer == null)
            {
                return NotFound("Transfer not found");
            }

            return Ok(transfer);
        }

        // POST: api/Transfer
        [HttpPost]
        public async Task<IActionResult> CreateTransfer(CreateTransferDto dto)
            
        {
            try
            {
                int transferId = await transferService.CreateTransfer(dto);
                   

                return Ok(new
                {
                    TransferId = transferId,
                    Message = "Transfer created successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Transfer/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransfer(int id,UpdateTransferDto dto)
        
            
        {
            try
            {
                bool updated = await transferService.UpdateTransfer(id, dto );
                      
                   
                if (!updated)
                {
                    return NotFound("Transfer not found");
                }

                return Ok("Transfer updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}