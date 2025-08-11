using WarehouseManagement.Models.Units;

namespace WarehouseManagement.Services.Interfaces
{
    public interface IUnitService
    {
        Task<IEnumerable<Unit>> GetAllAsync();
        Task<IEnumerable<Unit>> GetActiveAsync();
        Task<Unit?> GetByIdAsync(int id);
        Task<Unit> CreateAsync(Unit unit);
        Task<Unit> UpdateAsync(Unit unit);
        Task<bool> ArchiveAsync(int id);
        Task<bool> CanDeleteAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
    }
}
