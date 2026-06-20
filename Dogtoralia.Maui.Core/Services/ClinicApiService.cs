using System.Net.Http.Json;
using Dogtoralia.Shared.Dtos;

namespace Dogtoralia.Maui.Core.Services;

public class ClinicApiService : IClinicApiService
{
    private readonly HttpClient _http;

    public ClinicApiService(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<ClinicDto>> GetAllAsync()
    {
        var clinics = await _http.GetFromJsonAsync<List<ClinicDto>>("api/clinics", ApiJson.Options);
        return clinics ?? new List<ClinicDto>();
    }

    public Task<ClinicDto?> GetByIdAsync(int id) =>
        _http.GetFromJsonAsync<ClinicDto>($"api/clinics/{id}", ApiJson.Options);
}
