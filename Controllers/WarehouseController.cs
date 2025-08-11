using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagement.ViewModels.Warehouse;
using WarehouseManagement.Models;

namespace WarehouseManagement.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly IWarehouseService _warehouseService;
        private readonly ILogger<WarehouseController> _logger;

        public WarehouseController(
            IWarehouseService warehouseService,
            ILogger<WarehouseController> logger)
        {
            _warehouseService = warehouseService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var balances = await _warehouseService.GetBalanceAsync();
                
                var viewModels = balances.Select(b => new WarehouseBalanceViewModel
                {
                    Id = b.Id,
                    ResourceName = b.Resource?.Name ?? "Неизвестный ресурс",
                    UnitName = b.Unit?.Name ?? "Неизвестная ед. изм.",
                    Quantity = b.Quantity,
                    ResourceId = b.ResourceId,
                    UnitId = b.UnitId
                }).ToList();

                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении баланса склада");
                TempData["Error"] = "Произошла ошибка при загрузке данных склада";
                return View(new List<WarehouseBalanceViewModel>());
            }
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
