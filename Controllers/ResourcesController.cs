using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagement.Models.Resources;
using WarehouseManagement.ViewModels.Resources;

namespace WarehouseManagement.Controllers
{
    public class ResourcesController : Controller
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

        public async Task<IActionResult> Index()
        {
            try
            {
                var resources = await _resourceService.GetAllAsync();
                var viewModels = resources.Select(r => new ResourceViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Status = r.Status,
                    CanDelete = _resourceService.CanDeleteAsync(r.Id).Result
                }).ToList();

                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка ресурсов");
                TempData["Error"] = "Произошла ошибка при загрузке ресурсов";
                return View(new List<ResourceViewModel>());
            }
        }
    }
}
