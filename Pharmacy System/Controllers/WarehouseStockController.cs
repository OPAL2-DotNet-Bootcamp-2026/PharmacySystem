using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.WarehouseStock;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{

    [ApiController]
    [Route("warehouseStock")]
    public class WarehouseStockController : ControllerBase
    {
        private readonly WarehouseStockService warehouseStockService;

        public WarehouseStockController(WarehouseStockService _warehouseStockService)
        {
            warehouseStockService = _warehouseStockService;
        }


        // Get all stock in one warehouse
        [HttpGet("GetByWarehouse/{warehouseId}")]
        [Authorize]
        public async Task<IActionResult> GetByWarehouse(int warehouseId)
        {
            List<WarehouseStockDto> stocks = await warehouseStockService.GetByWarehouse(warehouseId);

            return Ok(stocks);
        }

        // Get warehouse stock for one medicine
        [HttpGet("GetByMedicine/{medicineId}")]
        [Authorize]
        public async Task<IActionResult> GetByMedicine(int medicineId)
        {
            List<WarehouseStockDto> stocks = await warehouseStockService.GetByMedicine(medicineId);

            return Ok(stocks);
        }


        // Increase medicine quantity in the warehouse
        [HttpPut("Increase")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Increase(int warehouseId,int medicineId,int quantity)
        {
            try
            {
                await warehouseStockService.Increase(warehouseId,medicineId,quantity);

                return Ok(new
                {
                    message = "Warehouse stock increased successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // Decrease medicine quantity in the warehouse
        [HttpPut("Decrease")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Decrease(int warehouseId,int medicineId,int quantity)
        {
            try
            {
                await warehouseStockService.Decrease(
                    warehouseId,
                    medicineId,
                    quantity);

                return Ok(new
                {
                    message = "Warehouse stock decreased successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Get medicines with low stock
        [HttpGet("GetLowStock/{warehouseId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetLowStock(int warehouseId,
            [FromQuery] int minimumQuantity)
        {
            List<WarehouseStockDto> stocks = await warehouseStockService.GetLowStock( warehouseId,minimumQuantity);

            return Ok(stocks);


        }


    }


}
