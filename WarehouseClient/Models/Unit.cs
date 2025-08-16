using System.ComponentModel.DataAnnotations;

namespace WarehouseClient.Models
{
    public class Unit
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Наименование обязательно")]
        [StringLength(50, ErrorMessage = "Наименование не должно превышать 50 символов")]
        [Display(Name = "Наименование")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Состояние")]
        public EntityState Status { get; set; } = EntityState.Active;
    }
}