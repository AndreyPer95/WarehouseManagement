using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagement.Models.Receipts;
using WarehouseManagementAPI.Dto.Common;
using WarehouseManagementAPI.Dto.Receipts;
using WarehouseManagementAPI.Services.Interfaces;
using WarehouseManagement.Services.Interfaces.WarehouseManagementAPI.Services.Interfaces;


namespace WarehouseManagementAPI.Controllers
{
    [ApiController]
    [Route("api/receipts")]
    public class ReceiptsResourceController : ControllerBase
    {
        private readonly IReceiptResourceService _service;
        private readonly IResourceService _resources;
        private readonly IUnitService _units;

        public ReceiptsResourceController(
            IReceiptResourceService service,
            IResourceService resources,
            IUnitService units)
        {
            _service = service;
            _resources = resources;
            _units = units;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReceiptWithLinesDto>>> Get(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] List<string>? numbers,
            [FromQuery] List<int>? resourceIds,
            [FromQuery] List<int>? unitIds)
        {
            var filter = new ReceiptFilter
            {
                From = from,
                To = to,
                Numbers = numbers,
                ResourceIds = resourceIds,
                UnitIds = unitIds
            };

            var data = await _service.GetReceiptsAsync(filter);
            return Ok(data);
        }

        // --- значения для мультиселектов (НЕ зависят от периода) ---
        [HttpGet("filters/numbers")]
        public async Task<ActionResult<List<string>>> Numbers()
            => Ok(await _service.GetAllReceiptNumbersAsync());

        [HttpGet("filters/resources")]
        public async Task<ActionResult<List<OptionDto>>> Resources()
            => Ok(await _resources.GetFilterOptionsAsync());

        [HttpGet("filters/units")]
        public async Task<ActionResult<List<OptionDto>>> Units()
            => Ok(await _units.GetFilterOptionsAsync());

        [HttpPost]
        public async Task<ActionResult<Receipt>> Create([FromBody] Receipt receipt)
        {
            var result = await _service.CreateReceiptAsync(receipt);
            if (!result.IsSuccess)
                return BadRequest(result.Errors.ToArray() ?? new[] { result.ErrorMessage ?? "Ошибка валидации" });
            //var resultWithLine = await _service.AddResourceToReceiptAsync(line);
            //if (!result.IsSuccess)
            //    return BadRequest(result.Errors.ToArray() ?? new[] { result.ErrorMessage ?? "Ошибка валидации" });

            return Ok(result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Receipt>> Update(int id, [FromBody] Receipt receipt)
        {
            if (id != receipt.Id)
                return BadRequest("Id в пути и в теле не совпадают");

            var result = await _service.UpdateReceiptAsync(receipt);
            if (!result.IsSuccess)
                return BadRequest(result.Errors.ToArray() ?? new[] { result.ErrorMessage ?? "Ошибка валидации" });

            return Ok(result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteReceiptAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.Errors.ToArray() ?? new[] { result.ErrorMessage ?? "Удаление невозможно" });

            return NoContent();
        }

        [HttpPost("lines")]
        public async Task<ActionResult<ReceiptResource>> AddLine([FromBody] ReceiptResource line)
        {
            var result = await _service.AddResourceToReceiptAsync(line);
            if (!result.IsSuccess)
                return BadRequest(result.Errors.ToArray() ?? new[] { result.ErrorMessage ?? "Ошибка валидации" });

            return Ok(result.Data);
        }

        [HttpPut("lines/replace/{receiptId:int}")]
        public async Task<IActionResult> ReplaceLines(int receiptId, [FromBody] List<ReceiptResource> lines)
        {
            var result = await _service.ReplaceReceiptLinesAsync(receiptId, lines);
            if (!result.IsSuccess)
                return BadRequest(result.Errors.ToArray() ?? new[] { result.ErrorMessage ?? "Сохранить изменения не удалось" });

            return NoContent();
        }

        [HttpDelete("lines/{lineId:int}")]
        public async Task<IActionResult> DeleteLine(int lineId)
        {
            var result = await _service.DeleteReceiptLineAsync(lineId);
            if (!result.IsSuccess)
                return BadRequest(result.Errors.ToArray() ?? new[] { result.ErrorMessage ?? "Удалить строку не удалось" });

            return NoContent();
        }
    }
}