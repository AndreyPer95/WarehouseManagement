using System.Text;
using System.Text.Json;
using WarehouseClient.Models;

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
            var response = await _httpClient.PutAsync($"api/resources/{id}/archive", null);
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
            var response = await _httpClient.PutAsync($"api/units/{id}/archive", null);
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
        public async Task<List<WarehouseBalance>> GetWarehouseBalanceAsync()
        {
            var response = await _httpClient.GetAsync("api/warehouse/balance");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<WarehouseBalance>>(json, _jsonOptions) ?? new List<WarehouseBalance>();
        }
    }
}