using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Units;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagement.Services.Interfaces.WarehouseManagementAPI.Services.Interfaces;
using WarehouseManagementAPI.Dto.Common;
using WarehouseManagementAPI.Validators.Interfaces;

public class UnitService:IUnitService
{
    private readonly WarehouseContext _context;
    private readonly IUnitValidator _validator;

    public UnitService(WarehouseContext context, IUnitValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public Task<List<Unit>> GetAllAsync()
        => _context.Units.OrderBy(u => u.Name).ToListAsync();

    public Task<Unit?> GetByIdAsync(int id)
        => _context.Units.FindAsync(id).AsTask();

    public async Task<ServiceResult<Unit>> CreateAsync(Unit unit)
    {
        var vr = await _validator.ValidateForCreateAsync(unit);
        if (!vr.IsSuccess) return ServiceResult<Unit>.Failure(vr.ErrorMessage ?? "Ошибка валидации");

        _context.Units.Add(unit);
        await _context.SaveChangesAsync();
        return ServiceResult<Unit>.Success(unit);
    }

    public async Task<ServiceResult<Unit>> UpdateAsync(Unit unit)
    {
        var vr = await _validator.ValidateForUpdateAsync(unit);
        if (!vr.IsSuccess) return ServiceResult<Unit>.Failure(vr.ErrorMessage ?? "Ошибка валидации");

        var existing = await _context.Units.FindAsync(unit.Id);
        if (existing is null) return ServiceResult<Unit>.Failure("Единица измерения не найдена");

        existing.Name = unit.Name;
        await _context.SaveChangesAsync();

        return ServiceResult<Unit>.Success(existing);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var vr = await _validator.ValidateForDeleteAsync(id);
        if (!vr.IsSuccess) return vr;

        var entity = await _context.Units.FindAsync(id);
        if (entity is null) return ServiceResult.Failure("Единица измерения не найдена");

        _context.Units.Remove(entity);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<List<OptionDto>> GetFilterOptionsAsync()
        => await _context.Units
            .OrderBy(u => u.Name)
            .Select(u => new OptionDto { Id = u.Id, Name = u.Name })
            .ToListAsync();
}
