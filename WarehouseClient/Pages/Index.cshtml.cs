using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WarehouseClient.Models;
using WarehouseClient.Services;

namespace WarehouseClient.Pages
{
    public class IndexModel : PageModel
    {
        private readonly WarehouseApiService _apiService;

        public IndexModel(WarehouseApiService apiService)
        {
            _apiService = apiService;
        }

        public List<WarehouseBalance> Balances { get; set; } = new();
        public SelectList ResourcesList { get; set; } = new(Enumerable.Empty<SelectListItem>());
        public SelectList UnitsList { get; set; } = new(Enumerable.Empty<SelectListItem>());

        [BindProperty(SupportsGet = true)]
        public int? ResourceId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? UnitId { get; set; }

        public async Task OnGetAsync()
        {
            // Получаем все балансы
            var allBalances = await _apiService.GetWarehouseBalanceAsync();
            
            // Фильтруем если нужно
            Balances = allBalances;
            if (ResourceId.HasValue)
            {
                Balances = Balances.Where(b => b.ResourceId == ResourceId.Value).ToList();
            }
            if (UnitId.HasValue)
            {
                Balances = Balances.Where(b => b.UnitId == UnitId.Value).ToList();
            }

            // Получаем списки для фильтров
            var resources = await _apiService.GetResourcesAsync();
            var units = await _apiService.GetUnitsAsync();

            ResourcesList = new SelectList(resources, "Id", "Name", ResourceId);
            UnitsList = new SelectList(units, "Id", "Name", UnitId);
        }
    }
}