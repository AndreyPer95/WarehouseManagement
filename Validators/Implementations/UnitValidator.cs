using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Units;
using WarehouseManagementAPI.Validators.Interfaces;

namespace WarehouseManagementAPI.Validators.Implementations
{
    public class UnitValidator : IUnitValidator
    {
        private readonly WarehouseContext _context;

        public UnitValidator(WarehouseContext context) => _context = context;

        public async Task<ServiceResult> ValidateForCreateAsync(Unit unit)
        {
            var errors = new List<string>();

            if (await NameExistsAsync(unit.Name))
                errors.Add($"Единица измерения с именем '{unit.Name}' уже существует");

            return errors.Any() ? ServiceResult.Failure(errors) : ServiceResult.Success();
        }

        public async Task<ServiceResult> ValidateForUpdateAsync(Unit unit)
        {
            var errors = new List<string>();

            var exists = await _context.Units.FindAsync(unit.Id);
            if (exists is null)
            {
                errors.Add($"Единица измерения с ID {unit.Id} не найдена");
                return ServiceResult.Failure(errors);
            }

            if (await NameExistsAsync(unit.Name, excludeId: unit.Id))
                errors.Add($"Единица измерения с именем '{unit.Name}' уже существует");

            return errors.Any() ? ServiceResult.Failure(errors) : ServiceResult.Success();
        }

        public async Task<ServiceResult> ValidateForDeleteAsync(int unitId)
        {
            var errors = new List<string>();

            var unit = await _context.Units.FindAsync(unitId);
            if (unit is null)
            {
                errors.Add($"Единица измерения с ID {unitId} не найдена");
                return ServiceResult.Failure(errors);
            }

            var usedInReceipts = await _context.ReceiptResources.AnyAsync(r => r.UnitId == unitId);
            if (usedInReceipts)
                errors.Add("Единица измерения используется и не может быть удалена. Переведите её в архив.");

            return errors.Any() ? ServiceResult.Failure(errors) : ServiceResult.Success();
        }

        private Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            var normalized = Normalize(name);
            var query = _context.Units.Where(u => Normalize(u.Name) == normalized);
            if (excludeId.HasValue)
                query = query.Where(u => u.Id != excludeId.Value);
            return query.AnyAsync();
        }

        private static string Normalize(string s) => (s ?? string.Empty).Trim().ToUpperInvariant();
    }
}
