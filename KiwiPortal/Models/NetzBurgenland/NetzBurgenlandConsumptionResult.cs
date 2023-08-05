namespace KiwiPortal.Models.NetzBurgenland;

public class NetzBurgenlandConsumptionResult
{
    public string Threshold { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public bool FeedIn { get; set; }
    
    public string DeviceExceptionStatus { get; set; } = string.Empty;
    
    public IEnumerable<NetzBurgenlandConsumptionData> Data { get; set; } = Enumerable.Empty<NetzBurgenlandConsumptionData>();
    
    public bool EstimatedSeries { get; set; }
    
    public bool IsEmpty { get; set; }
}
