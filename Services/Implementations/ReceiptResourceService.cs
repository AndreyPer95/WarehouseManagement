using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Receipts;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagementAPI.Validators.Interfaces;

namespace WarehouseManagement.Services.Implementations
{
    public class ReceiptResourceService : IReceiptResourceService
    {
        private readonly WarehouseContext _context;
        private readonly IWarehouseService _warehouseService;
        private readonly IReceiptValidator _receiptValidator;
        private readonly IReceiptResourceValidator _lineValidator;

        public ReceiptResourceService(
            WarehouseContext context,
            IWarehouseService warehouseService,
            IReceiptValidator receiptValidator,
            IReceiptResourceValidator lineValidator)
        {
            _context = context;
            _warehouseService = warehouseService;
            _receiptValidator = receiptValidator;
            _lineValidator = lineValidator;
        }

        #region Receipt (шапка)

        public Task<List<Receipt>> GetAllReceiptsAsync()
            => _context.Receipts.ToListAsync();

        public Task<Receipt?> GetReceiptByIdAsync(int id)
            => _context.Receipts.FindAsync(id).AsTask();

        public async Task<ServiceResult<Receipt>> CreateReceiptAsync(Receipt receipt)
        {
            // Документ может быть пустым — передаём пустой список строк
            var vr = await _receiptValidator.ValidateForCreateAsync(receipt, new List<ReceiptResource>());
            if (!vr.IsSuccess) return ServiceResult<Receipt>.Failure(vr.ErrorMessage ?? "Ошибка валидации");

            await using var tx = await _context.Database.BeginTransactionAsync();

            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();
            return ServiceResult<Receipt>.Success(receipt);
        }

        public async Task<ServiceResult<Receipt>> UpdateReceiptAsync(Receipt receipt)
        {
            var existing = await _context.Receipts.FindAsync(receipt.Id);
            if (existing is null) return ServiceResult<Receipt>.Failure("Документ не найден");

            // Передаём текущие строки (их изменение не происходит)
            var currentLines = await _context.ReceiptResources
                .Where(x => x.ReceiptId == receipt.Id)
                .ToListAsync();

            var vr = await _receiptValidator.ValidateForUpdateAsync(receipt, currentLines);
            if (!vr.IsSuccess) return ServiceResult<Receipt>.Failure(vr.ErrorMessage ?? "Ошибка валидации");

            existing.Number = receipt.Number;
            existing.Date = receipt.Date;

            await _context.SaveChangesAsync();
            return ServiceResult<Receipt>.Success(existing);
        }

        public async Task<ServiceResult> DeleteReceiptAsync(int id)
        {
            var receipt = await _context.Receipts.FindAsync(id);
            if (receipt is null) return ServiceResult.Failure("Документ не найден");

            // Валидатор проверит, что склад может «снять» все количества документа
            var vr = await _receiptValidator.ValidateForDeleteAsync(id);
            if (!vr.IsSuccess) return vr;

            var lines = await _context.ReceiptResources.Where(r => r.ReceiptId == id).ToListAsync();

            await using var tx = await _context.Database.BeginTransactionAsync();

            // 1) склад
            foreach (var l in lines)
                await _warehouseService.DecreaseBalanceAsync(l.ResourceId, l.UnitId, l.Quantity);

            // 2) БД
            _context.Receipts.Remove(receipt);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();
            return ServiceResult.Success();
        }

        #endregion

        #region ReceiptResources (строки)

        public Task<List<ReceiptResource>> GetReceiptResourcesAsync(int receiptId)
            => _context.ReceiptResources.Where(rr => rr.ReceiptId == receiptId).ToListAsync();

        public async Task<ServiceResult<ReceiptResource>> AddResourceToReceiptAsync(ReceiptResource newLine)
        {
            // Документ существует?
            var receipt = await _context.Receipts.FindAsync(newLine.ReceiptId);
            if (receipt is null) return ServiceResult<ReceiptResource>.Failure("Документ не найден");

            // Новая строка: oldLine == null (архивные — нельзя)
            var vr = await _lineValidator.ValidateAsync(newLine, null);
            if (!vr.IsSuccess) return ServiceResult<ReceiptResource>.Failure(vr.ErrorMessage ?? "Ошибка валидации");

            await using var tx = await _context.Database.BeginTransactionAsync();

            _context.ReceiptResources.Add(newLine);
            await _context.SaveChangesAsync();

            await _warehouseService.IncreaseBalanceAsync(newLine.ResourceId, newLine.UnitId, newLine.Quantity);

            await tx.CommitAsync();
            return ServiceResult<ReceiptResource>.Success(newLine);
        }

