using AutoMapper;
using KiwiPortal.Models;
using KiwiPortal.Services.GridOperators;
using KiwiPortal.Services.GridOperators.NetzBurgenland;

namespace KiwiPortal.Services;

public class EnergyConsumptionService
{
    public event EventHandler? LoggedInChanged;

    public bool LoggedIn => _energyProviderService?.LoggedIn ?? false;
    
    public MeteringPoint? SelectedMeteringPoint { get; private set; }
    
    private IGridOperatorService? _energyProviderService;
    
    private IMapper _mapper;

    public EnergyConsumptionService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task Login(string username, string password, GridOperator provider)
    {
        _energyProviderService = provider switch
        {
            GridOperator.NetzBurgenland => new NetzBurgenlandService(_mapper),
            _ => throw new NotSupportedException($"Provider {provider} is not supported!")
        };
        
        await _energyProviderService.Login(username, password);

        var meteringPoints = await _energyProviderService.GetMeteringPoints();
        
        
        // TODO: currently we just take the first one here... we should probably let the user choose
        SelectedMeteringPoint = meteringPoints?.FirstOrDefault();
        
        LoggedInChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task Logout()
    {
        if (_energyProviderService is null)
        {
            return;
        }
        
        await _energyProviderService.Logout();
        
        LoggedInChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task<IEnumerable<ConsumptionResult>?> GetConsumptionData(DateOnly startDate, DateOnly endDate)
    {
        if (_energyProviderService is not null && SelectedMeteringPoint is not null)
        {
            return await _energyProviderService.GetConsumptionData(SelectedMeteringPoint, startDate, endDate);
        }

        return Enumerable.Empty<ConsumptionResult>();
    }
    
}
