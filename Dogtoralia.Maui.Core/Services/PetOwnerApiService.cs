using System.Net.Http.Json;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Core.Services;

public class PetOwnerApiService : IPetOwnerApiService
{
    private readonly HttpClient _http;

    public PetOwnerApiService(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<PetOwnerDto>> GetAllAsync()
    {
        var owners = await _http.GetFromJsonAsync<List<PetOwnerDto>>("api/petowners", ApiJson.Options);
        return owners ?? new List<PetOwnerDto>();
    }
}
