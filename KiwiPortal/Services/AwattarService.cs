using System.Text.Json;
using KiwiPortal.Models;

namespace KiwiPortal.Services;

public class AwattarService
{
    private readonly HttpClient _httpClient;
    
    public AwattarService()
    {
        _httpClient = new HttpClient();
    }
    
    public async Task<AwattarPriceResult?> GetPriceData(DateOnly startDate, DateOnly endDate)
    {
        var dateStart = startDate.ToDateTime(new TimeOnly(0, 0, 0));
        var dateEnd = endDate.AddDays(1).ToDateTime(new TimeOnly(0, 0, 0));
        var timestampStart = new DateTimeOffset(dateStart).ToUnixTimeMilliseconds();
        var timestampEnd = new DateTimeOffset(dateEnd).ToUnixTimeMilliseconds();
        var url = $"https://api.awattar.at/v1/marketdata?start={timestampStart}&end={timestampEnd}";
        var result = await _httpClient.GetAsync(url);
        var jsonResult = await result.Content.ReadAsStringAsync();
        
        return JsonSerializer.Deserialize<AwattarPriceResult>(jsonResult,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true, });
    }
}
