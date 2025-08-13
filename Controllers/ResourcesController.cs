using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagement.Models.Resources;
using WarehouseManagement.Models;

namespace WarehouseManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourcesController : ControllerBase
    {
        private readonly IResourceService _resourceService;
        private readonly ILogger<ResourcesController> _logger;

        public ResourcesController(
            IResourceService resourceService,
            ILogger<ResourcesController> logger)
        {
            _resourceService = resourceService;
            _logger = logger;
        }

        // GET: api/resources
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var resources = await _resourceService.GetAllAsync();
                return Ok(resources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка ресурсов");
                return StatusCode(500, new { error = "Произошла ошибка при загрузке ресурсов" });
            }
        }

        // GET: api/resources/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            try
            {
                var resources = await _resourceService.GetActiveAsync();
                return Ok(resources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении активных ресурсов");
                return StatusCode(500, new { error = "Произошла ошибка при загрузке ресурсов" });
            }
        }

        // GET: api/resources/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var resource = await _resourceService.GetByIdAsync(id);
                if (resource == null)
                {
                    return NotFound(new { error = "Ресурс не найден" });
                }
                return Ok(resource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении ресурса");
                return StatusCode(500, new { error = "Произошла ошибка при загрузке ресурса" });
            }
        }

        // POST: api/resources
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Resource resource)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var created = await _resourceService.CreateAsync(resource);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании ресурса");
                return StatusCode(500, new { error = "Произошла ошибка при создании ресурса" });
            }
        }

        // PUT: api/resources/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Resource resource)
        {
            try
            {
                if (id != resource.Id)
                {
                    return BadRequest(new { error = "ID не совпадает" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updated = await _resourceService.UpdateAsync(resource);
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении ресурса");
                return StatusCode(500, new { error = "Произошла ошибка при обновлении ресурса" });
            }
        }

        // PUT: api/resources/5/archive
        [HttpPut("{id}/archive")]
        public async Task<IActionResult> Archive(int id)
        {
            try
            {
                var result = await _resourceService.ArchiveAsync(id);
                if (!result)
                {
                    return NotFound(new { error = "Ресурс не найден" });
                }
                return Ok(new { message = "Ресурс архивирован" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при архивировании ресурса");
                return StatusCode(500, new { error = "Произошла ошибка при архивировании ресурса" });
            }
        }

        // DELETE: api/resources/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var canDelete = await _resourceService.CanDeleteAsync(id);
                if (!canDelete)
                {
                    return BadRequest(new { error = "Ресурс используется и не может быть удалён" });
                }

                await _resourceService.ArchiveAsync(id);
                return Ok(new { message = "Ресурс архивирован" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении ресурса");
                return StatusCode(500, new { error = "Произошла ошибка при удалении ресурса" });
            }
        }
    }
}