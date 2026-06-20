using System.Net.Http.Json;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Core.Services;

public class VeterinarianApiService : IVeterinarianApiService
{
    private readonly HttpClient _http;

    public VeterinarianApiService(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<VeterinarianDto>> GetAllAsync()
    {
        var vets = await _http.GetFromJsonAsync<List<VeterinarianDto>>("api/veterinarians", ApiJson.Options);
        return vets ?? new List<VeterinarianDto>();
    }

    public Task<VeterinarianDto?> GetByIdAsync(int id) =>
        _http.GetFromJsonAsync<VeterinarianDto>($"api/veterinarians/{id}", ApiJson.Options);
}
