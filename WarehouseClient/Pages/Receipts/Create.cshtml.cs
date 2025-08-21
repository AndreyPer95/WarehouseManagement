using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WarehouseClient.Models;
using WarehouseClient.Services;

namespace WarehouseClient.Pages.Receipts;

public class CreateModel : PageModel
{
    private readonly WarehouseApiService _api;
    public CreateModel(WarehouseApiService api) => _api = api;

    [BindProperty]
    public Receipt Receipt { get; set; } = new() { Date = DateTime.UtcNow.Date };

    // строки приходят из формы как Lines[i].ResourceId / UnitId / Quantity
    [BindProperty]
    public List<ReceiptResource> Lines { get; set; } = new();

    // списки для селектов
    public List<SelectListItem> ResourceOptions { get; private set; } = new();
    public List<SelectListItem> UnitOptions { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var resources = await _api.GetActiveResourcesAsync();
        var units = await _api.GetActiveUnitsAsync();

        ResourceOptions = resources
            .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name })
            .ToList();

        UnitOptions = units
            .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name })
            .ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var created = await _api.CreateReceiptAsync(Receipt);
        Receipt.Id = created.Id;

        var cleanLines = (Lines ?? new())
            .Where(l => l.ResourceId > 0 && l.UnitId > 0 && l.Quantity > 0)
            .Select(l => new ReceiptResource
            {
                ReceiptId = Receipt.Id,
                ResourceId = l.ResourceId,
                UnitId = l.UnitId,
                Quantity = l.Quantity
            })
            .ToList();

        await _api.ReplaceReceiptLinesAsync(Receipt.Id, cleanLines);

        return RedirectToPage("Index");
    }
}