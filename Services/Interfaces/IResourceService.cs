using WarehouseManagement.Models.Resources;

namespace WarehouseManagement.Services.Interfaces
{
    public interface IResourceService
    {
        Task<IEnumerable<Resource>> GetAllAsync();
        Task<IEnumerable<Resource>> GetActiveAsync();
        Task<Resource?> GetByIdAsync(int id);
        Task<Resource> CreateAsync(Resource resource);
        Task<Resource> UpdateAsync(Resource resource);
        Task<bool> ArchiveAsync(int id);
        Task<bool> CanDeleteAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
    }
}
