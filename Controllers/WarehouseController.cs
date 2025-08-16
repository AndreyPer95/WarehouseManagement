using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagementAPI.Dto.Common;
using WarehouseManagementAPI.Dto.Warehouse;

namespace WarehouseManagementAPI.Controllers
{
    [ApiController]
    [Route("api/warehouse")]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouse;
        private readonly ResourceService _resources;
        private readonly UnitService _units;

        public WarehouseController(
            IWarehouseService warehouse,
            ResourceService resources,
            UnitService units)
        {
            _warehouse = warehouse;
            _resources = resources;
            _units = units;
        }

        // баланс склада с серверной фильтрацией (мультиселекты)
        [HttpGet("balance")]
        public async Task<ActionResult<List<WarehouseBalanceRowDto>>> GetBalance(
            [FromQuery] List<int>? resourceIds,
            [FromQuery] List<int>? unitIds)
        {
            var data = await _warehouse.GetBalanceAsync(resourceIds, unitIds);
            return Ok(data);
        }

        // значения для мультиселектов (НЕ зависят от периода)
        [HttpGet("filters/resources")]
        public async Task<ActionResult<List<OptionDto>>> Resources()
            => Ok(await _resources.GetFilterOptionsAsync());

        [HttpGet("filters/units")]
        public async Task<ActionResult<List<OptionDto>>> Units()
            => Ok(await _units.GetFilterOptionsAsync());
    }
}