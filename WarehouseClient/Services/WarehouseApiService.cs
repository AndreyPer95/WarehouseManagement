using System.Text;
using System.Text.Json;
using WarehouseClient.Dto;
using WarehouseClient.Models;
using WarehouseClient.Models.Dto;

namespace WarehouseClient.Services
{
    public class WarehouseApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public WarehouseApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // Resources
        public async Task<List<Resource>> GetResourcesAsync()
        {
            var response = await _httpClient.GetAsync("api/resources");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Resource>>(json, _jsonOptions) ?? new List<Resource>();
        }

        public async Task<List<Resource>> GetActiveResourcesAsync()
        {
            var response = await _httpClient.GetAsync("api/resources/active");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Resource>>(json, _jsonOptions) ?? new List<Resource>();
        }
        public async Task<Resource?> GetResourceByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/resources/{id}");
            if (!response.IsSuccessStatusCode)
                return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Resource>(json, _jsonOptions);
        }

        public async Task<Resource> CreateResourceAsync(Resource resource)
        {
            var json = JsonSerializer.Serialize(resource);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/resources", content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Resource>(responseJson, _jsonOptions)!;
        }

        public async Task<bool> ArchiveResourceAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/resources/{id}/archive", null);
            return response.IsSuccessStatusCode;
        }

        // Units
        public async Task<List<Unit>> GetUnitsAsync()
        {
            var response = await _httpClient.GetAsync("api/units");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Unit>>(json, _jsonOptions) ?? new List<Unit>();
        }
        public async Task<List<Unit>> GetActiveUnitsAsync()
        {
            var response = await _httpClient.GetAsync("api/units/active");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Unit>>(json, _jsonOptions) ?? new List<Unit>();
        }

        public async Task<Unit?> GetUnitByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/units/{id}");
            if (!response.IsSuccessStatusCode)
                return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Unit>(json, _jsonOptions);
        }

        public async Task<Unit> CreateUnitAsync(Unit unit)
        {
            var json = JsonSerializer.Serialize(unit);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/units", content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Unit>(responseJson, _jsonOptions)!;
        }

        public async Task<bool> ArchiveUnitAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/units/{id}/archive", null);
            return response.IsSuccessStatusCode;
        }
        
        // Receipts
        public async Task<List<Receipt>> GetReceiptsAsync()
        {
            var response = await _httpClient.GetAsync("api/receipts");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Receipt>>(json, _jsonOptions) ?? new List<Receipt>();
        }

        // Новый метод для получения фильтрованных поступлений
        public async Task<List<Receipt>> GetFilteredReceiptsAsync(
            DateTime? dateFrom, 
            DateTime? dateTo, 
            List<string>? numbers, 
            List<int>? resourceIds, 
            List<int>? unitIds)
        {
            var queryParams = new List<string>();
            
            if (dateFrom.HasValue)
                queryParams.Add($"from={dateFrom.Value:yyyy-MM-dd}");
            
            if (dateTo.HasValue)
                queryParams.Add($"to={dateTo.Value:yyyy-MM-dd}");
            
            if (numbers != null && numbers.Any())
            {
                foreach (var number in numbers)
                    queryParams.Add($"numbers={Uri.EscapeDataString(number)}");
            }
            
            if (resourceIds != null && resourceIds.Any())
            {
                foreach (var id in resourceIds)
                    queryParams.Add($"resourceIds={id}");
            }
            
            if (unitIds != null && unitIds.Any())
            {
                foreach (var id in unitIds)
                    queryParams.Add($"unitIds={id}");
            }
            
            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetAsync($"api/receipts{queryString}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            
            // Десериализация DTO с линиями и преобразование в модель Receipt
            var receiptsDto = JsonSerializer.Deserialize<List<ReceiptWithLinesDto>>(json, _jsonOptions) ?? new List<ReceiptWithLinesDto>();
            
            // Преобразуем DTO в модели для отображения
            var receipts = new List<Receipt>();
            foreach (var dto in receiptsDto)
            {
                var receipt = new Receipt
                {
                    Id = dto.Id,
                    Number = dto.Number,
                    Date = dto.Date,
                    ReceiptResources = dto.Lines?.Select(l => new ReceiptResource
                    {
                        ResourceId = l.ResourceId,
                        Resource = new Resource { Id = l.ResourceId, Name = l.ResourceName },
                        UnitId = l.UnitId,
                        Unit = new Unit { Id = l.UnitId, Name = l.UnitName },
                        Quantity = l.Quantity
                    }).ToList() ?? new List<ReceiptResource>()
                };
                receipts.Add(receipt);
            }
            
            return receipts;
        }

        // Новый метод для получения номеров документов
        public async Task<List<string>> GetReceiptNumbersAsync()
        {
            var response = await _httpClient.GetAsync("api/receipts/filters/numbers");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<string>>(json, _jsonOptions) ?? new List<string>();
        }

        public async Task<Receipt?> GetReceiptByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/receipts/{id}");
            if (!response.IsSuccessStatusCode)
                return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Receipt>(json, _jsonOptions);
        }

        public async Task<Receipt> CreateReceiptAsync(Receipt receipt)
        {
            var json = JsonSerializer.Serialize(receipt);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/receipts", content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Receipt>(responseJson, _jsonOptions)!;
        }

        public async Task<bool> DeleteReceiptAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/receipts/{id}");
            return response.IsSuccessStatusCode;
        }

        // Warehouse Balance
        public async Task<List<WarehouseBalanceRowDto>> GetWarehouseBalanceAsync(List<int>? resourceIds = null, List<int>? unitIds = null)
        {
            var queryParams = new List<string>();
            
            if (resourceIds != null && resourceIds.Any())
            {
                foreach (var id in resourceIds)
                    queryParams.Add($"resourceIds={id}");
            }
            
            if (unitIds != null && unitIds.Any())
            {
                foreach (var id in unitIds)
                    queryParams.Add($"unitIds={id}");
            }
            
            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetAsync($"api/warehouse/balance{queryString}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<WarehouseBalanceRowDto>>(json, _jsonOptions) ?? new List<WarehouseBalanceRowDto>();
        }
    }
}