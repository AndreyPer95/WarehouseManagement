using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseClient.Models;
using WarehouseClient.Services;

namespace WarehouseClient.Pages.Resources
{
    public class IndexModel : PageModel
    {
        private readonly WarehouseApiService _apiService;

        public IndexModel(WarehouseApiService apiService)
        {
            _apiService = apiService;
        }

        public List<Resource> Resources { get; set; } = new();

        public async Task OnGetAsync()
        {
            Resources = await _apiService.GetResourcesAsync();
        }

        public async Task<IActionResult> OnPostArchiveAsync(int id)
        {
            await _apiService.ArchiveResourceAsync(id);
            return RedirectToPage();
        }
    }
}