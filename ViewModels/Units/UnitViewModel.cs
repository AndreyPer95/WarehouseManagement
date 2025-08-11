using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Models.Units;

namespace WarehouseManagement.ViewModels.Units
{
    public class UnitViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Наименование обязательно")]
        [StringLength(50, ErrorMessage = "Наименование не может превышать 50 символов")]
        [Display(Name = "Наименование")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Статус")]
        public UnitStatus Status { get; set; }
        
        [Display(Name = "Статус")]
        public string StatusDisplay => Status == UnitStatus.Active ? "Активная" : "В архиве";
        
        public bool CanDelete { get; set; }
    }
}
