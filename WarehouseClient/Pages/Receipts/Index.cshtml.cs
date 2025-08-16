using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WarehouseClient.Models;
using WarehouseClient.Services;

namespace WarehouseClient.Pages.Receipts
{
    public class IndexModel : PageModel
    {
        private readonly WarehouseApiService _apiService;

        public IndexModel(WarehouseApiService apiService)
        {
            _apiService = apiService;
        }

        public List<Receipt> Receipts { get; set; } = new();
        public SelectList ResourcesList { get; set; } = new(Enumerable.Empty<SelectListItem>());
        public SelectList UnitsList { get; set; } = new(Enumerable.Empty<SelectListItem>());

        [BindProperty(SupportsGet = true)]
        public DateTime? DateFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DateTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Number { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? ResourceId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? UnitId { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Получаем все поступления
                var allReceipts = await _apiService.GetReceiptsAsync();
                
                // Фильтруем если нужно
                Receipts = allReceipts ?? new List<Receipt>();
                
                if (DateFrom.HasValue)
                {
                    Receipts = Receipts.Where(r => r.Date >= DateFrom.Value).ToList();
                }
                
                if (DateTo.HasValue)
                {
                    Receipts = Receipts.Where(r => r.Date <= DateTo.Value).ToList();
                }
                
                if (!string.IsNullOrEmpty(Number))
                {
                    Receipts = Receipts.Where(r => r.Number.Contains(Number, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                
                if (ResourceId.HasValue)
                {
                    Receipts = Receipts.Where(r => r.ReceiptResources != null && 
                        r.ReceiptResources.Any(rr => rr.ResourceId == ResourceId.Value)).ToList();
                }
                
                if (UnitId.HasValue)
                {
                    Receipts = Receipts.Where(r => r.ReceiptResources != null && 
                        r.ReceiptResources.Any(rr => rr.UnitId == UnitId.Value)).ToList();
                }

                // Получаем списки для фильтров
                var resources = await _apiService.GetResourcesAsync() ?? new List<Resource>();
                var units = await _apiService.GetUnitsAsync() ?? new List<Unit>();

                ResourcesList = new SelectList(resources, "Id", "Name", ResourceId);
                UnitsList = new SelectList(units, "Id", "Name", UnitId);
            }
            catch (Exception ex)
            {
                // Логируем ошибку и устанавливаем пустые данные
                Console.WriteLine($"Error loading receipts: {ex.Message}");
                Receipts = new List<Receipt>();
                ResourcesList = new SelectList(Enumerable.Empty<Resource>(), "Id", "Name");
                UnitsList = new SelectList(Enumerable.Empty<Unit>(), "Id", "Name");
            }
        }
    }
}