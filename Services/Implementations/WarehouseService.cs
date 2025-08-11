using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models.Warehouse;
using WarehouseManagement.Services.Interfaces;

namespace WarehouseManagement.Services.Implementations
{
    public class WarehouseService : IWarehouseService
    {
        private readonly WarehouseContext _context;

        public WarehouseService(WarehouseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WarehouseBalance>> GetBalanceAsync()
        {
            return await _context.WarehouseBalances
                .Include(wb => wb.Resource)
                .Include(wb => wb.Unit)
                .ToListAsync();
        }

        public async Task<WarehouseBalance?> GetBalanceByResourceAndUnitAsync(int resourceId, int unitId)
        {
            return await _context.WarehouseBalances
                .FirstOrDefaultAsync(wb => wb.ResourceId == resourceId && wb.UnitId == unitId);
        }

        public async Task<bool> IncreaseBalanceAsync(int resourceId, int unitId, decimal quantity)
        {
            var balance = await GetBalanceByResourceAndUnitAsync(resourceId, unitId);            
            if (balance == null)
            {
                balance = new WarehouseBalance
                {
                    ResourceId = resourceId,
                    UnitId = unitId,
                    Quantity = quantity
                };
                _context.WarehouseBalances.Add(balance);
            }
            else
            {
                balance.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DecreaseBalanceAsync(int resourceId, int unitId, decimal quantity)
        {
            var balance = await GetBalanceByResourceAndUnitAsync(resourceId, unitId);
            
            if (balance == null || balance.Quantity < quantity)
            {
                return false; 
            }

            balance.Quantity -= quantity;
            
            if (balance.Quantity == 0)
            {
                _context.WarehouseBalances.Remove(balance);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckAvailabilityAsync(int resourceId, int unitId, decimal requiredQuantity)
        {
            var balance = await GetBalanceByResourceAndUnitAsync(resourceId, unitId);
            return balance != null && balance.Quantity >= requiredQuantity;
        }
    }
}
