namespace KiwiPortal.Models.NetzBurgenland;

public class NetzBurgenlandMeteringPoint
{
    public string Identifier { get; set; } = string.Empty;

    public string MeteringPointType { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string HouseNumber { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public IEnumerable<NetzBurgenlandDataPoint> DataPoints { get; set; } = Enumerable.Empty<NetzBurgenlandDataPoint>();
}