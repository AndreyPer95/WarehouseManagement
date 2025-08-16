using System.ComponentModel.DataAnnotations;

namespace WarehouseClient.Models
{
    public class Receipt
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Номер документа обязателен")]
        [StringLength(50, ErrorMessage = "Номер не должен превышать 50 символов")]
        [Display(Name = "Номер")]
        public string Number { get; set; } = string.Empty;

        [Required(ErrorMessage = "Дата обязательна")]
        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        public List<ReceiptResource> ReceiptResources { get; set; } = new();
    }

    public class ReceiptResource
    {
        public int Id { get; set; }
        
        public int ReceiptId { get; set; }

        [Required(ErrorMessage = "Ресурс обязателен")]
        [Display(Name = "Ресурс")]
        public int ResourceId { get; set; }

        [Required(ErrorMessage = "Единица измерения обязательна")]
        [Display(Name = "Единица измерения")]
        public int UnitId { get; set; }

        [Required(ErrorMessage = "Количество обязательно")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        [Display(Name = "Количество")]
        public decimal Quantity { get; set; }

        // Навигационные свойства для отображения
        public Resource? Resource { get; set; }
        public Unit? Unit { get; set; }
    }
}