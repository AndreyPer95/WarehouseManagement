using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseClient.Models;
using WarehouseClient.Services;

namespace WarehouseClient.Pages.Units
{
    public class IndexModel : PageModel
    {
        private readonly WarehouseApiService _apiService;

        public IndexModel(WarehouseApiService apiService)
        {
            _apiService = apiService;
        }

        public List<Unit> Units { get; set; } = new();

        public async Task OnGetAsync()
        {
            Units = await _apiService.GetUnitsAsync();
        }

        public async Task<IActionResult> OnPostArchiveAsync(int id)
        {
            await _apiService.ArchiveUnitAsync(id);
            return RedirectToPage();
        }
    }
}