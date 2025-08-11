using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models.Resources;
using WarehouseManagement.Services.Interfaces;

namespace WarehouseManagement.Services.Implementations
{
    public class ResourceService : IResourceService
    {
        private readonly WarehouseContext _context;

        public ResourceService(WarehouseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Resource>> GetAllAsync()
        {
            return await _context.Resources.ToListAsync();
        }

        public async Task<IEnumerable<Resource>> GetActiveAsync()
        {
            return await _context.Resources
                .Where(r => r.Status == ResourceStatus.Active)
                .ToListAsync();
        }

        public async Task<Resource?> GetByIdAsync(int id)
        {
            return await _context.Resources.FindAsync(id);
        }
        public async Task<Resource> CreateAsync(Resource resource)
        {
            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();
            return resource;
        }

        public async Task<Resource> UpdateAsync(Resource resource)
        {
            _context.Entry(resource).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return resource;
        }

        public async Task<bool> ArchiveAsync(int id)
        {
            var resource = await GetByIdAsync(id);
            if (resource == null) return false;

            resource.Status = ResourceStatus.Archived;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanDeleteAsync(int id)
        {
            return !await _context.ReceiptResources
                .AnyAsync(rr => rr.ResourceId == id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var resource = await GetByIdAsync(id);
            if (resource == null) return false;

            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            var query = _context.Resources.Where(r => r.Name == name);
            if (excludeId.HasValue)
            {
                query = query.Where(r => r.Id != excludeId.Value);
            }
            return !await query.AnyAsync();
        }
    }
}
