using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models.Receipts;
using WarehouseManagement.Services.Interfaces;

namespace WarehouseManagement.Services.Implementations
{
    public class ReceiptService : IReceiptService
    {
        private readonly WarehouseContext _context;
        private readonly IWarehouseService _warehouseService;

        public ReceiptService(WarehouseContext context, IWarehouseService warehouseService)
        {
            _context = context;
            _warehouseService = warehouseService;
        }

        public async Task<IEnumerable<Receipt>> GetAllAsync()
        {
            return await _context.Receipts
                .Include(r => r.ReceiptResources)
                    .ThenInclude(rr => rr.Resource)
                .Include(r => r.ReceiptResources)
                    .ThenInclude(rr => rr.Unit)
                .ToListAsync();
        }

        public async Task<Receipt?> GetByIdAsync(int id)
        {
            return await _context.Receipts.FindAsync(id);
        }
        public async Task<Receipt?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Receipts
                .Include(r => r.ReceiptResources)
                    .ThenInclude(rr => rr.Resource)
                .Include(r => r.ReceiptResources)
                    .ThenInclude(rr => rr.Unit)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Receipt> CreateAsync(Receipt receipt)
        {
            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();

            foreach (var resource in receipt.ReceiptResources)
            {
                await _warehouseService.IncreaseBalanceAsync(
                    resource.ResourceId,
                    resource.UnitId,
                    resource.Quantity);
            }

            return receipt;
        }

        public async Task<Receipt> UpdateAsync(Receipt receipt)
        {
            _context.Entry(receipt).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return receipt;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var receipt = await GetByIdWithDetailsAsync(id);
            if (receipt == null) return false;

            foreach (var resource in receipt.ReceiptResources)
            {
                var canDecrease = await _warehouseService.CheckAvailabilityAsync(
                    resource.ResourceId,
                    resource.UnitId,
                    resource.Quantity);
                
                if (!canDecrease)
                {
                    return false; 
                }
            }

            foreach (var resource in receipt.ReceiptResources)
            {
                await _warehouseService.DecreaseBalanceAsync(
                    resource.ResourceId,
                    resource.UnitId,
                    resource.Quantity);
            }

            _context.Receipts.Remove(receipt);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsNumberUniqueAsync(string number, int? excludeId = null)
        {
            var query = _context.Receipts.Where(r => r.Number == number);
            if (excludeId.HasValue)
            {
                query = query.Where(r => r.Id != excludeId.Value);
            }
            return !await query.AnyAsync();
        }
    }
}
