using WarehouseManagement.Models.Common;
using WarehouseManagement.Models.Receipts;
using WarehouseManagementAPI.Dto.Receipts;

namespace WarehouseManagement.Services.Interfaces
{
    /// <summary>
    /// Сервис для работы с документами поступления и их строками.
    /// Серверная фильтрация (период, номера, ресурсы, единицы),
    /// мультиселекты для номеров/ресурсов/единиц, 
    /// выдача документов вместе с наполнением (строками).
    /// Все изменения выполняются атомарно внутри транзакций.
    /// </summary>
    public interface IReceiptResourceService
    {
        /// <summary>Список всех номеров документов поступления (distinct, для мультиселекта).</summary>
        Task<List<string>> GetAllReceiptNumbersAsync();

        /// <summary>
        /// Получить список поступлений c наполнением (строками) по серверным фильтрам.
        /// Период (From/To), номера документов (multi), ресурсы (multi), единицы (multi).
        /// </summary>
        Task<List<ReceiptWithLinesDto>> GetReceiptsAsync(ReceiptFilter filter);

        /// <summary>Создать документ (пустой допускается). Проверяется уникальность номера.</summary>
        Task<ServiceResult<Receipt>> CreateReceiptAsync(Receipt receipt);

        /// <summary>Обновить шапку (номер/дата). Проверяется уникальность номера.</summary>
        Task<ServiceResult<Receipt>> UpdateReceiptAsync(Receipt receipt);

        /// <summary>Удалить документ. Перед удалением проверяется возможность списания всех строк со склада.</summary>
        Task<ServiceResult> DeleteReceiptAsync(int id);

        /// <summary>Добавить строку в документ. При успехе пополняет баланс склада.</summary>
        Task<ServiceResult<ReceiptResource>> AddResourceToReceiptAsync(ReceiptResource line);

        /// <summary>
        /// Полная замена строк документа (простая и читаемая операция «удалить и вставить заново»).
        /// Сначала считается и применяется дельта к складу, затем заменяются строки.
        /// </summary>
        Task<ServiceResult> ReplaceReceiptLinesAsync(int receiptId, List<ReceiptResource> newLines);

        /// <summary>Удалить одну строку документа. Перед удалением проверяется возможность списания её количества.</summary>
        Task<ServiceResult> DeleteReceiptLineAsync(int receiptResourceId);
    }
}
