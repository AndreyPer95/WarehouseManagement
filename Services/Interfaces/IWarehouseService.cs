using WarehouseManagement.Models.Warehouse;

namespace WarehouseManagement.Services.Interfaces
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseBalance>> GetBalanceAsync();
        Task<WarehouseBalance?> GetBalanceByResourceAndUnitAsync(int resourceId, int unitId);
        Task<bool> IncreaseBalanceAsync(int resourceId, int unitId, decimal quantity);
        Task<bool> DecreaseBalanceAsync(int resourceId, int unitId, decimal quantity);
        Task<bool> CheckAvailabilityAsync(int resourceId, int unitId, decimal requiredQuantity);
    }
}
