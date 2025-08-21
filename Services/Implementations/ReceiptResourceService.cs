using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Receipts;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagementAPI.Dto.Receipts;
using WarehouseManagementAPI.Validators.Interfaces;

public class ReceiptResourceService : IReceiptResourceService
{
    private readonly WarehouseContext _context;
    private readonly IWarehouseService _warehouse;
    private readonly IReceiptValidator _receiptValidator;
    private readonly IReceiptResourceValidator _lineValidator;

    public ReceiptResourceService(
        WarehouseContext context,
        IWarehouseService warehouse,
        IReceiptValidator receiptValidator,
        IReceiptResourceValidator lineValidator)
    {
        _context = context;
        _warehouse = warehouse;
        _receiptValidator = receiptValidator;
        _lineValidator = lineValidator;
    }

    public async Task<List<string>> GetAllReceiptNumbersAsync()
        => await _context.Receipts
            .Select(r => r.Number)
            .Distinct()
            .OrderBy(n => n)
            .ToListAsync();

    public async Task<List<ReceiptWithLinesDto>> GetReceiptsAsync(ReceiptFilter filter)
    {
        var rq = _context.Receipts.AsQueryable();

        if (filter.From.HasValue)
            rq = rq.Where(r => r.Date >= filter.From.Value.Date);

        if (filter.To.HasValue)
        {
            var to = filter.To.Value.Date.AddDays(1); 
            rq = rq.Where(r => r.Date < to);
        }

        if (filter.Numbers is { Count: > 0 })
            rq = rq.Where(r => filter.Numbers.Contains(r.Number));

        if (filter.ResourceIds is { Count: > 0 } || filter.UnitIds is { Count: > 0 })
        {
            var lr = _context.ReceiptResources.AsQueryable();
            if (filter.ResourceIds is { Count: > 0 })
                lr = lr.Where(x => filter.ResourceIds.Contains(x.ResourceId));
            if (filter.UnitIds is { Count: > 0 })
                lr = lr.Where(x => filter.UnitIds.Contains(x.UnitId));

            rq = rq.Where(r => lr.Any(x => x.ReceiptId == r.Id));
        }

        var receipts = await rq
            .OrderByDescending(r => r.Date)
            .ThenBy(r => r.Number)
            .ToListAsync();

        if (receipts.Count == 0)
            return new List<ReceiptWithLinesDto>();

        var ids = receipts.Select(r => r.Id).ToList();
        var lines = await _context.ReceiptResources
            .Where(x => ids.Contains(x.ReceiptId))
            .ToListAsync();

        var resIds = lines.Select(l => l.ResourceId).Distinct().ToList();
        var unitIds = lines.Select(l => l.UnitId).Distinct().ToList();

        var resMap = await _context.Resources
            .Where(r => resIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, r => r.Name);

        var unitMap = await _context.Units
            .Where(u => unitIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Name);

        var linesByReceipt = lines.GroupBy(l => l.ReceiptId)
            .ToDictionary(g => g.Key, g => g.Select(l => new ReceiptLineDto
            {
                ResourceId = l.ResourceId,
                ResourceName = resMap.TryGetValue(l.ResourceId, out var rn) ? rn : l.ResourceId.ToString(),
                UnitId = l.UnitId,
                UnitName = unitMap.TryGetValue(l.UnitId, out var un) ? un : l.UnitId.ToString(),
                Quantity = l.Quantity
            }).ToList());

        return receipts.Select(r => new ReceiptWithLinesDto
        {
            Id = r.Id,
            Number = r.Number,
            Date = r.Date,
            Lines = linesByReceipt.TryGetValue(r.Id, out var ls) ? ls : new List<ReceiptLineDto>()
        }).ToList();
    }

