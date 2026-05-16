using Dogtoralia.Shared.Dtos;
using System.Net.Http.Json;

namespace Dogtoralia.MVC.Services;

public class VeterinarianService : IVeterinarianService
{
    private readonly HttpClient _http;

    public VeterinarianService(HttpClient http) => _http = http;

    public async Task<IEnumerable<VeterinarianDto>> GetAllAsync()
        => await _http.GetFromJsonAsync<IEnumerable<VeterinarianDto>>("api/veterinarians") ?? [];

    public async Task<VeterinarianDto?> GetByIdAsync(int id)
        => await _http.GetFromJsonAsync<VeterinarianDto>($"api/veterinarians/{id}");

    public async Task<VeterinarianDto?> CreateAsync(VeterinarianWriteDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/veterinarians", dto);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<VeterinarianDto>()
            : null;
    }

    public async Task<bool> UpdateAsync(int id, VeterinarianWriteDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/veterinarians/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/veterinarians/{id}");
        return response.IsSuccessStatusCode;
    }
}
