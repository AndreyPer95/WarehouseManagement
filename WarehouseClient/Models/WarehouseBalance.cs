namespace WarehouseClient.Models
{
    public class WarehouseBalance
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public int UnitId { get; set; }
        public decimal Quantity { get; set; }
        
        // Навигационные свойства
        public Resource? Resource { get; set; }
        public Unit? Unit { get; set; }
    }
}