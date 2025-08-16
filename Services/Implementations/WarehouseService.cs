using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models.Warehouse;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagementAPI.Dto.Warehouse;

namespace WarehouseManagement.Services.Implementations
{
    public class WarehouseService : IWarehouseService
    {
        private readonly WarehouseContext _context;

        public WarehouseService(WarehouseContext context) => _context = context;

        public async Task<bool> CheckAvailabilityAsync(int resourceId, int unitId, decimal qty)
        {
            if (qty <= 0) return true;
            var row = await _context.WarehouseBalances
                .FirstOrDefaultAsync(b => b.ResourceId == resourceId && b.UnitId == unitId);
            var current = row?.Quantity ?? 0m;
            return current >= qty;
        }

        public async Task IncreaseBalanceAsync(int resourceId, int unitId, decimal qty)
        {
            if (qty <= 0) return;

            var row = await _context.WarehouseBalances
                .FirstOrDefaultAsync(b => b.ResourceId == resourceId && b.UnitId == unitId);

            if (row is null)
            {
                row = new WarehouseBalance { ResourceId = resourceId, UnitId = unitId, Quantity = qty };
                _context.WarehouseBalances.Add(row);
            }
            else
            {
                row.Quantity += qty;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DecreaseBalanceAsync(int resourceId, int unitId, decimal qty)
        {
            if (qty <= 0) return;

            var row = await _context.WarehouseBalances
                .FirstOrDefaultAsync(b => b.ResourceId == resourceId && b.UnitId == unitId);

            if (row is null || row.Quantity < qty)
                throw new InvalidOperationException("Недостаточно остатков для списания");

            row.Quantity -= qty;
            await _context.SaveChangesAsync();
        }

        public async Task<List<WarehouseBalanceRowDto>> GetBalanceAsync(List<int>? resourceIds, List<int>? unitIds)
        {
            var q = _context.WarehouseBalances.AsQueryable();

            if (resourceIds is { Count: > 0 })
                q = q.Where(b => resourceIds.Contains(b.ResourceId));

            if (unitIds is { Count: > 0 })
                q = q.Where(b => unitIds.Contains(b.UnitId));

            var rows = await q.ToListAsync();

            // Подтянем имена одним батчем
            var resIds = rows.Select(r => r.ResourceId).Distinct().ToList();
            var unitIdsAll = rows.Select(r => r.UnitId).Distinct().ToList();

            var resMap = await _context.Resources
                .Where(r => resIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id, r => r.Name);

            var unitMap = await _context.Units
                .Where(u => unitIdsAll.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Name);

            return rows.Select(r => new WarehouseBalanceRowDto
            {
                ResourceId = r.ResourceId,
                ResourceName = resMap.TryGetValue(r.ResourceId, out var rn) ? rn : r.ResourceId.ToString(),
                UnitId = r.UnitId,
                UnitName = unitMap.TryGetValue(r.UnitId, out var un) ? un : r.UnitId.ToString(),
                Quantity = r.Quantity
            }).ToList();
        }
    }

}
