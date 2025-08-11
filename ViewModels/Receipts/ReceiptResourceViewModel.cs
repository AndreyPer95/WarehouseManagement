using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.ViewModels.Receipts
{
    public class ReceiptResourceViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Выберите ресурс")]
        [Display(Name = "Ресурс")]
        public int ResourceId { get; set; }
        
        [Display(Name = "Ресурс")]
        public string? ResourceName { get; set; }
        
        [Required(ErrorMessage = "Выберите единицу измерения")]
        [Display(Name = "Единица измерения")]
        public int UnitId { get; set; }
        
        [Display(Name = "Единица измерения")]
        public string? UnitName { get; set; }
        
        [Required(ErrorMessage = "Укажите количество")]
        [Range(0.001, double.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        [Display(Name = "Количество")]
        public decimal Quantity { get; set; }
    }
}
