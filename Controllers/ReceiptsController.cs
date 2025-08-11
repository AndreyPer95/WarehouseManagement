using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagement.ViewModels.Receipts;

namespace WarehouseManagement.Controllers
{
    public class ReceiptsController : Controller
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

        public async Task<IActionResult> Index()
        {
            try
            {
                var receipts = await _receiptService.GetAllAsync();
                var viewModels = receipts.Select(r => new ReceiptViewModel
                {
                    Id = r.Id,
                    Number = r.Number,
                    Date = r.Date,
                    Resources = r.ReceiptResources?.Select(rr => new ReceiptResourceViewModel
                    {
                        Id = rr.Id,
                        ResourceId = rr.ResourceId,
                        ResourceName = rr.Resource?.Name,
                        UnitId = rr.UnitId,
                        UnitName = rr.Unit?.Name,
                        Quantity = rr.Quantity
                    }).ToList() ?? new List<ReceiptResourceViewModel>()
                }).ToList();

                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка поступлений");
                TempData["Error"] = "Произошла ошибка при загрузке поступлений";
                return View(new List<ReceiptViewModel>());
            }
        }
    }
}
