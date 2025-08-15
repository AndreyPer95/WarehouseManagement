using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Units;

namespace WarehouseManagementAPI.Validators.Interfaces
{
    public interface IUnitValidator
    {
        Task<ServiceResult> ValidateForCreateAsync(Unit unit);
        Task<ServiceResult> ValidateForUpdateAsync(Unit unit);
        Task<ServiceResult> ValidateForDeleteAsync(int unitId);
    }
}
