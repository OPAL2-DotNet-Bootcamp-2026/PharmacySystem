using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PharmacyStockController : ControllerBase
    {
        private readonly PharmacyStockService pharmacyStockService;

        public PharmacyStockController(PharmacyStockService pharmacyStockService)
        {
            this.pharmacyStockService = pharmacyStockService;
        }

        [HttpGet("by-pharmacy/{pharmacyId}")]              // GET api/PharmacyStock/by-pharmacy/5
        public async Task<IActionResult> GetByPharmacy(int pharmacyId)
            => Ok(await pharmacyStockService.GetByPharmacy(pharmacyId));

        [HttpGet("by-medicine/{medicineId}")]             // GET api/PharmacyStock/by-medicine/5
        public async Task<IActionResult> GetByMedicine(int medicineId)
            => Ok(await pharmacyStockService.GetByMedicine(medicineId));

        [HttpGet("low-stock/{pharmacyId}")]               // GET api/PharmacyStock/low-stock/5
        public async Task<IActionResult> GetLowStock(int pharmacyId)
            => Ok(await pharmacyStockService.GetLowStock(pharmacyId));
    }
}