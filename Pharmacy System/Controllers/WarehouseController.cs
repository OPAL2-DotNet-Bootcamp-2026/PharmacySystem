using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Warehouse;
using Pharmacy_System.DTOs.WarehouseStock;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WarehouseController : ControllerBase
    {
        private readonly WarehouseService warehouseService;



        public WarehouseController(WarehouseService _warehouseService)
        {
            warehouseService = _warehouseService;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllWarehouses()
        {
            List<WarehouseDto> warehouses = await warehouseService.GetAllWarehouses();

            return Ok(warehouses);
        }


        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetWarehouseById(int id)
        {
            WarehouseDto? warehouse = await warehouseService.GetWarehouseById(id);

            if (warehouse == null)
            {
                return NotFound(new
                {
                    message = $"Warehouse with ID {id} was not found."
                });
            }

            return Ok(warehouse);
        }

        
        [HttpPost("Add")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddWarehouse([FromBody] CreateWarehouseDto dto)
        {
            int warehouseId = await warehouseService.AddWarehouse(dto);

            return Ok(new
            {
                message = "Warehouse added successfully",
                WarehouseID = warehouseId
            });
        }


        [HttpPut("Update/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateWarehouse(int id,
            [FromBody] UpdateWarehouseDto dto)
        {
            bool updated = await warehouseService.UpdateWarehouse(id, dto);

            if (!updated)
            {
                return NotFound(new
                {
                    message = $"Warehouse with ID {id} was not found"
                });
            }

            return Ok(new
            {
                message = "Warehouse updated successfully"
            });
        }


        [HttpGet("GetStock/{warehouseId}")]
        public async Task<IActionResult> GetStock(int warehouseId)
        {
            try
            {
                List<WarehouseStockDto> stocks = await warehouseService.GetStock(warehouseId);

                return Ok(stocks);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }


        [HttpGet("GetExpiringItems/{warehouseId}")]
        [Authorize(Roles = "Admin,Manager,Pharmacist")]
        public async Task<IActionResult> GetExpiringItems(int warehouseId)
        {
            try
            {
                List<WarehouseStockDto> stocks = await warehouseService.GetExpiringItems(warehouseId);

                return Ok(stocks);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }


        }

    }
}




        
