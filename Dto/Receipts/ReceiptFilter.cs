namespace WarehouseManagementAPI.Dto.Receipts
{
    public class ReceiptFilter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public List<string>? Numbers { get; set; }
        public List<int>? ResourceIds { get; set; }
        public List<int>? UnitIds { get; set; }
    }
}
