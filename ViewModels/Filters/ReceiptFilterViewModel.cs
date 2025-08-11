using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.ViewModels.Filters
{
    public class ReceiptFilterViewModel
    {
        [Display(Name = "Дата с")]
        [DataType(DataType.Date)]
        public DateTime? DateFrom { get; set; }
        
        [Display(Name = "Дата по")]
        [DataType(DataType.Date)]
        public DateTime? DateTo { get; set; }
        
        [Display(Name = "Номера документов")]
        public List<string> Numbers { get; set; } = new();
        
        [Display(Name = "Ресурсы")]
        public List<int> ResourceIds { get; set; } = new();
        
        [Display(Name = "Единицы измерения")]
        public List<int> UnitIds { get; set; } = new();
    }
}
