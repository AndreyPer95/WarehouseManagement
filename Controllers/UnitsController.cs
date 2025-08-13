using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagement.Models.Units;
using WarehouseManagement.Models;

namespace WarehouseManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitService _unitService;
        private readonly ILogger<UnitsController> _logger;

        public UnitsController(
            IUnitService unitService,
            ILogger<UnitsController> logger)
        {
            _unitService = unitService;
            _logger = logger;
        }

        // GET: api/units
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var units = await _unitService.GetAllAsync();
                return Ok(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка единиц измерения");
                return StatusCode(500, new { error = "Произошла ошибка при загрузке единиц измерения" });
            }
        }

        // GET: api/units/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            try
            {
                var units = await _unitService.GetActiveAsync();
                return Ok(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении активных единиц измерения");
                return StatusCode(500, new { error = "Произошла ошибка при загрузке единиц измерения" });
            }
        }

        // GET: api/units/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var unit = await _unitService.GetByIdAsync(id);
                if (unit == null)
                {
                    return NotFound(new { error = "Единица измерения не найдена" });
                }
                return Ok(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении единицы измерения");
                return StatusCode(500, new { error = "Произошла ошибка при загрузке единицы измерения" });
            }
        }

        // POST: api/units
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Unit unit)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var created = await _unitService.CreateAsync(unit);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании единицы измерения");
                return StatusCode(500, new { error = "Произошла ошибка при создании единицы измерения" });
            }
        }

        // PUT: api/units/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Unit unit)
        {
            try
            {
                if (id != unit.Id)
                {
                    return BadRequest(new { error = "ID не совпадает" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updated = await _unitService.UpdateAsync(unit);
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении единицы измерения");
                return StatusCode(500, new { error = "Произошла ошибка при обновлении единицы измерения" });
            }
        }

        // PUT: api/units/5/archive
        [HttpPut("{id}/archive")]
        public async Task<IActionResult> Archive(int id)
        {
            try
            {
                var result = await _unitService.ArchiveAsync(id);
                if (!result)
                {
                    return NotFound(new { error = "Единица измерения не найдена" });
                }
                return Ok(new { message = "Единица измерения архивирована" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при архивировании единицы измерения");
                return StatusCode(500, new { error = "Произошла ошибка при архивировании единицы измерения" });
            }
        }

        // DELETE: api/units/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var canDelete = await _unitService.CanDeleteAsync(id);
                if (!canDelete)
                {
                    return BadRequest(new { error = "Единица измерения используется и не может быть удалена" });
                }

                await _unitService.ArchiveAsync(id);
                return Ok(new { message = "Единица измерения архивирована" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении единицы измерения");
                return StatusCode(500, new { error = "Произошла ошибка при удалении единицы измерения" });
            }
        }
    }
}