    /// <summary>
    /// Полная замена набора строк документа.
    /// Просто: валидируем строки, считаем дельту и применяем её, затем заменяем строки в БД.
    ///</summary>
    public async Task<ServiceResult> UpdateReceiptResourcesAsync(int receiptId, List<ReceiptResource> newLines)
        {
            var receipt = await _context.Receipts.FindAsync(receiptId);
            if (receipt is null) return ServiceResult.Failure("Документ не найден");

            var oldLines = await _context.ReceiptResources.Where(r => r.ReceiptId == receiptId).ToListAsync();

            // 1) Валидация каждой строки (архивность/существование/количество + корректность уменьшения/замены)
            foreach (var nl in newLines)
            {
                nl.ReceiptId = receiptId; // на всякий
                var old = oldLines.FirstOrDefault(x =>
                    x.Id == nl.Id || (x.ResourceId == nl.ResourceId && x.UnitId == nl.UnitId));

                var vrLine = await _lineValidator.ValidateAsync(nl, old);
                if (!vrLine.IsSuccess) return vrLine;
            }

            // 2) Считаем дельту по складу и предварительно проверяем, что списания возможны
            var (decreases, increases) = ComputeDiff(oldLines, newLines);

            foreach (var d in decreases)
            {
                var ok = await _warehouseService.CheckAvailabilityAsync(d.resId, d.unitId, d.qty);
                if (!ok)
                    return ServiceResult.Failure($"Недостаточно остатков для уменьшения: ResourceId={d.resId}, UnitId={d.unitId}, Qty={d.qty}");
            }

            // 3) Применяем изменения атомарно
            await using var tx = await _context.Database.BeginTransactionAsync();

            // 3.1 склад: сначала списания, потом пополнения
            foreach (var d in decreases)
                await _warehouseService.DecreaseBalanceAsync(d.resId, d.unitId, d.qty);

            foreach (var i in increases)
                await _warehouseService.IncreaseBalanceAsync(i.resId, i.unitId, i.qty);

            // 3.2 строки: простая замена
            _context.ReceiptResources.RemoveRange(oldLines);

            foreach (var nl in newLines)
                nl.ReceiptId = receiptId;

            await _context.ReceiptResources.AddRangeAsync(newLines);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteReceiptResourceAsync(int receiptResourceId)
        {
            var line = await _context.ReceiptResources.FirstOrDefaultAsync(x => x.Id == receiptResourceId);
            if (line is null) return ServiceResult.Failure("Строка документа не найдена");

            var ok = await _warehouseService.CheckAvailabilityAsync(line.ResourceId, line.UnitId, line.Quantity);
            if (!ok) return ServiceResult.Failure("Недостаточно остатков на складе для удаления строки");

            await using var tx = await _context.Database.BeginTransactionAsync();

            await _warehouseService.DecreaseBalanceAsync(line.ResourceId, line.UnitId, line.Quantity);
            _context.ReceiptResources.Remove(line);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();
            return ServiceResult.Success();
        }

        #endregion

        #region Helpers

        private static (List<(int resId, int unitId, decimal qty)> decreases,
                        List<(int resId, int unitId, decimal qty)> increases)
            ComputeDiff(IEnumerable<ReceiptResource> oldLines, IEnumerable<ReceiptResource> newLines)
        {
            var oldMap = oldLines
                .GroupBy(k => (k.ResourceId, k.UnitId))
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            var newMap = newLines
                .GroupBy(k => (k.ResourceId, k.UnitId))
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            var allKeys = oldMap.Keys.Union(newMap.Keys);

            var dec = new List<(int, int, decimal)>();
            var inc = new List<(int, int, decimal)>();

            foreach (var key in allKeys)
            {
                oldMap.TryGetValue(key, out var oldQty);
                newMap.TryGetValue(key, out var newQty);
                var delta = newQty - oldQty;

                if (delta < 0) dec.Add((key.ResourceId, key.UnitId, -delta));
                if (delta > 0) inc.Add((key.ResourceId, key.UnitId, delta));
            }

            return (dec, inc);
        }

        #endregion
    }
}