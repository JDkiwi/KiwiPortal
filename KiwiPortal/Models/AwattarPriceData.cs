namespace KiwiPortal.Models;

public class AwattarPriceData
{
    public long StartTimestamp { get; set; }

    public long EndTimestamp { get; set; }

    public double MarketPrice { get; set; }
    
    public string Unit { get; set; } = string.Empty;

    public DateTimeOffset StartDate => DateTimeOffset.FromUnixTimeMilliseconds(StartTimestamp);
    
    public DateTimeOffset EndDate => DateTimeOffset.FromUnixTimeMilliseconds(EndTimestamp);
}
