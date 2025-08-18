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
        public SelectList NumbersList { get; set; } = new(Enumerable.Empty<SelectListItem>());

        [BindProperty(SupportsGet = true)]
        public DateTime? DateFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DateTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<string>? Numbers { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<int>? ResourceIds { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<int>? UnitIds { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Получаем фильтрованные поступления с сервера
                Receipts = await _apiService.GetFilteredReceiptsAsync(
                    DateFrom, 
                    DateTo, 
                    Numbers, 
                    ResourceIds, 
                    UnitIds) ?? new List<Receipt>();

                // Получаем все доступные номера документов для фильтра
                var allNumbers = await _apiService.GetReceiptNumbersAsync() ?? new List<string>();
                NumbersList = new SelectList(allNumbers.Select(n => new SelectListItem 
                { 
                    Value = n, 
                    Text = n,
                    Selected = Numbers?.Contains(n) ?? false
                }), "Value", "Text");

                // Получаем списки ресурсов и единиц для фильтров
                var resources = await _apiService.GetResourcesAsync() ?? new List<Resource>();
                var units = await _apiService.GetUnitsAsync() ?? new List<Unit>();

                ResourcesList = new SelectList(resources.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name,
                    Selected = ResourceIds?.Contains(r.Id) ?? false
                }), "Value", "Text");

                UnitsList = new SelectList(units.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Name,
                    Selected = UnitIds?.Contains(u.Id) ?? false
                }), "Value", "Text");
            }
            catch (Exception ex)
            {
                // Логируем ошибку и устанавливаем пустые данные
                Console.WriteLine($"Error loading receipts: {ex.Message}");
                Receipts = new List<Receipt>();
                ResourcesList = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
                UnitsList = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
                NumbersList = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
            }
        }
    }
}