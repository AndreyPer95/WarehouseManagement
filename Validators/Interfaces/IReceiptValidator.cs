using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Receipts;

namespace WarehouseManagementAPI.Validators.Interfaces
{
    public interface IReceiptValidator
    {
        Task<ServiceResult> ValidateForCreateAsync(Receipt receipt, List<ReceiptResource> newLines);
        Task<ServiceResult> ValidateForUpdateAsync(Receipt updatedReceipt, List<ReceiptResource> newLines);
        Task<ServiceResult> ValidateForDeleteAsync(int receiptId);
    }
}