namespace KiwiPortal.Models;

public class ConsumptionData
{
    public DateTimeOffset StartTimestamp { get; set; }
    
    public DateTimeOffset EndTimestamp { get; set; }
    
    public double Value { get; set; }
    
    public double Reading { get; set; }
    
    public double PowerValue { get; set; }
}
