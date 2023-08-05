using System.Text.Json;

namespace KiwiPortal;

public static class DefaultJsonSerializerOptions
{
    public static JsonSerializerOptions Create()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        return options;
    }
}
