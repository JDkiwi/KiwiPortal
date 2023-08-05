using AutoMapper;
using KiwiPortal.Models.NetzBurgenland;

namespace KiwiPortal.Models;

public class ModelMappingProfile : Profile
{
    public ModelMappingProfile()
    {
        // NetzBurgenland Mapping
        CreateMap<NetzBurgenlandConsumptionData, ConsumptionData>();
        CreateMap<NetzBurgenlandConsumptionResult, ConsumptionResult>();
        CreateMap<NetzBurgenlandMeteringPoint, MeteringPoint>();
        CreateMap<NetzBurgenlandPriceData, AwattarPriceData>();
        CreateMap<NetzBurgenlandPriceResult, AwattarPriceResult>();
    }
}
