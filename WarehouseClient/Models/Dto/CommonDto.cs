namespace WarehouseClient.Models.Dto
{
    public class OptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
