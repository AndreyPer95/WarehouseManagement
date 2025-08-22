using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WarehouseClient.Models.Dto;
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

        public List<WarehouseBalanceRowDto> Balances { get; set; } = new();
        public SelectList ResourcesList { get; set; } = new(Enumerable.Empty<SelectListItem>());
        public SelectList UnitsList { get; set; } = new(Enumerable.Empty<SelectListItem>());

        [BindProperty(SupportsGet = true)]
        public List<int>? ResourceIds { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<int>? UnitIds { get; set; }

        public async Task OnGetAsync()
        {
            Balances = await _apiService.GetWarehouseBalanceAsync(ResourceIds, UnitIds);

            var resources = await _apiService.GetResourcesAsync();
            var units = await _apiService.GetUnitsAsync();

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
    }
}