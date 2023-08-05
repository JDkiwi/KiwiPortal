namespace KiwiPortal.Models;

public class ConsumptionResult
{
    public string Threshold { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public bool FeedIn { get; set; }
    
    public string DeviceExceptionStatus { get; set; } = string.Empty;
    
    public IEnumerable<ConsumptionData> Data { get; set; } = Enumerable.Empty<ConsumptionData>();
    
    public bool EstimatedSeries { get; set; }
    
    public bool IsEmpty { get; set; }
}
