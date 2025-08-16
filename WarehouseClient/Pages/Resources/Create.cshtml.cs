using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseClient.Models;
using WarehouseClient.Services;

namespace WarehouseClient.Pages.Resources
{
    public class CreateModel : PageModel
    {
        private readonly WarehouseApiService _apiService;

        public CreateModel(WarehouseApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty]
        public Resource Resource { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _apiService.CreateResourceAsync(Resource);
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}