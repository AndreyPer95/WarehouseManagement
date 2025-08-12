using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Models.Receipts
{
    public class Receipt
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Номер документа обязателен")]
        [StringLength(50, ErrorMessage = "Номер документа не может превышать 50 символов")]
        public string Number { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Дата документа обязательна")]
        public DateTime Date { get; set; } = DateTime.Now;

        public virtual ICollection<ReceiptResource> ReceiptResources { get; set; } = new List<ReceiptResource>();
    }
}
