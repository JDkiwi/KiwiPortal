using KiwiPortal.Services;
using Microsoft.AspNetCore.Components;

namespace KiwiPortal.Shared;

public partial class NavMenu
{
    [Inject]
    private EnergyConsumptionService EnergyConsumptionService { get; set; } = null!;
    
    private bool _collapseNavMenu = true;
    
    private string LoginLogoutText => EnergyConsumptionService.LoggedIn ? "Abmelden" : "Anmelden";
    
    private string LoginLogoutIcon => EnergyConsumptionService.LoggedIn ? "oi oi-account-logout" : "oi oi-account-login";

    private string? NavMenuCssClass => _collapseNavMenu ? "collapse" : null;

    protected override void OnInitialized()
    {
        EnergyConsumptionService.LoggedInChanged += (_, _) => InvokeAsync(StateHasChanged);

        base.OnInitialized();
    }

    private void ToggleNavMenu()
    {
        _collapseNavMenu = !_collapseNavMenu;
    }
}
