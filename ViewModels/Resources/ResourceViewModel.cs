using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Models.Resources;

namespace WarehouseManagement.ViewModels.Resources
{
    public class ResourceViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Наименование обязательно")]
        [StringLength(200, ErrorMessage = "Наименование не может превышать 200 символов")]
        [Display(Name = "Наименование")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Статус")]
        public ResourceStatus Status { get; set; }
        
        [Display(Name = "Статус")]
        public string StatusDisplay => Status == ResourceStatus.Active ? "Активный" : "В архиве";
        
        public bool CanDelete { get; set; }
    }
}
