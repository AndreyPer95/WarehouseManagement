using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Resources;
using WarehouseManagementAPI.Validators.Interfaces;

namespace WarehouseManagementAPI.Validators.Implementations
{
    public class ResourceValidator : IResourceValidator
    {
        private readonly WarehouseContext _context;

        public ResourceValidator(WarehouseContext context) => _context = context;

        public async Task<ServiceResult> ValidateForCreateAsync(Resource resource)
        {
            var errors = new List<string>();
            var name = resource.Name;
            if (string.IsNullOrWhiteSpace(name))
                errors.Add("Наименование обязательно");
            
            if (await NameExistsAsync(name))
                errors.Add($"Ресурс с именем '{resource.Name}' уже существует");

            return errors.Any() ? ServiceResult.Failure(errors) : ServiceResult.Success();
        }

        public async Task<ServiceResult> ValidateForUpdateAsync(Resource resource)
        {
            var errors = new List<string>();

            var exists = await _context.Resources.FindAsync(resource.Id);
            if (exists is null)
            {
                errors.Add($"Ресурс с ID {resource.Id} не найден");
                return ServiceResult.Failure(errors);
            }

            if (string.IsNullOrWhiteSpace(resource.Name))
                errors.Add("Наименование обязательно");

            if (await NameExistsAsync(resource.Name, excludeId: resource.Id))
                errors.Add($"Ресурс с именем '{resource.Name}' уже существует");

            return errors.Any() ? ServiceResult.Failure(errors) : ServiceResult.Success();
        }

        public async Task<ServiceResult> ValidateForDeleteAsync(int resourceId)
        {
            var errors = new List<string>();

            var resource = await _context.Resources.FindAsync(resourceId);
            if (resource is null)
            {
                errors.Add($"Ресурс с ID {resourceId} не найден");
                return ServiceResult.Failure(errors);
            }

            var usedInReceipts = await _context.ReceiptResources.AnyAsync(r => r.ResourceId == resourceId);
            if (usedInReceipts)
                errors.Add("Ресурс используется и не может быть удалён. Переведите его в архив.");

            return errors.Any() ? ServiceResult.Failure(errors) : ServiceResult.Success();
        }

        private Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            var normalized = (name ?? string.Empty).Trim().ToUpper();
            var query = _context.Resources.AsNoTracking()
                                .Where(r => r.Name != null && r.Name.Trim().ToUpper() == normalized); 
            if (excludeId.HasValue)
                query = query.Where(r => r.Id != excludeId.Value);
            return query.AnyAsync();
        }
    }
}
