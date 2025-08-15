using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Receipts;

namespace WarehouseManagement.Services.Interfaces
{
    /// <summary>
    /// Сервис работы с документами поступления и их строками.
    /// Все операции атомарны (транзакции) и валидируются валидаторами.
    /// Баланс склада корректируется согласно дельтам по (ResourceId, UnitId).
    /// </summary>
    public interface IReceiptResourceService
    {
        // -------- Receipt (шапка) --------

        /// <summary>Получить все документы поступления.</summary>
        Task<List<Receipt>> GetAllReceiptsAsync();

        /// <summary>Получить документ по идентификатору.</summary>
        Task<Receipt?> GetReceiptByIdAsync(int id);

        /// <summary>
        /// Создать документ. Допускается пустой.
        /// Валидируется уникальность номера.
        /// </summary>
        Task<ServiceResult<Receipt>> CreateReceiptAsync(Receipt receipt);

        /// <summary>
        /// Обновить шапку документа (номер/дата).
        /// Проверяется уникальность номера.
        /// </summary>
        Task<ServiceResult<Receipt>> UpdateReceiptAsync(Receipt receipt);

        /// <summary>
        /// Удалить документ. Перед удалением проверяется возможность списания всех строк со склада.
        /// </summary>
        Task<ServiceResult> DeleteReceiptAsync(int id);


        // -------- ReceiptResources (строки) --------

        /// <summary>Получить строки документа.</summary>
        Task<List<ReceiptResource>> GetReceiptResourcesAsync(int receiptId);

        /// <summary>
        /// Добавить строку в документ. При успехе увеличивает баланс склада.
        /// Архивные ресурсы/единицы запрещены (если строка новая).
        /// </summary>
        Task<ServiceResult<ReceiptResource>> AddResourceToReceiptAsync(ReceiptResource newLine);

        /// <summary>
        /// Полная замена набора строк документа.
        /// Считается дельта по складу: сначала списания, затем пополнения.
        /// </summary>
        Task<ServiceResult> UpdateReceiptResourcesAsync(int receiptId, List<ReceiptResource> newLines);

        /// <summary>
        /// Удалить строку документа. Перед удалением проверяется возможность списания её количества.
        /// </summary>
        Task<ServiceResult> DeleteReceiptResourceAsync(int receiptResourceId);
    }
}
