using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Resources;

namespace WarehouseManagementAPI.Validators.Interfaces
{
    public interface IResourceValidator
    {
        Task<ServiceResult> ValidateForCreateAsync(Resource resource);
        Task<ServiceResult> ValidateForUpdateAsync(Resource resource);
        Task<ServiceResult> ValidateForDeleteAsync(int resourceId);
    }
}
