using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Units;
using WarehouseManagementAPI.Dto.Common;

namespace WarehouseManagement.Services.Interfaces
{
    namespace WarehouseManagementAPI.Services.Interfaces
    {
        public interface IUnitService
        {
            Task<List<Unit>> GetAllAsync();
            Task<Unit?> GetByIdAsync(int id);

            Task<ServiceResult<Unit>> CreateAsync(Unit unit);
            Task<ServiceResult<Unit>> UpdateAsync(Unit unit);
            Task<ServiceResult> DeleteAsync(int id);

            /// <summary>Опции для фильтров (id+name), не зависят от периода.</summary>
            Task<List<OptionDto>> GetFilterOptionsAsync();
        }
    }
}