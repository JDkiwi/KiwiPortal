using KiwiPortal.Services;
using KiwiPortal.Services.GridOperators;
using Microsoft.AspNetCore.Components;

namespace KiwiPortal.Pages;

public partial class Index
{
    [Inject]
    public EnergyConsumptionService EnergyProviderService { get; set; } = null!;
    
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    private string _username = string.Empty;

    private string _password = string.Empty;
    
    private string _provider = "0";

    private bool _loggingIn;

    private bool _loggingOut;
    
    private string? _errorMessage;
    
    private Dictionary<GridOperator, string> _providers = new()
    {
        {GridOperator.NetzBurgenland, "Netz Burgenland"}
    };

    private async Task OnLoginClicked()
    {
        _loggingIn = true;

        await InvokeAsync(StateHasChanged);

        try
        {
            var provider = (GridOperator) int.Parse(_provider);

            await EnergyProviderService.Login(_username, _password, provider);
        }
        catch
        {
            _errorMessage = "Einloggen fehlgeschlagen!";
        }

        _loggingIn = false;

        if (EnergyProviderService.LoggedIn)
        {
            NavigationManager.NavigateTo("/calculator");
        }
        else
        {
            _errorMessage = "Einloggen fehlgeschlagen!";
        }
    }
    
    private async Task OnLogoutClicked()
    {
        _loggingOut = true;

        await InvokeAsync(StateHasChanged);

        try
        {
            await EnergyProviderService.Logout();
        }
        catch
        {
            // ignored
        }

        _loggingOut = false;
    }
}
