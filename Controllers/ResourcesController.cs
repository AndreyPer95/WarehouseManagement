using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Models.Resources;
using WarehouseManagementAPI.Dto.Common;
using WarehouseManagementAPI.Services.Interfaces;

namespace WarehouseManagementAPI.Controllers
{
    [ApiController]
    [Route("api/resources")]
    public class ResourcesController : ControllerBase
    {
        private readonly IResourceService _service;

        public ResourcesController(IResourceService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<List<Resource>>> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Resource>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpGet("options")]
        public async Task<ActionResult<List<OptionDto>>> Options()
            => Ok(await _service.GetFilterOptionsAsync());

        [HttpPost]
        public async Task<ActionResult<Resource>> Create([FromBody] Resource resource)
        {
            var result = await _service.CreateAsync(resource);
            if (!result.IsSuccess)
                return BadRequest(result.Errors.ToArray() ?? [result.ErrorMessage ?? "Ошибка валидации"]);

            return Ok(resource);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Resource>> Update(int id, [FromBody] Resource resource)
        {
            if (id != resource.Id)
                return BadRequest("Id в пути и в теле не совпадают");

            var result = await _service.UpdateAsync(resource);
            if (!result.IsSuccess)
                return BadRequest(result.Errors.ToArray() ?? [result.ErrorMessage ?? "Ошибка валидации"]);

            return Ok(resource);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.Errors.ToArray() ?? [result.ErrorMessage ?? "Удаление невозможно"]);

            return NoContent();
        }
    }
}