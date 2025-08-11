using WarehouseManagement.Models.Receipts;

namespace WarehouseManagement.Services.Interfaces
{
    public interface IReceiptService
    {
        Task<IEnumerable<Receipt>> GetAllAsync();
        Task<Receipt?> GetByIdAsync(int id);
        Task<Receipt?> GetByIdWithDetailsAsync(int id);
        Task<Receipt> CreateAsync(Receipt receipt);
        Task<Receipt> UpdateAsync(Receipt receipt);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsNumberUniqueAsync(string number, int? excludeId = null);
    }
}
