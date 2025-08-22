using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseClient.Models;
using WarehouseClient.Services;

namespace WarehouseClient.Pages.Receipts;

public class CreateModel : PageModel
{
    private readonly WarehouseApiService _api;
    public CreateModel(WarehouseApiService api) => _api = api;

    [BindProperty]
    public Receipt Receipt { get; set; } = new() { Date = DateTime.UtcNow.Date };

    [BindProperty]
    public List<ReceiptLine> Lines { get; set; } = new();
    
    public async Task OnGetAsync()
    {
        await Task.CompletedTask;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Console.WriteLine($"=== Form Data Debug ===");
        
        foreach (var key in Request.Form.Keys)
        {
            Console.WriteLine($"Form[{key}] = {Request.Form[key]}");
        }
        
        Console.WriteLine($"Receipt.Number: {Receipt.Number}");
        Console.WriteLine($"Receipt.Date: {Receipt.Date}");
        Console.WriteLine($"Lines is null: {Lines == null}");
        Console.WriteLine($"Lines count: {Lines?.Count ?? 0}");
        
        if (Lines != null)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                var line = Lines[i];
                Console.WriteLine($"Lines[{i}]: ResourceName='{line.ResourceName}', UnitName='{line.UnitName}', Quantity={line.Quantity}");
            }
        }

        var created = await _api.CreateReceiptAsync(Receipt);
        Receipt.Id = created.Id;
        Console.WriteLine($"Created Receipt with Id: {Receipt.Id}");

        var validLines = (Lines ?? new())
            .Where(l => !string.IsNullOrWhiteSpace(l.ResourceName) 
                     && !string.IsNullOrWhiteSpace(l.UnitName) 
                     && l.Quantity > 0)
            .ToList();

        Console.WriteLine($"Valid lines count: {validLines.Count}");
        foreach (var line in validLines)
        {
            Console.WriteLine($"Line: Resource='{line.ResourceName}', Unit='{line.UnitName}', Qty={line.Quantity}");
        }

        if (validLines.Any())
        {
            var allResources = await _api.GetResourcesAsync();
            var allUnits = await _api.GetUnitsAsync();

            var receiptResources = new List<ReceiptResource>();

            foreach (var line in validLines)
            {
                var resource = allResources.FirstOrDefault(r => r.Name.Equals(line.ResourceName, StringComparison.OrdinalIgnoreCase));
                if (resource == null)
                {
                    resource = await _api.CreateResourceAsync(new Resource 
                    { 
                        Name = line.ResourceName,
                        Status = EntityState.Active
                    });

                    if (resource == null || resource.Id == 0)
                    {
                        ModelState.AddModelError("", $"Не удалось создать ресурс '{line.ResourceName}'");
                        return Page();
                    }
                }

                var unit = allUnits.FirstOrDefault(u => u.Name.Equals(line.UnitName, StringComparison.OrdinalIgnoreCase));
                if (unit == null)
                {
                    unit = await _api.CreateUnitAsync(new Unit 
                    { 
                        Name = line.UnitName,
                        Status = EntityState.Active
                    });
                    
                    if (unit == null || unit.Id == 0)
                    {
                        ModelState.AddModelError("", $"Не удалось создать единицу измерения '{line.UnitName}'");
                        return Page();
                    }
                }

                receiptResources.Add(new ReceiptResource
                {
                    ReceiptId = Receipt.Id,
                    ResourceId = resource.Id,
                    UnitId = unit.Id,
                    Quantity = line.Quantity
                });
                
                Console.WriteLine($"Added line: ReceiptId={Receipt.Id}, ResourceId={resource.Id}, UnitId={unit.Id}, Quantity={line.Quantity}");
            }

            var success = await _api.ReplaceReceiptLinesAsync(Receipt.Id, receiptResources);
            
            if (!success)
            {
                await _api.DeleteReceiptAsync(Receipt.Id);
                
                ModelState.AddModelError("", "Не удалось добавить строки в документ. Проверьте правильность данных.");
                
                return Page();
            }
        }

        return RedirectToPage("Index");
    }
}