namespace KiwiPortal.Models.NetzBurgenland;

public class NetzBurgenlandPriceResult
{
    public string Object { get; set; } = string.Empty;
    
    public string Url { get; set; } = string.Empty;
    
    public IEnumerable<NetzBurgenlandPriceData> Data { get; set; } = Enumerable.Empty<NetzBurgenlandPriceData>();
}
