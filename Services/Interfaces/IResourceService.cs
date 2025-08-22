using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Resources;
using WarehouseManagementAPI.Dto.Common;

namespace WarehouseManagementAPI.Services.Interfaces
{
    public interface IResourceService
    {
        Task<List<Resource>> GetAllAsync();
        Task<Resource?> GetByIdAsync(int id);

        Task<ServiceResult<Resource>> CreateAsync(Resource resource);
        Task<ServiceResult<Resource>> UpdateAsync(Resource resource);
        Task<ServiceResult> DeleteAsync(int id);
        Task<List<OptionDto>> GetFilterOptionsAsync();
    }
}
