using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Resources;
using WarehouseManagementAPI.Dto.Common;
using WarehouseManagementAPI.Validators.Interfaces;
using WarehouseManagementAPI.Services.Interfaces;

public class ResourceService : IResourceService
{
    private readonly WarehouseContext _context;
    private readonly IResourceValidator _validator;

    public ResourceService(WarehouseContext context, IResourceValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public Task<List<Resource>> GetAllAsync()
        => _context.Resources.OrderBy(r => r.Name).ToListAsync();

    public Task<Resource?> GetByIdAsync(int id)
        => _context.Resources.FindAsync(id).AsTask();

    public async Task<ServiceResult<Resource>> CreateAsync(Resource resource)
    {
        var vr = await _validator.ValidateForCreateAsync(resource);
        if (!vr.IsSuccess) return ServiceResult<Resource>.Failure(vr.ErrorMessage ?? "������ ���������");

        _context.Resources.Add(resource);
        await _context.SaveChangesAsync();
        return ServiceResult<Resource>.Success(resource);
    }

    public async Task<ServiceResult<Resource>> UpdateAsync(Resource resource)
    {
        var vr = await _validator.ValidateForUpdateAsync(resource);
        if (!vr.IsSuccess) return ServiceResult<Resource>.Failure(vr.ErrorMessage ?? "������ ���������");

        var existing = await _context.Resources.FindAsync(resource.Id);
        if (existing is null) return ServiceResult<Resource>.Failure("������ �� ������");

        existing.Name = resource.Name;
        existing.Status = resource.Status;
        await _context.SaveChangesAsync();

        return ServiceResult<Resource>.Success(existing);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var vr = await _validator.ValidateForDeleteAsync(id);
        if (!vr.IsSuccess) return vr;

        var entity = await _context.Resources.FindAsync(id);
        if (entity is null) return ServiceResult.Failure("������ �� ������");

        _context.Resources.Remove(entity);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    // ��� �������� (�� ������� �� �������)
    public async Task<List<OptionDto>> GetFilterOptionsAsync()
        => await _context.Resources
            .OrderBy(r => r.Name)
            .Select(r => new OptionDto { Id = r.Id, Name = r.Name })
            .ToListAsync();
}
