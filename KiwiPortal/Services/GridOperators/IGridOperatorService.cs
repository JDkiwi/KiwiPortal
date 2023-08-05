using KiwiPortal.Models;

namespace KiwiPortal.Services.GridOperators;

public interface IGridOperatorService
{
    bool LoggedIn { get; }

    Task Login(string username, string password);

    Task Logout();
    
    Task<IEnumerable<ConsumptionResult>?> GetConsumptionData(MeteringPoint meteringPoint, DateOnly startDate, DateOnly endDate);
    
    Task<IEnumerable<MeteringPoint>?> GetMeteringPoints();
}
