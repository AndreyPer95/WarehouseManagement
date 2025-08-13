using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Services.Interfaces;

namespace WarehouseManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly ILogger<WarehouseController> _logger;

        public WarehouseController(
            IWarehouseService warehouseService,
            ILogger<WarehouseController> logger)
        {
            _warehouseService = warehouseService;
            _logger = logger;
        }

        // GET: api/warehouse/balance
        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            try
            {
                var balances = await _warehouseService.GetBalanceAsync();
                return Ok(balances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении баланса склада");
                return StatusCode(500, new { error = "Произошла ошибка при загрузке данных склада" });
            }
        }

        // GET: api/warehouse/balance/5/3
        [HttpGet("balance/{resourceId}/{unitId}")]
        public async Task<IActionResult> GetBalanceByResourceAndUnit(int resourceId, int unitId)
        {
            try
            {
                var balance = await _warehouseService.GetBalanceByResourceAndUnitAsync(resourceId, unitId);
                if (balance == null)
                {
                    return NotFound(new { error = "Баланс не найден" });
                }
                return Ok(balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении баланса");
                return StatusCode(500, new { error = "Произошла ошибка при загрузке баланса" });
            }
        }

        // GET: api/warehouse/check/5/3/100
        [HttpGet("check/{resourceId}/{unitId}/{quantity}")]
        public async Task<IActionResult> CheckAvailability(int resourceId, int unitId, decimal quantity)
        {
            try
            {
                var available = await _warehouseService.CheckAvailabilityAsync(resourceId, unitId, quantity);
                return Ok(new { available });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке доступности");
                return StatusCode(500, new { error = "Произошла ошибка при проверке доступности" });
            }
        }
    }
}