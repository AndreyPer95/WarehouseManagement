namespace WarehouseClient.Dto
{
    public class ReceiptWithLinesDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = "";
        public DateTime Date { get; set; }
        public List<ReceiptLineDto> Lines { get; set; } = new();
    }

    public class ReceiptLineDto
    {
        public int ResourceId { get; set; }
        public string ResourceName { get; set; } = "";
        public int UnitId { get; set; }
        public string UnitName { get; set; } = "";
        public decimal Quantity { get; set; }
    }

    public class ReceiptFilter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public List<string>? Numbers { get; set; }
        public List<int>? ResourceIds { get; set; }
        public List<int>? UnitIds { get; set; }
    }
}
