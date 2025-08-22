using WarehouseManagement.Models.Warehouse;
using WarehouseManagementAPI.Dto.Warehouse;

namespace WarehouseManagement.Services.Interfaces
{
    public interface IWarehouseService
    {
        Task<bool> CheckAvailabilityAsync(int resourceId, int unitId, decimal qty);
        Task IncreaseBalanceAsync(int resourceId, int unitId, decimal qty);
        Task DecreaseBalanceAsync(int resourceId, int unitId, decimal qty);
        Task<List<WarehouseBalanceRowDto>> GetBalanceAsync(List<int>? resourceIds, List<int>? unitIds);
    }
}
