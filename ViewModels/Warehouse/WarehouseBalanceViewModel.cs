using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.ViewModels.Warehouse
{
    public class WarehouseBalanceViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Ресурс")]
        public string ResourceName { get; set; } = string.Empty;
        
        [Display(Name = "Единица измерения")]
        public string UnitName { get; set; } = string.Empty;
        
        [Display(Name = "Количество")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal Quantity { get; set; }
        
        public int ResourceId { get; set; }
        public int UnitId { get; set; }
    }
}
