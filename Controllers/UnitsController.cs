using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagement.Models.Units;
using WarehouseManagement.ViewModels.Units;

namespace WarehouseManagement.Controllers
{
    public class UnitsController : Controller
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

        public async Task<IActionResult> Index()
        {
            try
            {
                var units = await _unitService.GetAllAsync();
                var viewModels = units.Select(u => new UnitViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Status = u.Status,
                    CanDelete = _unitService.CanDeleteAsync(u.Id).Result
                }).ToList();

                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка единиц измерения");
                TempData["Error"] = "Произошла ошибка при загрузке единиц измерения";
                return View(new List<UnitViewModel>());
            }
        }
    }
}
