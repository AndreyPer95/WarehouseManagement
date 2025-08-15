using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagement.Models.Receipts;

namespace WarehouseManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptResourceService _receiptResourceService;
        private readonly ILogger<ReceiptsController> _logger;

        public ReceiptsController(
            IReceiptResourceService receiptResourceService,
            ILogger<ReceiptsController> logger)
        {
            _receiptResourceService = receiptResourceService;
            _logger = logger;
        }

        // GET: api/receipts
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var receipts = await _receiptResourceService.GetAllReceiptsAsync();
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
                var receipt = await _receiptResourceService.GetReceiptByIdAsync(id);
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
                
                var result = await _receiptResourceService.CreateReceiptAsync(receipt);
                
                if (!result.IsSuccess)
                {
                    return BadRequest(new { error = result.ErrorMessage, errors = result.Errors });
                }
                
                return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
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
                
                var result = await _receiptResourceService.UpdateReceiptAsync(receipt);
                
                if (!result.IsSuccess)
                {
                    return BadRequest(new { error = result.ErrorMessage, errors = result.Errors });
                }
                
                return Ok(result.Data);
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
                var result = await _receiptResourceService.DeleteReceiptAsync(id);
                
                if (!result.IsSuccess)
                {
                    return BadRequest(new { error = result.ErrorMessage, errors = result.Errors });
                }
                
                return Ok(new { message = "Поступление удалено" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении поступления");
                return StatusCode(500, new { error = "Произошла ошибка при удалении поступления" });
            }
        }        
        // GET: api/receipts/5/resources
        [HttpGet("{id}/resources")]
        public async Task<IActionResult> GetReceiptResources(int id)
        {
            try
            {
                var receipt = await _receiptResourceService.GetReceiptByIdAsync(id);
                if (receipt == null)
                {
                    return NotFound(new { error = "Поступление не найдено" });
                }
                
                var resources = await _receiptResourceService.GetReceiptResourcesAsync(id);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении ресурсов поступления");
                return StatusCode(500, new { error = "Произошла ошибка при загрузке ресурсов" });
            }
        }
        
        // POST: api/receipts/5/resources
        [HttpPost("{id}/resources")]
        public async Task<IActionResult> AddResource(int id, [FromBody] ReceiptResource resource)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }                
                resource.ReceiptId = id;
                var result = await _receiptResourceService.AddResourceToReceiptAsync(resource);
                
                if (!result.IsSuccess)
                {
                    return BadRequest(new { error = result.ErrorMessage, errors = result.Errors });
                }
                
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении ресурса в поступление");
                return StatusCode(500, new { error = "Произошла ошибка при добавлении ресурса" });
            }
        }
        
        // PUT: api/receipts/5/resources
        [HttpPut("{id}/resources")]
        public async Task<IActionResult> UpdateResources(int id, [FromBody] List<ReceiptResource> resources)
        {
            try
            {
                var result = await _receiptResourceService.UpdateReceiptResourcesAsync(id, resources);
                
                if (!result.IsSuccess)
                {
                    return BadRequest(new { error = result.ErrorMessage, errors = result.Errors });
                }
                
                return Ok(new { message = "Ресурсы обновлены" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении ресурсов поступления");
                return StatusCode(500, new { error = "Произошла ошибка при обновлении ресурсов" });
            }
        }
        
        // DELETE: api/receipts/resources/5
        [HttpDelete("resources/{resourceId}")]
        public async Task<IActionResult> DeleteResource(int resourceId)
        {
            try
            {
                var result = await _receiptResourceService.DeleteReceiptResourceAsync(resourceId);
                
                if (!result.IsSuccess)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }
                
                return Ok(new { message = "Ресурс удален" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении ресурса из документа");
                return StatusCode(500, new { error = "Произошла ошибка при удалении ресурса" });
            }
        }
    }
}