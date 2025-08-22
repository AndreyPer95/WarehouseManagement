using WarehouseManagement.Data;
using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Receipts;
using WarehouseManagement.Models.Resources;
using WarehouseManagement.Models.Units;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagementAPI.Validators.Interfaces;

namespace WarehouseManagementAPI.Validators.Implementations
{
    public class ReceiptResourceValidator : IReceiptResourceValidator
    {
        private readonly WarehouseContext _context;
        private readonly IWarehouseService _warehouseService;

        public ReceiptResourceValidator(WarehouseContext context, IWarehouseService warehouseService)
        {
            _context = context;
            _warehouseService = warehouseService;
        }

        public async Task<ServiceResult> ValidateAsync(ReceiptResource newLine, ReceiptResource? oldLine)
        {
            var errors = new List<string>();

            var allowArchived = oldLine != null 
                && oldLine.ResourceId == newLine.ResourceId
                && oldLine.UnitId == newLine.UnitId;

            var resource = await _context.Resources.FindAsync(newLine.ResourceId);
            if (resource is null)
                errors.Add($"Ресурс с ID {newLine.ResourceId} не найден");
            else if (!allowArchived && resource.Status != ResourceStatus.Active)
                errors.Add($"Ресурс '{resource.Name}' архивирован и недоступен для выбора");

            var unit = await _context.Units.FindAsync(newLine.UnitId);
            if (unit is null)
                errors.Add($"Единица измерения с ID {newLine.UnitId} не найдена");
            else if (!allowArchived && unit.Status != UnitStatus.Active)
                errors.Add($"Единица измерения '{unit.Name}' архивирована и недоступна для выбора");

            if (newLine.Quantity <= 0)
                errors.Add("Количество должно быть больше нуля");

            if (errors.Any())
                return ServiceResult.Failure(errors);

            if (oldLine != null)
            {
                if (oldLine.ResourceId == newLine.ResourceId && oldLine.UnitId == newLine.UnitId)
                {
                    var delta = oldLine.Quantity - newLine.Quantity; 
                    if (delta > 0)
                    {
                        var canReduce = await _warehouseService.CheckAvailabilityAsync(
                            newLine.ResourceId, newLine.UnitId, delta);

                        if (!canReduce)
                            errors.Add($"Недостаточно ресурса '{resource?.Name}' ({unit?.Name}) на складе для уменьшения количества на {delta}");
                    }
                }
                else
                {
                    var oldRes = await _context.Resources.FindAsync(oldLine.ResourceId);
                    var oldUnit = await _context.Units.FindAsync(oldLine.UnitId);

                    var canReduceOld = await _warehouseService.CheckAvailabilityAsync(
                        oldLine.ResourceId, oldLine.UnitId, oldLine.Quantity);

                    if (!canReduceOld)
                        errors.Add($"Недостаточно ресурса '{oldRes?.Name}' ({oldUnit?.Name}) на складе для замены строки (нужно снять {oldLine.Quantity})");
                }
            }

            return errors.Any() ? ServiceResult.Failure(errors) : ServiceResult.Success();
        }
    }
}
