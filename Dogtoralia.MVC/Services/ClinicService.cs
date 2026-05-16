using Dogtoralia.Shared.Dtos;
using System.Net.Http.Json;

namespace Dogtoralia.MVC.Services;

public class ClinicService : IClinicService
{
    private readonly HttpClient _http;

    public ClinicService(HttpClient http) => _http = http;

    public async Task<IEnumerable<ClinicDto>> GetAllAsync()
        => await _http.GetFromJsonAsync<IEnumerable<ClinicDto>>("api/clinics") ?? [];

    public async Task<ClinicDto?> GetByIdAsync(int id)
        => await _http.GetFromJsonAsync<ClinicDto>($"api/clinics/{id}");

    public async Task<ClinicDto?> CreateAsync(ClinicWriteDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/clinics", dto);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<ClinicDto>()
            : null;
    }

    public async Task<bool> UpdateAsync(int id, ClinicWriteDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/clinics/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/clinics/{id}");
        return response.IsSuccessStatusCode;
    }
}
