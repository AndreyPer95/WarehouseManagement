namespace WarehouseClient.Models
{
    public class ReceiptLine
    {
        public string ResourceName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
    }
}
