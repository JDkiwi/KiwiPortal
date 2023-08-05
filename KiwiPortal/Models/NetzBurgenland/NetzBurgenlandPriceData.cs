using System.Text.Json.Serialization;

namespace KiwiPortal.Models.NetzBurgenland;

public class NetzBurgenlandPriceData
{
    [JsonPropertyName("start_timestamp")]
    public long StartTimestamp { get; set; }

    [JsonPropertyName("end_timestamp")]
    public long EndTimestamp { get; set; }

    public double MarketPrice { get; set; }
    
    public string Unit { get; set; } = string.Empty;

    public DateTimeOffset StartDate => DateTimeOffset.FromUnixTimeMilliseconds(StartTimestamp);
    
    public DateTimeOffset EndDate => DateTimeOffset.FromUnixTimeMilliseconds(EndTimestamp);
}
