using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Receipts;

namespace WarehouseManagementAPI.Validators.Interfaces
{
    public interface IReceiptResourceValidator
    {
        /// <summary>
        /// Валидация строки документа поступления.
        /// oldLine == null -> это новая строка (создание)
        /// Если пара (ResourceId, UnitId) изменилась — будет проверка доступности для снятия "старого" количества.
        /// </summary>
        Task<ServiceResult> ValidateAsync(ReceiptResource newLine, ReceiptResource? oldLine);
    }
}