    public async Task<ServiceResult<Receipt>> CreateReceiptAsync(Receipt receipt)
    {
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

        var currentLines = await _context.ReceiptResources.Where(x => x.ReceiptId == receipt.Id).ToListAsync();
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

        var vr = await _receiptValidator.ValidateForDeleteAsync(id);
        if (!vr.IsSuccess) return vr;

        var lines = await _context.ReceiptResources.Where(r => r.ReceiptId == id).ToListAsync();

        await using var tx = await _context.Database.BeginTransactionAsync();

        foreach (var l in lines)
            await _warehouse.DecreaseBalanceAsync(l.ResourceId, l.UnitId, l.Quantity);

        _context.Receipts.Remove(receipt);
        await _context.SaveChangesAsync();

        await tx.CommitAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult<ReceiptResource>> AddResourceToReceiptAsync(ReceiptResource line)
    {
        var receipt = await _context.Receipts.FindAsync(line.ReceiptId);
        if (receipt is null) return ServiceResult<ReceiptResource>.Failure("Документ не найден");

        var vr = await _lineValidator.ValidateAsync(line, null);
        if (!vr.IsSuccess) return ServiceResult<ReceiptResource>.Failure(vr.ErrorMessage ?? "Ошибка валидации");

        await using var tx = await _context.Database.BeginTransactionAsync();

        _context.ReceiptResources.Add(line);
        await _context.SaveChangesAsync();

        await _warehouse.IncreaseBalanceAsync(line.ResourceId, line.UnitId, line.Quantity);

        await tx.CommitAsync();
        return ServiceResult<ReceiptResource>.Success(line);
    }

    public async Task<ServiceResult> ReplaceReceiptLinesAsync(int receiptId, List<ReceiptResource> newLines)
    {
        var receipt = await _context.Receipts.FindAsync(receiptId);
        if (receipt is null) return ServiceResult.Failure("Документ не найден");

        var oldLines = await _context.ReceiptResources.Where(r => r.ReceiptId == receiptId).ToListAsync();

        foreach (var nl in newLines)
        {
            nl.ReceiptId = receiptId;
            var vr = await _lineValidator.ValidateAsync(nl, oldLine: null);
            if (!vr.IsSuccess) return vr;
        }

        var oldMap = oldLines.GroupBy(k => (k.ResourceId, k.UnitId))
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));
        var newMap = newLines.GroupBy(k => (k.ResourceId, k.UnitId))
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        var allKeys = oldMap.Keys.Union(newMap.Keys);
        var dec = new List<(int res, int unit, decimal qty)>();
        var inc = new List<(int res, int unit, decimal qty)>();
        foreach (var key in allKeys)
        {
            oldMap.TryGetValue(key, out var oq);
            newMap.TryGetValue(key, out var nq);
            var delta = nq - oq;
            if (delta < 0) dec.Add((key.ResourceId, key.UnitId, -delta));
            if (delta > 0) inc.Add((key.ResourceId, key.UnitId, delta));
        }

        foreach (var d in dec)
        {
            var ok = await _warehouse.CheckAvailabilityAsync(d.res, d.unit, d.qty);
            if (!ok) return ServiceResult.Failure($"Недостаточно остатков для уменьшения: res={d.res}, unit={d.unit}, qty={d.qty}");
        }

        await using var tx = await _context.Database.BeginTransactionAsync();

        foreach (var d in dec) await _warehouse.DecreaseBalanceAsync(d.res, d.unit, d.qty);
        foreach (var i in inc) await _warehouse.IncreaseBalanceAsync(i.res, i.unit, i.qty);

        _context.ReceiptResources.RemoveRange(oldLines);
        foreach (var nl in newLines) nl.ReceiptId = receiptId;
        await _context.ReceiptResources.AddRangeAsync(newLines);
        await _context.SaveChangesAsync();

        await tx.CommitAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteReceiptLineAsync(int receiptResourceId)
    {
        var line = await _context.ReceiptResources.FirstOrDefaultAsync(x => x.Id == receiptResourceId);
        if (line is null) return ServiceResult.Failure("Строка документа не найдена");

        var ok = await _warehouse.CheckAvailabilityAsync(line.ResourceId, line.UnitId, line.Quantity);
        if (!ok) return ServiceResult.Failure("Недостаточно остатков на складе");

        await using var tx = await _context.Database.BeginTransactionAsync();

        await _warehouse.DecreaseBalanceAsync(line.ResourceId, line.UnitId, line.Quantity);
        _context.ReceiptResources.Remove(line);
        await _context.SaveChangesAsync();

        await tx.CommitAsync();
        return ServiceResult.Success();
    }
}
