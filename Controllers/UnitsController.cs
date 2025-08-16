using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Models.Units;
using WarehouseManagement.Services.Interfaces.WarehouseManagementAPI.Services.Interfaces;
using WarehouseManagementAPI.Dto.Common;

namespace WarehouseManagementAPI.Controllers
{
    [ApiController]
    [Route("api/units")]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitService _service;

        public UnitsController(IUnitService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<List<Unit>>> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Unit>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpGet("options")]
        public async Task<ActionResult<List<OptionDto>>> Options()
            => Ok(await _service.GetFilterOptionsAsync());

        [HttpPost]
        public async Task<ActionResult<Unit>> Create([FromBody] Unit unit)
        {
            var result = await _service.CreateAsync(unit);
            if (!result.IsSuccess)
                return BadRequest(result.Errors?.ToArray() ?? new[] { result.ErrorMessage ?? "Ошибка валидации" });
 
            return Ok(unit);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Unit>> Update(int id, [FromBody] Unit unit)
        {
            if (id != unit.Id)
                return BadRequest("Id в пути и в теле не совпадают");

            var result = await _service.UpdateAsync(unit);
            if (!result.IsSuccess)
                return BadRequest(result.Errors?.ToArray() ?? new[] { result.ErrorMessage ?? "Ошибка валидации" });
   
            return Ok(unit);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.Errors?.ToArray() ?? new[] { result.ErrorMessage ?? "Удаление невозможно" });
  
            return NoContent();
        }
    }
}