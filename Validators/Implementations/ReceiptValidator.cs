using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Receipts;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagementAPI.Validators.Interfaces;

namespace WarehouseManagementAPI.Validators.Implementations
{
    public class ReceiptValidator : IReceiptValidator
    {
        private readonly WarehouseContext _context;
        private readonly IWarehouseService _warehouseService;
        private readonly IReceiptResourceValidator _lineValidator;

        public ReceiptValidator(
            WarehouseContext context,
            IWarehouseService warehouseService,
            IReceiptResourceValidator lineValidator)
        {
            _context = context;
            _warehouseService = warehouseService;
            _lineValidator = lineValidator;
        }

        public async Task<ServiceResult> ValidateForCreateAsync(Receipt receipt, List<ReceiptResource> newLines)
        {
            var errors = new List<string>();

            if (!await IsReceiptNumberUniqueAsync(receipt.Number))
                errors.Add($"Документ с номером {receipt.Number} уже существует");

            foreach (var line in newLines)
            {
                var lineResult = await _lineValidator.ValidateAsync(line, oldLine: null);
                if (!lineResult.IsSuccess)
                    errors.AddRange(lineResult.Errors);
            }

            return errors.Any() ? ServiceResult.Failure(errors) : ServiceResult.Success();
        }

        public async Task<ServiceResult> ValidateForUpdateAsync(Receipt updatedReceipt, List<ReceiptResource> newLines)
        {
            var errors = new List<string>();

            var existing = await _context.Receipts.FindAsync(updatedReceipt.Id);
            if (existing is null)
            {
                errors.Add($"Документ поступления с ID {updatedReceipt.Id} не найден");
                return ServiceResult.Failure(errors);
            }

            if (!await IsReceiptNumberUniqueAsync(updatedReceipt.Number, excludeId: updatedReceipt.Id))
                errors.Add($"Документ с номером {updatedReceipt.Number} уже существует");

            var oldLines = await _context.ReceiptResources
                .Where(rr => rr.ReceiptId == updatedReceipt.Id)
                .ToListAsync();

            foreach (var newLine in newLines)
            {
                var oldLine = oldLines.FirstOrDefault(x =>
                    x.Id == newLine.Id ||
                    x.ResourceId == newLine.ResourceId && x.UnitId == newLine.UnitId);

                var lineResult = await _lineValidator.ValidateAsync(newLine, oldLine);
                if (!lineResult.IsSuccess)
                    errors.AddRange(lineResult.Errors);
            }

            var removed = oldLines.Where(ol =>
                !newLines.Any(nl => nl.Id == ol.Id ||
                                    nl.ResourceId == ol.ResourceId && nl.UnitId == ol.UnitId));

            if (removed.Any())
            {
                var resIds = removed.Select(r => r.ResourceId).Distinct().ToList();
                var unitIds = removed.Select(r => r.UnitId).Distinct().ToList();

                var resMap = await _context.Resources
                    .Where(r => resIds.Contains(r.Id))
                    .ToDictionaryAsync(r => r.Id, r => r.Name);

                var unitMap = await _context.Units
                    .Where(u => unitIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u.Name);

                foreach (var rm in removed)
                {
                    var canReduce = await _warehouseService.CheckAvailabilityAsync(rm.ResourceId, rm.UnitId, rm.Quantity);
                    if (!canReduce)
                    {
                        resMap.TryGetValue(rm.ResourceId, out var resName);
                        unitMap.TryGetValue(rm.UnitId, out var unitName);
                        errors.Add($"Недостаточно ресурса '{resName ?? rm.ResourceId.ToString()}' ({unitName ?? rm.UnitId.ToString()}) на складе для удаления строки (нужно снять {rm.Quantity})");
                    }
                }
            }

            return errors.Any() ? ServiceResult.Failure(errors) : ServiceResult.Success();
        }

        public async Task<ServiceResult> ValidateForDeleteAsync(int receiptId)
        {
            var errors = new List<string>();

            var existing = await _context.Receipts.FindAsync(receiptId);
            if (existing is null)
            {
                errors.Add($"Документ с ID {receiptId} не найден");
                return ServiceResult.Failure(errors);
            }

            var lines = await _context.ReceiptResources
                .Where(rr => rr.ReceiptId == receiptId)
                .ToListAsync();

            if (lines.Any())
            {
                var resIds = lines.Select(r => r.ResourceId).Distinct().ToList();
                var unitIds = lines.Select(r => r.UnitId).Distinct().ToList();

                var resMap = await _context.Resources
                    .Where(r => resIds.Contains(r.Id))
                    .ToDictionaryAsync(r => r.Id, r => r.Name);

                var unitMap = await _context.Units
                    .Where(u => unitIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u.Name);

                foreach (var line in lines)
                {
                    var canReduce = await _warehouseService.CheckAvailabilityAsync(line.ResourceId, line.UnitId, line.Quantity);
                    if (!canReduce)
                    {
                        resMap.TryGetValue(line.ResourceId, out var resName);
                        unitMap.TryGetValue(line.UnitId, out var unitName);
                        errors.Add($"Недостаточно ресурса '{resName ?? line.ResourceId.ToString()}' ({unitName ?? line.UnitId.ToString()}) на складе для удаления документа (нужно снять {line.Quantity})");
                    }
                }
            }

            return errors.Any() ? ServiceResult.Failure(errors) : ServiceResult.Success();
        }

        private async Task<bool> IsReceiptNumberUniqueAsync(string number, int? excludeId = null)
        {
            var normalized = (number ?? string.Empty).Trim().ToUpper();
            var query = _context.Receipts.AsNoTracking()
                                .Where(r => r.Number != null && r.Number.Trim().ToUpper() == normalized);
            if (excludeId.HasValue) query = query.Where(r => r.Id != excludeId.Value);
            return !await query.AnyAsync();
        }
    }
}
