namespace KiwiPortal.Models;

public class AwattarPriceResult
{
    public string Object { get; set; } = string.Empty;
    
    public string Url { get; set; } = string.Empty;
    
    public IEnumerable<AwattarPriceData> Data { get; set; } = Array.Empty<AwattarPriceData>();
}
