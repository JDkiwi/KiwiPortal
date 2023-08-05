namespace KiwiPortal.Models.NetzBurgenland;

public class NetzBurgenlandConsumptionData
{
    public DateTimeOffset StartTimestamp { get; set; }
    
    public DateTimeOffset EndTimestamp { get; set; }
    
    public double Value { get; set; }
    
    public double Reading { get; set; }
    
    public double PowerValue { get; set; }
}
