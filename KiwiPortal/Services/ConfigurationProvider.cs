using System.Text.Json;
using KiwiPortal.Models;

namespace KiwiPortal.Services;

public class ConfigurationProvider
{
    private const string PortalConfigurationFileName = "PortalConfiguration.json";
    
    public PortalConfiguartion CurrentConfiguration { get; private set; } = new();

    public void LoadConfiguration()
    {
        if (!File.Exists(PortalConfigurationFileName))
        {
            return;
        }
        
        var jsonResult = File.ReadAllText(PortalConfigurationFileName);
        
        CurrentConfiguration = JsonSerializer.Deserialize<PortalConfiguartion>(jsonResult, DefaultJsonSerializerOptions.Create()) ?? new PortalConfiguartion();
    }
}
