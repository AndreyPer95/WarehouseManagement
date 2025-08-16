namespace WarehouseManagementAPI.Dto.Warehouse
{
    public class WarehouseBalanceRowDto
    {
        public int ResourceId { get; set; }
        public string ResourceName { get; set; } = "";
        public int UnitId { get; set; }
        public string UnitName { get; set; } = "";
        public decimal Quantity { get; set; }
    }
}
