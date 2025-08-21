using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Models.Resources;
using WarehouseManagement.Models.Units;

namespace WarehouseManagement.Models.Receipts
{
    public class ReceiptResource
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int ReceiptId { get; set; }
        
        [Required]
        public int ResourceId { get; set; }
        
        [Required]
        public int UnitId { get; set; }
        
        [Required(ErrorMessage = "Количество обязательно")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public decimal Quantity { get; set; }
        [ValidateNever]
        public virtual Receipt Receipt { get; set; } = null!;
        [ValidateNever]
        public virtual Resource Resource { get; set; } = null!;
        [ValidateNever]
        public virtual Unit Unit { get; set; } = null!;
    }
}
