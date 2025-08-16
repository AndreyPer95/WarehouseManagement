using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WarehouseClient.Models;
using WarehouseClient.Services;

namespace WarehouseClient.Pages.Receipts
{
    public class CreateModel : PageModel
    {
        private readonly WarehouseApiService _apiService;

        public CreateModel(WarehouseApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty]
        public Receipt Receipt { get; set; } = new();

        [BindProperty]
        public List<int> ResourceIds { get; set; } = new();

        [BindProperty]
        public List<int> UnitIds { get; set; } = new();

        [BindProperty]
        public List<decimal> Quantities { get; set; } = new();

        public SelectList ResourcesList { get; set; } = new(Enumerable.Empty<SelectListItem>());
        public SelectList UnitsList { get; set; } = new(Enumerable.Empty<SelectListItem>());

        public async Task OnGetAsync()
        {
            var resources = await _apiService.GetActiveResourcesAsync();
            var units = await _apiService.GetActiveUnitsAsync();

            ResourcesList = new SelectList(resources, "Id", "Name");
            UnitsList = new SelectList(units, "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            try
            {
                // Формируем список ресурсов для поступления
                Receipt.ReceiptResources = new List<ReceiptResource>();
                for (int i = 0; i < ResourceIds.Count; i++)
                {
                    if (ResourceIds[i] > 0 && UnitIds[i] > 0 && Quantities[i] > 0)
                    {
                        Receipt.ReceiptResources.Add(new ReceiptResource
                        {
                            ResourceId = ResourceIds[i],
                            UnitId = UnitIds[i],
                            Quantity = Quantities[i]
                        });
                    }
                }

                if (!Receipt.ReceiptResources.Any())
                {
                    ModelState.AddModelError(string.Empty, "Добавьте хотя бы один ресурс");
                    await OnGetAsync();
                    return Page();
                }

                await _apiService.CreateReceiptAsync(Receipt);
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await OnGetAsync();
                return Page();
            }
        }
    }
}