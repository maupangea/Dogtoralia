using System.Text.Json;

namespace Dogtoralia.Maui.Core.Services;

internal static class ApiJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
