using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Warehouse;
using Pharmacy_System.DTOs.WarehouseStock;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require login
    public class WarehouseController : ControllerBase
    {
        private readonly WarehouseService warehouseService;



        public WarehouseController(WarehouseService _warehouseService)
        {
            warehouseService = _warehouseService;
        }


        // Get all warehouses
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllWarehouses()
        {
            List<WarehouseDto> warehouses = await warehouseService.GetAllWarehouses();

            return Ok(warehouses);
        }


        // Get one warehouse by ID
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

        // Add a new warehouse
        [HttpPost("Add")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddWarehouse(
            [FromBody] CreateWarehouseDto dto)
        {
            int warehouseId = await warehouseService.AddWarehouse(dto);

            return Ok(new
            {
                message = "Warehouse added successfully",
                WarehouseID = warehouseId
            });
        }

        // Update warehouse information
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


        // Get medicines available in the warehouse
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


        // Get medicines expiring within 30 days
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




        
