namespace WarehouseManagementAPI.Dto.Receipts
{
    public class ReceiptWithLinesDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = "";
        public DateTime Date { get; set; }
        public List<ReceiptLineDto> Lines { get; set; } = new();
    }
}
