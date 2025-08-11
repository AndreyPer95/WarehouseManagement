using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models.Units;
using WarehouseManagement.Services.Interfaces;

namespace WarehouseManagement.Services.Implementations
{
    public class UnitService : IUnitService
    {
        private readonly WarehouseContext _context;

        public UnitService(WarehouseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Unit>> GetAllAsync()
        {
            return await _context.Units.ToListAsync();
        }

        public async Task<IEnumerable<Unit>> GetActiveAsync()
        {
            return await _context.Units
                .Where(u => u.Status == UnitStatus.Active)
                .ToListAsync();
        }

        public async Task<Unit?> GetByIdAsync(int id)
        {
            return await _context.Units.FindAsync(id);
        }
        public async Task<Unit> CreateAsync(Unit unit)
        {
            _context.Units.Add(unit);
            await _context.SaveChangesAsync();
            return unit;
        }

        public async Task<Unit> UpdateAsync(Unit unit)
        {
            _context.Entry(unit).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return unit;
        }

        public async Task<bool> ArchiveAsync(int id)
        {
            var unit = await GetByIdAsync(id);
            if (unit == null) return false;

            unit.Status = UnitStatus.Archived;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanDeleteAsync(int id)
        {
            return !await _context.ReceiptResources
                .AnyAsync(rr => rr.UnitId == id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var unit = await GetByIdAsync(id);
            if (unit == null) return false;

            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            var query = _context.Units.Where(u => u.Name == name);
            if (excludeId.HasValue)
            {
                query = query.Where(u => u.Id != excludeId.Value);
            }
            return !await query.AnyAsync();
        }
    }
}
