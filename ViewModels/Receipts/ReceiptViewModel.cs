using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.ViewModels.Receipts
{
    public class ReceiptViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Номер документа обязателен")]
        [StringLength(50, ErrorMessage = "Номер не может превышать 50 символов")]
        [Display(Name = "Номер документа")]
        public string Number { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Дата документа обязательна")]
        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;
        
        public List<ReceiptResourceViewModel> Resources { get; set; } = new();
        
        [Display(Name = "Общее количество позиций")]
        public int TotalItems => Resources?.Count ?? 0;
    }
}
