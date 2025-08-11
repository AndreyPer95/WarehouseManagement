using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Models.Receipts;

namespace WarehouseManagement.Models.Resources
{
    public class Resource
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Наименование обязательно")]
        [StringLength(200, ErrorMessage = "Наименование не может превышать 200 символов")]
        public string Name { get; set; } = string.Empty;
        
        public ResourceStatus Status { get; set; } = ResourceStatus.Active;

        public virtual ICollection<ReceiptResource> ReceiptResources { get; set; } = new List<ReceiptResource>();
    }
    
    public enum ResourceStatus
    {
        Active = 0,
        Archived = 1
    }
}
