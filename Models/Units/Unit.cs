using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Models.Units
{
    public class Unit
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Наименование обязательно")]
        [StringLength(50, ErrorMessage = "Наименование не может превышать 50 символов")]
        public string Name { get; set; } = string.Empty;
        
        public UnitStatus Status { get; set; } = UnitStatus.Active;
        
        // Навигационные свойства
        public virtual ICollection<ReceiptResource> ReceiptResources { get; set; } = new List<ReceiptResource>();
    }
    
    public enum UnitStatus
    {
        Active = 0,
        Archived = 1
    }
}
