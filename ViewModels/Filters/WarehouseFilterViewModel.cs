using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.ViewModels.Filters
{
    public class WarehouseFilterViewModel
    {
        [Display(Name = "Ресурсы")]
        public List<int> ResourceIds { get; set; } = new();
        
        [Display(Name = "Единицы измерения")]
        public List<int> UnitIds { get; set; } = new();
    }
}
