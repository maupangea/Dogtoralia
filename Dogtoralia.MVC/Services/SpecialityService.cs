using Dogtoralia.Shared.Dtos;
using System.Net.Http.Json;

namespace Dogtoralia.MVC.Services;

public class SpecialityService : ISpecialityService
{
    private readonly HttpClient _http;

    public SpecialityService(HttpClient http) => _http = http;

    public async Task<IEnumerable<SpecialityDto>> GetAllAsync()
        => await _http.GetFromJsonAsync<IEnumerable<SpecialityDto>>("api/specialities") ?? [];

    public async Task<SpecialityDto?> GetByIdAsync(int id)
        => await _http.GetFromJsonAsync<SpecialityDto>($"api/specialities/{id}");

    public async Task<SpecialityDto?> CreateAsync(SpecialityWriteDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/specialities", dto);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<SpecialityDto>()
            : null;
    }

    public async Task<bool> UpdateAsync(int id, SpecialityWriteDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/specialities/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/specialities/{id}");
        return response.IsSuccessStatusCode;
    }
}
