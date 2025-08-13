using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagement.Models.Receipts;

namespace WarehouseManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptService;
        private readonly ILogger<ReceiptsController> _logger;

        public ReceiptsController(
            IReceiptService receiptService,
            ILogger<ReceiptsController> logger)
        {
            _receiptService = receiptService;
            _logger = logger;
        }

        // GET: api/receipts
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var receipts = await _receiptService.GetAllAsync();
                return Ok(receipts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка поступлений");
                return StatusCode(500, new { error = "Произошла ошибка при загрузке поступлений" });
            }
        }
        // GET: api/receipts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var receipt = await _receiptService.GetByIdWithDetailsAsync(id);
                if (receipt == null)
                {
                    return NotFound(new { error = "Поступление не найдено" });
                }
                return Ok(receipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении поступления");
                return StatusCode(500, new { error = "Произошла ошибка при загрузке поступления" });
            }
        }

        // POST: api/receipts
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Receipt receipt)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var created = await _receiptService.CreateAsync(receipt);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании поступления");
                return StatusCode(500, new { error = "Произошла ошибка при создании поступления" });
            }
        }

        // PUT: api/receipts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Receipt receipt)
        {
            try
            {
                if (id != receipt.Id)
                {
                    return BadRequest(new { error = "ID не совпадает" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var updated = await _receiptService.UpdateAsync(receipt);
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении поступления");
                return StatusCode(500, new { error = "Произошла ошибка при обновлении поступления" });
            }
        }

        // DELETE: api/receipts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _receiptService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { error = "Поступление не найдено" });
                }
                return Ok(new { message = "Поступление удалено" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении поступления");
                return StatusCode(500, new { error = "Произошла ошибка при удалении поступления" });
            }
        }
    }
}